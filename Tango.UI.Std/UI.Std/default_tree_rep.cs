using System;
using System.Linq.Expressions;
using Tango;
using Tango.UI;
using Tango.UI.Controls;
using Tango.UI.Std;
using Tango.Html;
using System.Linq;
using System.Collections.Generic;
using Tango.Data;
using Dapper;
using System.Reflection;
using System.Data;

namespace Tango.UI.Std
{
	public abstract class default_tree_rep<TResult> : default_list_rep<TResult>
		where TResult : ILazyListTree
	{
		readonly List<TreeLevelDescription<TResult>> _templateCollection = new List<TreeLevelDescription<TResult>>();
		Dictionary<int, TreeLevelDescription<TResult>> _templatesDict = new Dictionary<int, TreeLevelDescription<TResult>>();

		int _count = 0;

		string _highlightedRowID = null;

		bool _renderSelectedBlockMode = false;

		State InitialState = new State { };
		State CurrentState = null;
		

		protected override bool EnableViews => false;

		public bool AutoExpandSingles { get; set; } = true;

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


		protected override int GetCount()
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
				new List<TreeLevelDescriptionItem<TResult>> { new TreeLevelDescriptionItem<TResult> { Template = CurrentState.Template } } :
				CurrentState.Template.Children;

			var origAllObjectsQuery = Repository.AllObjectsQuery;

			using (var tran = Database.BeginTransaction())
			{
				BeforeGetPageData(tran);

				foreach (var t in nodeTemplates)
				{
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
						origAllObjectsQuery = t.Template.CustomQuery;

					var sqlTemplate = "select *";
					sqlTemplate += $" from ({origAllObjectsQuery}) t";

					if (!t.Template.AllowNulls)
						foreach (var p in t.Template.KeyProperties)
							nodeWhere.Add($"{p} is not null");

					if (nodeWhere.Count > 0)
						sqlTemplate += " where " + nodeWhere.Join(" and ");

					Repository.AllObjectsQuery = sqlTemplate;
					var res = Repository.List(expr);
					var resCnt = Repository.Count(exprCnt);

					foreach (var o in res)
						o.Template = t.Template.ID;


					pageData = pageData == null ? res : pageData.Concat(res);
					_count += resCnt;
				}
			}

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
			var initialTemplate = _templatesDict[templateID];
			var template = initialTemplate.ParentTemplate;
			var levelForExpandNext = level;

			var sqlTemplate = PrepareQuery(template, new List<Dictionary<string, object>> { parms });

			using (var tran = Database.BeginTransaction())
			{
				BeforeGetPageData(tran);

				var temp = Database.Connection.QueryFirstOrDefault<TResult>(sqlTemplate, Repository.Parameters, tran);
				if (temp == null) return;

				var states = new List<State>();
				var senders = new List<string>();

				level--;

				if (highlight)
					_highlightedRowID = GetClientID(initialTemplate.GetHtmlRowID(level, temp));

				while (template != null)
				{
					states.Add(new State
					{
						Level = level,
						Template = template,
						Parms = template.GetKeyCollection(temp)
					});
					senders.Add(template.GetHtmlRowID(level - 1, temp));

					template = template.ParentTemplate;
					level--;
				}

				if (expandNext)
				{
					
					states.Insert(0,new State
					{
						Level = levelForExpandNext,
						Template = initialTemplate,
						Parms = initialTemplate.GetKeyCollection(temp)
					});
					
					senders.Insert(0,(initialTemplate.GetHtmlRowID(levelForExpandNext - 1, temp)));
				}

				senders.Reverse();
				states.Reverse();

				var s = CurrentState;
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
			var expr = Enumerable.Empty<TResult>().AsQueryable().Where(predicate);

			var data = Repository.List(expr.Expression);

			var template = _templatesDict[templateID];
			foreach (var row in data)
			{
				var t = template;
				var lev = level;

				var key = t.GetDataRowID(lev, row);
				_selectedValues.Add(key);

				while (t.ParentTemplate != null)
				{
					var parentKey = t.ParentTemplate.GetHtmlRowID(lev - 1, row);
					var rowState = new State { Level = lev, Template = t };
					var parentState = new State { Level = lev - 1, Template = t.ParentTemplate };

					if (_selectedDataRows.TryGetValue(parentKey, out var parent))
					{
						parent.AddChild((rowState, row));
						break;
					}
					else
					{
						parent = new TreeNode<(State state, TResult row)>((parentState, row));
						parent.AddChild((rowState, row));
						_selectedDataRows.Add(parentKey, parent);
						if (lev - 1 == 0)
							_selectedDataRoot.Add(parent);
					}
					t = t.ParentTemplate;
					lev--;
				}
			}

			(Renderer as TreeListRenderer<TResult>).SetSelectedValues(_selectedValues);
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
				var sql = PrepareQuery(t, d.Value);
				var objs = Database.Connection.Query<TResult>(sql, Repository.Parameters);
				res.AddRange(objs);
			}

			return res;
		}
		
