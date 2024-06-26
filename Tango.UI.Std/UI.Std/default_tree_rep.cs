﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Tango.Data;
using Tango.Html;
using Tango.UI;
using Tango.UI.Controls;
using Tango.UI.Std;

namespace Tango.UI.Std
{
	public abstract class default_tree_rep<TResult> : default_list_rep<TResult>
		where TResult : ILazyListTree
	{
		readonly List<TreeLevelDescriptionItem<TResult>> _templateCollection = new List<TreeLevelDescriptionItem<TResult>>();
		Dictionary<int, TreeLevelDescriptionItem<TResult>> _templatesDict = new Dictionary<int, TreeLevelDescriptionItem<TResult>>();
		protected TreeLevelDescriptionItem<TResult> GetTemplateByID(int id) => _templatesDict[id];
		protected IEnumerable<(int level, TreeLevelDescriptionItem<TResult> item)> GetTemplates()
		{
			var stack = new Stack<(int level, IEnumerator<TreeLevelDescriptionItem<TResult>> en)>();
			stack.Push((0, _templateCollection.GetEnumerator()));

			while (stack.Count > 0)
			{
				var obj = stack.Peek();
				if (obj.en.MoveNext())
				{
					var t = obj.en.Current;
					yield return (obj.level, t);

					stack.Push((obj.level + 1, t.Children.GetEnumerator()));
				}
				else
				{
					stack.Pop().en.Dispose();
				}
			}
		}

		int _count = 0;

		string _highlightedRowID = null;

		bool _renderSelectedBlockMode = false;

		State InitialState = new State { };
		State CurrentState = null;


		protected override bool EnableViews => false;
		protected override bool EnableHover => false;
		protected override bool EnableKeyboard => true;
		protected virtual bool AutoExpandSingles => true;
		protected virtual bool AutoExpandOnSearch => false;
		public ObjectSetSettings ObjectSetSettings { get; set; } = null;

		public default_tree_rep()
		{
			CurrentState = InitialState;
		}

		public override void OnInit()
		{
			base.OnInit();

			var level = Context.GetIntArg("level", -1);
			level++;

			InitialState.Level = level;

			Renderer = new TreeListRenderer<TResult>(ID, Paging, level);
		}


		public override int GetCount()
		{
			return _count;
		}

		protected virtual void BeforeGetPageData(IDbTransaction dbTransaction)
		{

		}

		protected override IEnumerable<TResult> GetPageData()
		{
			if (_result != null)
				return _result;

			IEnumerable<TResult> pageData = null;
			_count = 0;

			foreach (var gs in Fields.GroupSorting)
				Sorter.AddOrderBy(gs.SeqNo, gs.SortDesc, true);

			var filtered = ApplyFilter(Data);

			if (Repository.AllObjectsQuery.StartsWith("@"))
			{
				var (filters, parms) = Filter.GetSqlFilters();
				Repository.AllObjectsQuery = EmbeddedResourceManager.GetString(typeof(TResult), Repository.AllObjectsQuery.Substring(1), filters);

				foreach (var pair in parms)
					Repository.Parameters.Add(pair.Key, pair.Value);
			}

			var where = new List<string>();

			if (CurrentState.Parms != null)
			{
				foreach (var pair in CurrentState.Parms)
				{
					if (pair.Value == null) continue;
					where.Add($"{pair.Key} = @lf_{pair.Key}");
					Repository.Parameters["lf_" + pair.Key] = pair.Value;
				}
			}

			var dialect = QueryHelper.CreateDialect(Database.GetDBType());
			var q = Sorter.Count > 0 ? Sorter.Apply(filtered) : DefaultOrderBy(filtered);

			var nodeTemplates = CurrentState.Level == 0 ?
				new List<TreeLevelDescriptionItem<TResult>> { CurrentState.TemplateItem } :
				CurrentState.TemplateItem.Children;

			using (var tran = Database.BeginTransaction())
			{
				BeforeGetPageData(tran);

				foreach (var t in nodeTemplates)
				{
					var templateAllObjectsQuery = Repository.AllObjectsQuery;
					var templateRepository = GetRepository();

					foreach (var p in Repository.Parameters)
						if (!templateRepository.Parameters.ContainsKey(p.Key))
							templateRepository.Parameters.Add(p);

					var nodeQuery = q;
					if (Sections.RenderPaging)
						nodeQuery = Paging.Apply(nodeQuery, true);
					var nodeQueryCnt = q;

					var nodeWhere = new List<string>(where);

					if (t.Where != null)
					{
						nodeQuery = nodeQuery.Where(t.Where);
						nodeQueryCnt = nodeQueryCnt.Where(t.Where);
					}

					nodeQuery = t.Template.OrderBy(nodeQuery);

					var expr = t.Template.GroupBy != null ? nodeQuery.GroupBy(t.Template.GroupBy).Select(t.Template.GroupBySelector).Expression : nodeQuery.Expression;
					var exprCnt = t.Template.GroupBy != null ? nodeQueryCnt.GroupBy(t.Template.GroupBy).Select(x => x.Key).Expression : nodeQueryCnt.Expression;

					if (!t.Template.CustomQuery.IsEmpty())
						templateAllObjectsQuery = t.Template.CustomQuery;

					var sqlTemplate = $"select * from ({templateAllObjectsQuery}) t";

					if (!t.Template.AllowNulls)
						foreach (var p in t.Template.KeyProperties)
							nodeWhere.Add($"{p} is not null");

					if (nodeWhere.Count > 0)
						sqlTemplate += " where " + nodeWhere.Join(" and ");

					templateRepository.AllObjectsQuery = sqlTemplate;
					var res = templateRepository.List(expr);
					var resCnt = Sections.RenderPaging ? templateRepository.Count(exprCnt) : res.Count();

					foreach (var o in res)
						o.Template = t.Template.ID;


					pageData = pageData == null ? res : pageData.Concat(res);
					_count += resCnt;
				}
			}

			if (pageData == null) pageData = new List<TResult>();

			return pageData;
		}

		//public void SetHightlighed(int templateID, Dictionary<string, object> parms)
		//{
		//	var s = $"level={templateID - 1}&" + parms.Select(x => $"{x.Key}={x.Value}").Join("&");
		//	_highlightedRowID = s;
		//}

		/// <summary>
		/// Предраскрытие уровня
		/// </summary>
		/// <param name="templateID">ID шаблона уровня с заданным элементом</param>
		/// <param name="level">Уровень, где находится заданный элемент (корневой уровень = 1)</param>
		/// <param name="parms">Параметры для построения уровня с заданным элементом</param>
		/// <param name="highlight">Нужно ли подсвечивать элемент, до которого раскрываем</param>
		public void SetExpandedItem(int templateID, int level, Dictionary<string, object> parms, bool highlight = true, bool expandNext = false)
		{
			if (Context.Event == nameof(OnQuickSearch).ToLower() && !Context.GetArg(_qSearchParmName.Name).IsEmpty())
				return;
			var initialTemplate = _templatesDict[templateID];
			var template = level == 1 ? initialTemplate : initialTemplate.ParentTemplateItem;
			var levelForExpandNext = level;

			var sqlTemplate = PrepareQuery(template.Template, new List<Dictionary<string, object>> { parms });

			using (var tran = Database.BeginTransaction())
			{
				BeforeGetPageData(tran);

				var temp = Database.Connection.QueryFirstOrDefault<TResult>(sqlTemplate, Repository.Parameters, tran);
				if (temp == null) return;

				var states = new List<State>();
				var senders = new List<string>();

				level--;

				if (highlight)
					_highlightedRowID = GetClientID(initialTemplate.Template.GetHtmlRowID(level, temp));

				while (template != null)
				{
					states.Add(new State {
						Level = level,
						TemplateItem = template,
						Parms = template.Template.GetKeyCollection(temp)
					});
					senders.Add(template.Template.GetHtmlRowID(level - 1, temp));

					template = template.ParentTemplateItem;
					level--;
				}

				if (expandNext)
				{

					states.Insert(0, new State {
						Level = levelForExpandNext,
						TemplateItem = initialTemplate,
						Parms = initialTemplate.Template.GetKeyCollection(temp)
					});

					senders.Insert(0, (initialTemplate.Template.GetHtmlRowID(levelForExpandNext - 1, temp)));
				}

				senders.Reverse();
				states.Reverse();

				var s = CurrentState;
				s.Children.Clear();
				for (int i = 0; i < states.Count; i++)
				{
					if (!s.Children.ContainsKey(senders[i]))
						s.Children.Add(senders[i], states[i]);

					s = states[i];
				}
			}
		}

		Dictionary<string, TreeNode<(State state, TResult row)>> _selectedDataRows = new Dictionary<string, TreeNode<(State state, TResult row)>>();
		List<TreeNode<(State state, TResult row)>> _selectedDataRoot = new List<TreeNode<(State state, TResult row)>>();
		HashSet<string> _selectedValues = new HashSet<string>();

		public void SetSelectedItems(int templateID, int level, Expression<Func<TResult, bool>> predicate)
		{
			SetSelectedItemsOne(templateID, level, predicate);

			(Renderer as TreeListRenderer<TResult>).SetSelectedValues(_selectedValues);
		}

		protected void SetSelectedItems(IEnumerable<(int templateID, int level, Expression<Func<TResult, bool>> predicate)> items)
		{
			_selectedValues.Clear();
			_selectedDataRows.Clear();
			_selectedDataRoot.Clear();
			foreach (var item in items)
			{
				SetSelectedItemsOne(item.templateID, item.level, item.predicate, true);
			}
			(Renderer as TreeListRenderer<TResult>).SetSelectedValues(_selectedValues);
		}

		private void SetSelectedItemsOne(int templateID, int level, Expression<Func<TResult, bool>> predicate, bool isObjectSet = false)
		{
			var q = Enumerable.Empty<TResult>().AsQueryable().Where(predicate);

			var template = _templatesDict[templateID];
			level--;

			var expr = template.Template.GroupBy != null ?
					q.GroupBy(template.Template.GroupBy).Select(template.Template.GroupBySelector).Expression :
					q.Expression;

			//TODO необходимо доработать метод т.к. вызов может быть в цикле по большому количеству данных (в случае применения хранимых наборов )

			IRepository<TResult> templateRepository;
			if ((ObjectSetSettings?.TableName).IsEmpty() || !isObjectSet)
				templateRepository = Repository;
			else
				templateRepository = Database.Repository<TResult>().WithAllObjectsQuery(EmbeddedResourceManager.GetString(typeof(TResult), ObjectSetSettings.TableName));

			var data = templateRepository.List(expr);

			foreach (var row in data)
			{
				var t = template;
				var lev = level;

				var key = t.Template.GetDataRowID(lev, row);
				var htmlKey = t.Template.GetHtmlRowID(lev, row);
				_selectedValues.Add(key);

				while (t.ParentTemplateItem != null)
				{
					var parentHtmlKey = t.ParentTemplateItem.Template.GetHtmlRowID(lev - 1, row);
					var rowState = new State { Level = lev, TemplateItem = t, IsChecked = true };
					var parentState = new State { Level = lev - 1, TemplateItem = t.ParentTemplateItem };

					if (_selectedDataRows.TryGetValue(parentHtmlKey, out var parent))
					{
						if (!_selectedDataRows.TryGetValue(htmlKey, out var ch))
							parent.AddChild((rowState, row));
						else
						{
							ch.Data.state.IsChecked = true;
							if (!parent.Children.Contains(ch))
								parent.Children.Add(ch);
						}
						break;
					}
					else
					{
						parent = new TreeNode<(State state, TResult row)>((parentState, row));
						if (!_selectedDataRows.TryGetValue(htmlKey, out var ch))
							parent.AddChild((rowState, row));
						else
							parent.Children.Add(ch);

						_selectedDataRows.Add(parentHtmlKey, parent);
						if (lev - 1 == 0)
							_selectedDataRoot.Add(parent);
					}
					t = t.ParentTemplateItem;
					lev--;
					htmlKey = parentHtmlKey;
				}
			}
		}

		string PrepareQuery(TreeLevelDescription<TResult> template, List<Dictionary<string, object>> parms)
		{
			var t = typeof(TResult);
			var nodeWhere = new List<string>();

			foreach (var gr in parms)
			{
				var s = new List<string>();
				foreach (var pair in gr)
				{
					var p = t.GetProperty(pair.Key);
					if (p.PropertyType.In(typeof(int), typeof(int?), typeof(decimal), typeof(decimal?)))
						s.Add($"{pair.Key} = {pair.Value}");
					else if (p.PropertyType.In(typeof(string), typeof(Guid), typeof(Guid?)))
						s.Add($"{pair.Key} = '{pair.Value}'");
					else if (p.PropertyType.In(typeof(DateTime), typeof(DateTime?)))
						s.Add($"{pair.Key} = '{pair.Value:yyyy-MM-dd HH:mm:ss}'");
				}
				if (s.Count > 0)
					nodeWhere.Add("(" + s.Join(" and ") + ")");
			}

			var origAllObjectsQuery = Repository.AllObjectsQuery;
			if (template.CustomQuery != null && !template.CustomQuery.IsEmpty())
				origAllObjectsQuery = template.CustomQuery;

			var sqlTemplate = "select *";
			sqlTemplate += $" from ({origAllObjectsQuery}) t";
			if (nodeWhere.Count > 0)
				sqlTemplate += " where " + nodeWhere.Join(" or ");

			return sqlTemplate;
		}

		public IEnumerable<TResult> GetSelectedObjects()
		{
			var ids = Context.GetListArg<string>(Constants.SelectedValues);

			var dict = new Dictionary<int, List<Dictionary<string, object>>>();

			foreach (var id in ids)
			{
				var parms = new Dictionary<string, object>();
				int templateid = -1;
				foreach (var item in id.Split('&'))
					foreach (var (cur, next) in item.Split('=').PairwiseWithNext())
					{
						if (next == null) continue;
						if (cur == "level") continue;
						if (cur == "template")
						{
							templateid = next.ToInt32(-1);
							continue;
						}
						parms.Add(cur, next);
					}

				if (templateid > 0)
					if (dict.TryGetValue(templateid, out var list))
						list.Add(parms);
					else
						dict.Add(templateid, new List<Dictionary<string, object>> { parms });
			}

			var res = new List<TResult>();
			foreach (var d in dict)
			{
				var t = _templatesDict[d.Key];
				var sql = PrepareQuery(t.Template, d.Value);
				var objs = Database.Connection.Query<TResult>(sql, Repository.Parameters);
				res.AddRange(objs);
			}

			return res;
		}

		/*[Obsolete]
		public void ExpandTree(ApiResponse response, string rowId, bool refreshtree)
		{
			if (_fields == null)
				_fields = FieldsConstructor();

			if (refreshtree)
				response.AddWidget(Sections.ContentBody, Render);

			int level;

			if (!rowId.IsEmpty())
			{
				var nodeWhere = new List<string>();

				var whereDict = new Dictionary<string, string>();

				foreach (var item in rowId.Split('&'))
					foreach (var (cur, next) in item.Split('=').PairwiseWithNext())
					{
						if (next == null) continue;
						whereDict.Add(cur, next);
					}

				if (whereDict.TryGetValue("level", out var value))
					level = Int32.Parse(value);
				else
					throw new Exception("Невозможно определить уровень строки");

				whereDict.Remove("level");
				whereDict.Remove("template");

				var t = typeof(TResult);
				foreach (var pair in whereDict)
				{
					var p = t.GetProperty(pair.Key);
					if (p.PropertyType.In(typeof(int), typeof(int?), typeof(decimal), typeof(decimal?)))
						nodeWhere.Add($"{pair.Key} = {pair.Value}");
					else if (p.PropertyType.In(typeof(string), typeof(Guid), typeof(Guid?)))
						nodeWhere.Add($"{pair.Key} = '{pair.Value}'");
					else if (p.PropertyType.In(typeof(DateTime), typeof(DateTime?)))
						nodeWhere.Add($"{pair.Key} = '{pair.Value:yyyy-MM-dd HH:mm:ss}'");
				}

				var origAllObjectsQuery = Repository.AllObjectsQuery;

				var sqlTemplate = "select *";
				sqlTemplate += $" from ({origAllObjectsQuery}) t";


				if (nodeWhere.Count > 0)
					sqlTemplate += " where " + nodeWhere.Join(" and ");

				List<string> rowsId = new List<string>();

				using (var tran = Database.BeginTransaction())
				{
					BeforeGetPageData(tran);

					var temp = Database.Connection.QueryFirstOrDefault<TResult>(sqlTemplate, Repository.Parameters, tran);
					if (temp == null) return;

					var id = level;
					var template = _templatesDict[id + 1];

					while (template != null)
					{
						var row = template.Template.GetDataRowID(id, temp);
						if (!template.Template.IsTerminal)
							rowsId.Add(row);

						id--;
						template = template.ParentTemplateItem;
					}
				}
				rowsId.Reverse();
				response.AddClientAction("listview", "openlevel", rowsId);
			}
		}*/

		protected override IFieldCollection<TResult, TResult> FieldsConstructor()
		{
			var enableSelect = false;
			var enableExtendedSearch = false;
			TemplateInit(_templateCollection);

			void buildTemplateDictionary(IEnumerable<TreeLevelDescriptionItem<TResult>> templateCollection)
			{
				foreach (var t in templateCollection)
				{
					enableSelect = enableSelect || t.Template.EnableSelect;
					enableExtendedSearch = enableExtendedSearch || t.EnableExtendedSearch;
					if (!_templatesDict.ContainsKey(t.Template.ID))
						_templatesDict.Add(t.Template.ID, t);
					buildTemplateDictionary(t.Children);
				}
			}
			buildTemplateDictionary(_templateCollection);

			CurrentState.TemplateItem = _templatesDict.Get(Context.GetIntArg("template", 0)) ?? _templateCollection[0];
			CurrentState.Parms = CurrentState.TemplateItem.Template.GetKeyCollection(Context);

			AfterTemplateInit();

			TreeLevelDescription<TResult> nodeTemplate = null;

			var f = new FieldCollection<TResult>(Context, Sorter, Filter);
			f.EnableSelect = enableSelect;
			f.ListAttributes += a => a.Class("tree highlight").Class("noborders").Data("highlightedid", _highlightedRowID);
			if (EnableHover)
				f.ListAttributes += a => a.Class("hover");
			if (EnableKeyboard)
			{
				f.ListAttributes += a => a.Class("kb");
				f.RowAttributes += (a, o, i) => a.TabIndex(0);
			}
			f.RowAttributes += (a, o, i) => {

				nodeTemplate = _templatesDict[o.Template].Template;
				var htmlRowID = nodeTemplate.GetHtmlRowID(CurrentState.Level, o);
				var dataRowID = nodeTemplate.GetDataRowID(CurrentState.Level, o);

				if (!CurrentState.Children.ContainsKey(htmlRowID) && !_renderSelectedBlockMode)
					a.Class("collapsed");
				else
					a.Data("loaded");

				if (nodeTemplate.IsSticky)
					a.Class("fixedrow");

				a.Data("level", CurrentState.Level);
				a.DataParm("level", CurrentState.Level);
				a.DataParm("template", o.Template);
				var coll = nodeTemplate.GetKeyCollection(o);
				foreach (var p in coll)
					a.DataParm(p.Key, p.Value);
				foreach (var d in DataCollection)
					a.Data(d.Key, d.Value);

				a.ID(htmlRowID + (_renderSelectedBlockMode ? "_selected" : ""));

				if (!_renderSelectedBlockMode && !nodeTemplate.IsTerminal && nodeTemplate.HasChildren(o))
					a.DataEvent(nodeTemplate.ToggleLevelAction ?? OnExpandRow);

				if (nodeTemplate.DataRef != null)
					foreach (var _ref in nodeTemplate.DataRef(o))
						a.DataRef("#" + _ref);

				if (nodeTemplate.EnableSelect || nodeTemplate.SetDataRowId || enableExtendedSearch)
					a.Data("rowid", dataRowID);

				if (nodeTemplate.EnableSelect && _renderSelectedBlockMode && CurrentState.IsChecked)
					a.Data("checked");
			};

			RenderRowCellDelegate<TResult> content = (w, o, i) => {
				if (nodeTemplate.EnableSelect && !_renderSelectedBlockMode)
				{
					Action<TagAttributes> attrs = a => a.Class("sel");
					if (nodeTemplate.EnableSelect.Strategy != EnableSelectOption.EnableSelectStrategy.Default)
						attrs += a => a.Data("strategy", nodeTemplate.EnableSelect.Strategy);

					w.Span(attrs, () => w.IconCheckBox());
				}

				if (nodeTemplate.IconFlag != null)
					foreach (var ic in nodeTemplate.IconFlag(o))
						w.I(a => a.Class("nodeicon").Set(ic.attributes).IconFlag(ic.iconName));

				if (nodeTemplate.Icon != null)
					nodeTemplate.Icon(o).Render(w);

				nodeTemplate.Cell(w, o, i);

				if (_renderSelectedBlockMode)
					w.Icon("delete", a => a.OnClick("listview.onRemoveIconClick(event)"));
			};

			f.AddCell("Наименование", (w, o, i) => {
				ListTreeExtensions.TreeCellContent(w, o, i, CurrentState.Level, !nodeTemplate.IsTerminal && nodeTemplate.HasChildren(o), content, new TreeCellOptions<TResult> {
					ContentAttributes = nodeTemplate.ContentAttributes
				});
			});

			FieldsInit(f);

			return f;
		}

		protected abstract void TemplateInit(List<TreeLevelDescriptionItem<TResult>> templateCollection);
		protected virtual void AfterTemplateInit() { }

		protected override void FieldsInit(FieldCollection<TResult> fields) { }

		public virtual void OnExpandRow(ApiResponse response)
		{
			OnExpandRow(response, Context.Sender);
		}

		protected void ExpandRow(ApiResponse response, string rowID)
		{
			OnExpandRow(response, $"#{rowID}");
		}

		void OnExpandRow(ApiResponse response, string sender, State state = null)
		{
			response.AddAdjacentWidget(sender, "childlevel", AdjacentHTMLPosition.AfterEnd, w => {
				if (state != null)
				{
					CurrentState = state;
					_result = null;
					(Renderer as TreeListRenderer<TResult>).SetLevel(state.Level);
				}
				Context.Sender = sender;
				Render(w);
			});

			response.ReplaceWidget(sender + "_" + Paging.ID, w => {
				Paging.Render2(w, _itemsCount, a => a.PostEvent(OnLevelSetPage).KeepTheSameUrl()
					.WithRequestMethod("listview.onlevelsetpage"), a => a.PostEvent(OnGetObjCount));
			});
		}

		public void OnLevelSetPage(ApiResponse response)
		{
			response.AddAdjacentWidget(Context.Sender, "childlevel", AdjacentHTMLPosition.AfterEnd, Render);

			response.ReplaceWidget(Context.Sender + "_" + Paging.ID, w => {
				Paging.Render2(w, _itemsCount, a => a.PostEvent(OnLevelSetPage).KeepTheSameUrl().WithRequestMethod("listview.onlevelsetpage"), a => a.PostEvent(OnGetObjCount));
			});
		}

		public override void OnEvent()
		{

		}

		protected virtual void OnAutoExpand(ApiResponse response, TResult obj) { }

		protected override void AfterRender(ApiResponse response)
		{
			base.AfterRender(response);

			void expandChildren(Dictionary<string, State> children)
			{
				foreach (var ch in children)
				{
					OnExpandRow(response, ch.Key, ch.Value);

					if (ch.Value.Children.Count > 0)
						expandChildren(ch.Value.Children);
				}
			}

			ForceFieldsInit();
			if (CurrentState.Children.Count > 0)
			{
				expandChildren(CurrentState.Children);
				response.AddClientAction("listview", "scrollToCurrentNode", ClientID);
			}
			else if (AutoExpandSingles)
			{
				var curResult = _result;
				var curCount = _count;
				var state = CurrentState;

				TResult obj = default(TResult);
				while (_count == 1 || (AutoExpandOnSearch && _count > 0 && !Context.GetArg(_qSearchParmName.Name).IsEmpty()))
				{
					obj = _result.First();
					var t = _templatesDict[obj.Template];
					if (t.Template.IsTerminal || t.Template.ToggleLevelAction != null) break;
					var sender = t.Template.GetHtmlRowID(CurrentState.Level, obj);

					var nextState = new State {
						Level = CurrentState.Level + 1,
						TemplateItem = t,
						Parms = t.Template.GetKeyCollection(obj)
					};
					CurrentState.Children.Add(sender, nextState);
					CurrentState = nextState;
					OnExpandRow(response, sender, CurrentState);
					_result = null;
					_result = GetPageData();
				}
				if (obj != null)
					OnAutoExpand(response, obj);

				_result = curResult;
				_count = curCount;
				CurrentState = state;
			}

			if (Fields.EnableSelect)
			{
				IEnumerable<TResult> getSelectedValues()
				{
					var stack = new Stack<TreeNode<(State state, TResult row)>>();

					foreach (var node in _selectedDataRoot)
						stack.Push(node);

					while (stack.Count > 0)
					{
						var current = stack.Pop();
						var row = current.Data.row;
						row.Template = current.Data.state.TemplateItem.Template.ID;
						CurrentState = current.Data.state;
						yield return row;
						foreach (var child in current.Children)
							stack.Push(child);
					}
				}

				response.SetElementClass("content", "selectable");
				response.AddAdjacentWidget("contentbody", "contenttitle_selected", AdjacentHTMLPosition.AfterEnd, w => {
					(Renderer as TreeListRenderer<TResult>).SelectedBlockTitle(w);
				});
				response.AddAdjacentWidget("contenttitle_selected", "contentbody_selected", AdjacentHTMLPosition.AfterEnd, w => {
					var values = getSelectedValues();
					var state = CurrentState;
					_renderSelectedBlockMode = true;
					(Renderer as TreeListRenderer<TResult>).SelectedBlock(w, values, Fields);
					CurrentState = state;
					_renderSelectedBlockMode = false;
				});
				//if (_selectedValues.Count > 0)
				//	response.SetElementValue("selectedvalues", _selectedValues.Join(","));
			}
		}

		public class State
		{
			public TreeLevelDescriptionItem<TResult> TemplateItem { get; set; }
			public int Level { get; set; }
			public bool IsChecked { get; set; }
			public Dictionary<string, object> Parms { get; set; }

			public Dictionary<string, State> Children { get; } = new Dictionary<string, State>();
		}
	}

