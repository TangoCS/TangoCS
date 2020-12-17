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

		int _level;

		int _count = 0;

		protected override bool EnableViews => false;

		public override ViewContainer GetContainer() => new SelectableTreeContainer {
			EnableSelect = Fields.EnableSelect,
			SelectedBlock = w => (Renderer as TreeListRenderer<TResult>).SelectedBlock(w, Fields)
		};

		public override void OnInit()
		{
			base.OnInit();

			_level = Context.GetIntArg("level", -1);
			_level++;

			Renderer = new TreeListRenderer<TResult>(ID, Paging, _level);
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
			if (_pageData != null)
				return _pageData;

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

			var curTemplate = _templatesDict.Get(Context.GetIntArg("template", 0)) ?? _templateCollection[0];
			var curParms = curTemplate.GetKeyCollection(Context);

			var where = new List<string>();

			if (curParms != null)
			{
				foreach (var pair in curParms)
				{
					if (pair.Value == null) continue;
					where.Add($"{pair.Key} = @lf_{pair.Key}");
					Repository.Parameters.Add("lf_" + pair.Key, pair.Value);
				}
			}

			var dialect = QueryHelper.CreateDialect(Database.GetDBType());
			var q = Sorter.Count > 0 ? Sorter.Apply(filtered) : DefaultOrderBy(filtered);

			var nodeTemplates = _level == 0 ?
				new List<ChildTreeLevelDescription<TResult>> { new ChildTreeLevelDescription<TResult> { Child = curTemplate } } :
				curTemplate.Children;

			var origAllObjectsQuery = Repository.AllObjectsQuery;

			using (var tran = Database.BeginTransaction())
			{
				BeforeGetPageData(tran);

				foreach (var t in nodeTemplates)
				{
					var nodeQuery = Paging.Apply(q, true);
					var nodeQueryCnt = q;

					var nodeWhere = new List<string>(where);

					if (t.Where != null)
					{
						nodeQuery = nodeQuery.Where(t.Where);
						nodeQueryCnt = nodeQueryCnt.Where(t.Where);
					}

					nodeQuery = t.Child.OrderBy(nodeQuery);

					var expr = t.Child.GroupBy != null ? nodeQuery.GroupBy(t.Child.GroupBy).Select(t.Child.GroupBySelector).Expression : nodeQuery.Expression;
					var exprCnt = t.Child.GroupBy != null ? nodeQueryCnt.GroupBy(t.Child.GroupBy).Select(x => x.Key).Expression : nodeQueryCnt.Expression;

					var sqlTemplate = "select *";
					sqlTemplate += $" from ({origAllObjectsQuery}) t";

					if (!t.Child.AllowNulls)
						foreach (var p in t.Child.KeyProperties)
							nodeWhere.Add($"{p} is not null");

					if (nodeWhere.Count > 0)
						sqlTemplate += " where " + nodeWhere.Join(" and ");

					Repository.AllObjectsQuery = sqlTemplate;
					var res = Repository.List(expr);
					var resCnt = Repository.Count(exprCnt);

					foreach (var o in res)
						o.Template = t.Child.ID;


					_pageData = _pageData == null ? res : _pageData.Concat(res);
					_count += resCnt;
				}
			}


			return _pageData;
		}

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

					var temp = Database.Connection.QueryFirstOrDefault<TResult>(sqlTemplate, transaction: tran);
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
					buildTemplateDictionary(t.Children.Select(x => x.Child));
				}
			}
			buildTemplateDictionary(_templateCollection);

			TreeLevelDescription<TResult> nodeTemplate = null;

			var f = new FieldCollection<TResult>(Context, Sorter, Filter);
			f.EnableSelect = enableSelect;
			f.ListAttributes += a => a.Class("tree");
			f.RowAttributes += (a, o, i) => {
				a.Class("collapsed");
				a.Data("level", _level);
				a.DataParm("level", _level);

				nodeTemplate = _templatesDict[o.Template];

				a.DataParm("template", o.Template);
				var coll = nodeTemplate.GetKeyCollection(o);
				foreach (var p in coll)
					a.DataParm(p.Key, p.Value);
				a.ID(nodeTemplate.GetHtmlRowID(_level, o));
				a.DataEvent(OnExpandRow);

				if (nodeTemplate.DataRef != null)
					foreach (var _ref in nodeTemplate.DataRef(o))
						a.DataRef("#"+_ref);

				if (nodeTemplate.EnableSelect || nodeTemplate.SetDataRowId)
					a.Data("rowid", nodeTemplate.GetDataRowID(_level, o));
			};

			void content(LayoutWriter w, TResult o)
			{
				if (nodeTemplate.EnableSelect)
					w.Span(a => a.Class("sel"), () => w.Icon("checkbox-unchecked"));

				if (nodeTemplate.IconFlag != null)
					foreach (var ic in nodeTemplate.IconFlag(o).Split(','))
						w.I(a => a.Class("nodeicon").IconFlag(ic.Trim()));

				if (nodeTemplate.Icon != null)
					foreach (var ic in nodeTemplate.Icon(o).Split(','))
						w.I(a => a.Class("nodeicon").Icon(ic.Trim()));				

				nodeTemplate.Cell(w, o);
			}

			f.AddCell("Наименование", (w, o) => {
				ListTreeExtensions.TreeCellContent(w, o, _level, !nodeTemplate.IsTerminal, content);
			});

			FieldsInit(f);

			return f;
		}

		protected abstract void TemplateInit(List<TreeLevelDescription<TResult>> templateColletion);

		protected override void FieldsInit(FieldCollection<TResult> fields) { }

		public void OnExpandRow(ApiResponse response)
		{
			response.AddAdjacentWidget(Context.Sender, "childlevel", AdjacentHTMLPosition.AfterEnd, Render);

			response.ReplaceWidget(Context.Sender + "_" + Paging.ID, w => {
				Paging.Render2(w, _itemsCount, a => a.PostEvent(OnLevelSetPage).KeepTheSameUrl()
					.WithRequestMethod("listview.onlevelsetpage"), a => a.PostEvent(GetObjCount));
			});
		}

		public void OnLevelSetPage(ApiResponse response)
		{
			//var root = "#" + Context.GetArg("root");

			response.AddAdjacentWidget(Context.Sender, "childlevel", AdjacentHTMLPosition.AfterEnd, Render);

			response.ReplaceWidget(Context.Sender + "_" + Paging.ID, w => {
				Paging.Render2(w, _itemsCount, a => a.PostEvent(OnLevelSetPage).KeepTheSameUrl().WithRequestMethod("listview.onlevelsetpage"), a => a.PostEvent(GetObjCount));
			});
		}
	}

	public class TreeLevelDescription<TResult>
	{
		public int ID { get; set; }
		public Expression<Func<TResult, object>> GroupBy { get; set; }
		public Expression<Func<IGrouping<object, TResult>, object>> GroupBySelector { get; set; } = x => x.Key;
		public Func<IQueryable<TResult>, IQueryable<TResult>> OrderBy { get; set; } = data => data;
		public Action<LayoutWriter, TResult> Cell { get; set; }
		public bool IsTerminal { get; set; } = false;
		//public string Icon { get; set; }
		public Func<TResult, string> Icon { get; set; }
		public Func<TResult, string> IconFlag { get; set; }
		public Func<TResult, List<string>> DataRef { get; set; }
		public Expression<Func<TResult, object>> Key { get; set; }
		public bool EnableSelect { get; set; }
		public bool SetDataRowId { get; set; }
		public TreeLevelDescription<TResult> ParentTemplate { get; set; }
		public bool AllowNulls { get; set; } = false;

		List<ChildTreeLevelDescription<TResult>> _children = new List<ChildTreeLevelDescription<TResult>>();

		public IReadOnlyList<ChildTreeLevelDescription<TResult>> Children => _children;

		List<PropertyInfo> keyProperties = null;

		List<PropertyInfo> InitKeyProperties()
		{
			var keyList = (Key.Body as NewExpression).Members.Select(x => x.Name);
			keyProperties = typeof(TResult).GetProperties().Where(p => keyList.Contains(p.Name)).ToList();
			return keyProperties;
		}

		public string GetDataRowID(int level, TResult o) => $"level={level}&" + (keyProperties ?? InitKeyProperties()).Select(p => p.Name + "=" + p.GetValue(o).ToString()).Join("&");
		public string GetHtmlRowID(int level, TResult o) => $"r_{level}_" + (keyProperties ?? InitKeyProperties()).Select(p => p.GetValue(o).ToString()).Join("_");

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

			_children.Add(new ChildTreeLevelDescription<TResult> { Child = template, Where = where });
		}
	}

	public class ChildTreeLevelDescription<TResult>
	{
		public TreeLevelDescription<TResult> Child { get; set; }
		public Expression<Func<TResult, bool>> Where { get; set; }

	}

	public class SelectableTreeContainer : ViewContainer
	{
		public bool EnableSelect { get; set; }
		public Action<LayoutWriter> SelectedBlock { get; set; }


		public override void Render(ApiResponse response)
		{
			response.AddWidget("container", w => {
				w.Div(a => a.ID("content").Class("content").DataContainer(Type, w.IDPrefix), () => {
					if (!ToRemove.Contains("contentheader"))
						w.ContentHeader();
					w.Div(a => a.ID("contenttoolbar"));
					if (EnableSelect)
					{
						w.Div(a => a.ID("contentbody").Class("contentbody").Style("flex:7;overflow-y:auto;"));
						w.GroupTitle("Выбранные объекты");
						w.Div(a => a.Style("flex:3;overflow-y:auto;"), () => SelectedBlock(w));
					}
					else
					{
						w.Div(a => a.ID("contentbody").Class("contentbody"));
					}

				});
			});
		}
	}
}