		[Obsolete]
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
						var row = template.GetDataRowID(id, temp);
						if (!template.IsTerminal)
							rowsId.Add(row);

						id--;
						template = template.ParentTemplate;
					}
				}
				rowsId.Reverse();
				response.AddClientAction("listview", "openlevel", rowsId);
			}
		}

		protected override IFieldCollection<TResult, TResult> FieldsConstructor()
		{
			var enableSelect = false;
			TemplateInit(_templateCollection);

			void buildTemplateDictionary(IEnumerable<TreeLevelDescription<TResult>> templateCollection)
			{
				foreach (var t in templateCollection)
				{
					enableSelect = enableSelect || t.EnableSelect;
					if (!_templatesDict.ContainsKey(t.ID))
						_templatesDict.Add(t.ID, t);
					buildTemplateDictionary(t.Children.Select(x => x.Template));
				}
			}
			buildTemplateDictionary(_templateCollection);

			CurrentState.Template = _templatesDict.Get(Context.GetIntArg("template", 0)) ?? _templateCollection[0];
			CurrentState.Parms = CurrentState.Template.GetKeyCollection(Context);

			AfterTemplateInit();

			TreeLevelDescription<TResult> nodeTemplate = null;

			var f = new FieldCollection<TResult>(Context, Sorter, Filter);
			f.EnableSelect = enableSelect;
			f.ListAttributes += a => a.Class("tree").Data("highlightedid", _highlightedRowID);
			f.RowAttributes += (a, o, i) => {

				nodeTemplate = _templatesDict[o.Template];
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
				a.TabIndex(0);
				a.DataParm("template", o.Template);
				var coll = nodeTemplate.GetKeyCollection(o);
				foreach (var p in coll)
					a.DataParm(p.Key, p.Value);
				foreach (var d in DataCollection)
					a.Data(d.Key, d.Value);

				a.ID(htmlRowID + (_renderSelectedBlockMode ? "_selected" : ""));

				if (!_renderSelectedBlockMode)
					a.DataEvent(nodeTemplate.ToggleLevelAction ?? OnExpandRow);

				if (nodeTemplate.DataRef != null)
					foreach (var _ref in nodeTemplate.DataRef(o))
						a.DataRef("#"+_ref);

				if (nodeTemplate.EnableSelect || nodeTemplate.SetDataRowId)
					a.Data("rowid", dataRowID);

				if (nodeTemplate.EnableSelect && _renderSelectedBlockMode)
					a.Data("checked");
			};

			RenderRowCellDelegate<TResult> content = (w, o, i) => {
				if (nodeTemplate.EnableSelect && !_renderSelectedBlockMode)
					w.Span(a => a.Class("sel"), () => w.Icon("checkbox-unchecked"));

				if (nodeTemplate.IconFlag != null)
					foreach (var ic in nodeTemplate.IconFlag(o).Split(','))
						w.I(a => a.Class("nodeicon").IconFlag(ic.Trim()));

				if (nodeTemplate.Icon != null)
					foreach (var ic in nodeTemplate.Icon(o))
						w.I(a => a.Class("nodeicon").Icon(ic.iconName.Trim()).Set(ic.attributes));

				nodeTemplate.Cell(w, o, i);

				if (_renderSelectedBlockMode)
					w.I(a => a.Icon("delete").OnClick("listview.onRemoveIconClick(event)"));
			};

			f.AddCell("Наименование", (w, o, i) => {
				ListTreeExtensions.TreeCellContent(w, o, i, CurrentState.Level, !nodeTemplate.IsTerminal, content);
			});

			FieldsInit(f);

			return f;
		}

		protected abstract void TemplateInit(List<TreeLevelDescription<TResult>> templateCollection);
		protected virtual void AfterTemplateInit() { }

		protected override void FieldsInit(FieldCollection<TResult> fields) { }

		public void OnExpandRow(ApiResponse response)
		{
			OnExpandRow(response, Context.Sender);
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
					.WithRequestMethod("listview.onlevelsetpage"), a => a.PostEvent(GetObjCount));
			});
		}

		public void OnLevelSetPage(ApiResponse response)
		{
			response.AddAdjacentWidget(Context.Sender, "childlevel", AdjacentHTMLPosition.AfterEnd, Render);

			response.ReplaceWidget(Context.Sender + "_" + Paging.ID, w => {
				Paging.Render2(w, _itemsCount, a => a.PostEvent(OnLevelSetPage).KeepTheSameUrl().WithRequestMethod("listview.onlevelsetpage"), a => a.PostEvent(GetObjCount));
			});
		}

		public override void OnEvent()
		{
			
		}

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
				expandChildren(CurrentState.Children);
			else if (AutoExpandSingles)
			{
				var curResult = _result;
				var curCount = _count;
				var state = CurrentState;

				while (_count == 1)
				{
					var obj = _result.First();
					var t = _templatesDict[obj.Template];
					if (t.IsTerminal) break;
					var sender = t.GetHtmlRowID(CurrentState.Level, obj);

					var nextState = new State
					{
						Level = CurrentState.Level + 1,
						Template = t,
						Parms = t.GetKeyCollection(obj)
					};
					CurrentState.Children.Add(sender, nextState);
					CurrentState = nextState;
					OnExpandRow(response, sender, CurrentState);
					_result = null;
					_result = GetPageData();
				}

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
						row.Template = current.Data.state.Template.ID;
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
			public TreeLevelDescription<TResult> Template { get; set; }
			public int Level { get; set; }
			public Dictionary<string, object> Parms { get; set; }

			public Dictionary<string, State> Children { get; } = new Dictionary<string, State>();
		}
	}

	public class TreeLevelDescription<TResult>
	{
		public int ID { get; set; }
		public Expression<Func<TResult, object>> GroupBy { get; set; }
		public Expression<Func<IGrouping<object, TResult>, object>> GroupBySelector { get; set; } = x => x.Key;
		public Func<IQueryable<TResult>, IQueryable<TResult>> OrderBy { get; set; } = data => data;
		public RenderRowCellDelegate<TResult> Cell { get; set; }
		public bool IsTerminal { get; set; } = false;
		//public string Icon { get; set; }
		public Func<TResult, IconInfoCollection> Icon { get; set; }
		public Func<TResult, string> IconFlag { get; set; }
		public Func<TResult, List<string>> DataRef { get; set; }
		public Expression<Func<TResult, object>> Key { get; set; }
		public bool EnableSelect { get; set; }
		public bool SetDataRowId { get; set; }
		public TreeLevelDescription<TResult> ParentTemplate { get; set; }
		public bool AllowNulls { get; set; } = false;
		public bool IsSticky { get; set; } = false;
		public string CustomQuery { get; set; }

		public Action<ApiResponse> ToggleLevelAction { get; set; }

		List<TreeLevelDescriptionItem<TResult>> _children = new List<TreeLevelDescriptionItem<TResult>>();

		public IReadOnlyList<TreeLevelDescriptionItem<TResult>> Children => _children;

		List<PropertyInfo> keyProperties = null;

		List<PropertyInfo> InitKeyProperties()
		{
			var keyList = (Key.Body as NewExpression).Members.Select(x => x.Name);
			keyProperties = typeof(TResult).GetProperties().Where(p => keyList.Contains(p.Name)).ToList();
			return keyProperties;
		}

		public string GetDataRowID(int level, TResult o) => $"level={level}&template={ID}&" + (keyProperties ?? InitKeyProperties()).Select(p => p.Name + "=" + p.GetValue(o).ToString()).Join("&");
		public string GetHtmlRowID(int level, TResult o) => $"r_{level}_{ID}_" + (keyProperties ?? InitKeyProperties()).Select(p => p.GetValue(o).ToString()).Join("_");

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
				return (object)ctx.GetArg(p.Name);
			});
		}

		public void AddChild(TreeLevelDescription<TResult> template, Expression<Func<TResult, bool>> where = null)
		{
			template.ParentTemplate = this;

			_children.Add(new TreeLevelDescriptionItem<TResult> { Template = template, Where = where });
		}
	}

	public class IconInfo
	{
		public string iconName;
		public Action<TagAttributes> attributes;

		public IconInfo(string iconName,Action<TagAttributes> attributes = null)
		{
			this.iconName = iconName;
			this.attributes = attributes;
		}
	}

	public class IconInfoCollection : List<IconInfo>
	{
		public static implicit operator IconInfoCollection(string iconName)
		{
			return new IconInfoCollection
			{
				new IconInfo(iconName)
			};
		}
	}
	public class TreeLevelDescriptionItem<TResult>
	{
		public TreeLevelDescription<TResult> Template { get; set; }
		public Expression<Func<TResult, bool>> Where { get; set; }
	}
}