	public class FlagIconInfo
	{
		public int FlagId { get; }
		public string Title { get; }

		public FlagIconInfo(int flagId, string title)
		{
			FlagId = flagId;
			Title = title;
		}

		public void Deconstruct(out int flagId, out string title)
		{
			flagId = FlagId;
			title = Title;
		}

		public static implicit operator FlagIconInfo(int flagId) => new FlagIconInfo(flagId, String.Empty);
	}

	/// <summary>
	/// шаблон для уровня дерева
	/// </summary>
	/// <typeparam name="TResult"></typeparam>
	public class TreeLevelDescription<TResult>
	{
		public int ID { get; set; }
		public string Title { get; set; }
		public Expression<Func<TResult, object>> GroupBy { get; set; }
		public Expression<Func<IGrouping<object, TResult>, object>> GroupBySelector { get; set; } = x => x.Key;
		public Func<IQueryable<TResult>, IQueryable<TResult>> OrderBy { get; set; } = data => data;
		public RenderRowCellDelegate<TResult> Cell { get; set; }
		public Func<TResult, string> TreeSearchResultHint { get; set; }
		public Func<TResult, Action<TagAttributes>> ContentAttributes { get; set; }
        public bool IsTerminal { get; set; } = false;
		public Func<TResult, bool> HasChildren { get; set; } = o => true;
		public Func<TResult, IconInfoCollection> Icon { get; set; }
		public Func<TResult, FlagIconInfoCollection> IconFlag { get; set; }
		public Func<TResult, List<string>> DataRef { get; set; }
		public Expression<Func<TResult, object>> Key { get; set; }
		public EnableSelectOption EnableSelect { get; set; }
		public bool SetDataRowId { get; set; }
		public bool AllowNulls { get; set; } = false;
		public bool IsSticky { get; set; } = false;
		public string CustomQuery { get; set; }

		public Action<ApiResponse> ToggleLevelAction { get; set; }

		List<PropertyInfo> keyProperties = null;

		List<PropertyInfo> InitKeyProperties()
		{
			var keyList = (Key.Body as NewExpression).Members.Select(x => x.Name);
			keyProperties = typeof(TResult).GetProperties().Where(p => keyList.Contains(p.Name)).ToList();
			return keyProperties;
		}

		public string GetDataRowID(int level, TResult o) => $"level={level}&template={ID}&" + (keyProperties ?? InitKeyProperties()).Select(p => p.Name + "=" + GetPropValue(p, o)).Join("&");
		public string GetHtmlRowID(int level, TResult o) => $"r_{level}_{ID}_" + (keyProperties ?? InitKeyProperties()).Select(p => GetPropValue(p, o)).Join("_");

		private string GetPropValue(PropertyInfo p, TResult o)
		{
			if (p.PropertyType == typeof(DateTime))
				return ((DateTime)p.GetValue(o)).ToString("yyyyMMddHHmmss");

			return p.GetValue(o).ToString();
		}

		public IEnumerable<string> KeyProperties => (keyProperties ?? InitKeyProperties()).Select(p => p.Name);

		public Dictionary<string, object> GetKeyCollection(TResult o)
		{
			return (keyProperties ?? InitKeyProperties()).ToDictionary(p => p.Name, p => p.GetValue(o));
		}

		public Dictionary<string, object> GetKeyCollection(ActionContext ctx)
		{
			return (Key.Body as NewExpression).Members.ToDictionary(p => p.Name, p => {
				var prop = p as PropertyInfo;
				if (prop.PropertyType == typeof(int))
					return (object)ctx.GetIntArg(p.Name);
				else if (prop.PropertyType == typeof(Guid))
					return (object)ctx.GetGuidArg(p.Name);
				else if (prop.PropertyType == typeof(DateTime))
					return (object)ctx.GetDateTimeArg(p.Name);
				return (object)ctx.GetArg(p.Name);
			});
		}
	}

	public class EnableSelectOption
	{
		public EnableSelectStrategy Strategy { get; set; } = EnableSelectStrategy.Default;

		public static implicit operator EnableSelectOption(bool enabled) =>
			new EnableSelectOption { Strategy = enabled ? EnableSelectStrategy.Default : EnableSelectStrategy.None };

		public static implicit operator bool(EnableSelectOption option) =>
			option != null && option.Strategy != EnableSelectStrategy.None;

		public enum EnableSelectStrategy
		{
			None,
			Default,
			WithChildren,
			WithChildrenRecursive
		}
	}

	public class IconInfo
	{
		public string iconName;
		public Action<TagAttributes> attributes;

		public IconInfo(string iconName, Action<TagAttributes> attributes = null)
		{
			this.iconName = iconName;
			this.attributes = attributes;
		}

		public static implicit operator Action<LayoutWriter>(IconInfo ic) => w => w.Icon(ic.iconName.Trim(), a => a.Class("nodeicon").Set(ic.attributes));
	}

	public class FlagIconInfoCollection : List<IconInfo>
	{
		public FlagIconInfoCollection()
		{
		}

		public FlagIconInfoCollection(IEnumerable<IconInfo> collection) : base(collection)
		{
		}

		public static implicit operator FlagIconInfoCollection(string iconName) =>
			new FlagIconInfoCollection(iconName.Split(",").Select(i => new IconInfo(i.Trim())));
	}

	public class IconInfoCollection : List<Action<LayoutWriter>>
	{
		public void Render(LayoutWriter w) => this.ForEach(ic => ic(w));

		public static implicit operator IconInfoCollection(string iconName)
		{
			return new IconInfoCollection
			{
				new IconInfo(iconName)
			};
		}
	}

	/// <summary>
	/// экземпляр шаблона TreeLevelDescription
	/// </summary>
	/// <typeparam name="TResult"></typeparam>
	public class TreeLevelDescriptionItem<TResult>
	{
		public TreeLevelDescription<TResult> Template { get; set; }

		public bool EnableExtendedSearch { get; set; } = false;
		public Expression<Func<TResult, bool>> Where { get; set; }

		public TreeLevelDescriptionItem<TResult> ParentTemplateItem { get; set; }
		List<TreeLevelDescriptionItem<TResult>> _children = new List<TreeLevelDescriptionItem<TResult>>();
		public IReadOnlyList<TreeLevelDescriptionItem<TResult>> Children => _children;

		public TreeLevelDescriptionItem<TResult> AddChild(TreeLevelDescription<TResult> template, Expression<Func<TResult, bool>> where = null)
		{
			var newItem = new TreeLevelDescriptionItem<TResult> { Template = template, Where = where, ParentTemplateItem = this };
			_children.Add(newItem);
			return newItem;
		}

		public static implicit operator TreeLevelDescriptionItem<TResult>(TreeLevelDescription<TResult> template)
		{
			return new TreeLevelDescriptionItem<TResult> { Template = template };
		}
	}

	public class ObjectSetSettings
	{
		public string SystemName { get; set; } = null;
		public string TableName { get; set; } = null;
	}
}
