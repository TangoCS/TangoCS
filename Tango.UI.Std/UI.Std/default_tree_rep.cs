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

			var sqlTemplate = $"select * from ({Repository.AllObjectsQuery}) t";

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
									
					var expr = t.Child.GroupBy != null ? nodeQuery.GroupBy(t.Child.GroupBy).Select(x => x.Key).Expression : nodeQuery.Expression;
					var exprCnt = t.Child.GroupBy != null ? nodeQueryCnt.GroupBy(t.Child.GroupBy).Select(x => x.Key).Expression : nodeQueryCnt.Expression;
					

					Repository.AllObjectsQuery = sqlTemplate;

					foreach (var p in t.Child.KeyProperties)
						nodeWhere.Add($"{p} is not null");

					if (nodeWhere.Count > 0)
						Repository.AllObjectsQuery += " where " + nodeWhere.Join(" and ");

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
				a.ID("r_" + Guid.NewGuid().ToString());
				a.DataEvent(OnExpandRow);
				if (nodeTemplate.EnableSelect)
					a.Data("rowid", nodeTemplate.GetRowID(_level, o));
			};

			void content(LayoutWriter w, TResult o)
			{
				if (nodeTemplate.EnableSelect)
					w.Span(a => a.Class("sel"), () => w.Icon("checkbox-unchecked"));

				if (nodeTemplate.Icon != null)
					w.I(a => a.Class("nodeicon").Icon(nodeTemplate.Icon(o)));

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
		public Func<IQueryable<TResult>, IQueryable<TResult>> OrderBy { get; set; } = data => data;
		public Action<LayoutWriter, TResult> Cell { get; set; }
		public bool IsTerminal { get; set; } = false;
		//public string Icon { get; set; }
		public Func<TResult, string> Icon { get; set; }
		public Expression<Func<TResult, object>> Key { get; set; }
		public bool EnableSelect { get; set; }

		List<ChildTreeLevelDescription<TResult>> _children = new List<ChildTreeLevelDescription<TResult>>();

		public IReadOnlyList<ChildTreeLevelDescription<TResult>> Children => _children;

		List<PropertyInfo> keyProperties = null;

		List<PropertyInfo> InitKeyProperties()
		{
			var keyList = (Key.Body as NewExpression).Members.Select(x => x.Name);
			keyProperties = typeof(TResult).GetProperties().Where(p => keyList.Contains(p.Name)).ToList();
			return keyProperties;
		}

		public string GetRowID(int level, TResult o) => $"level={level}&" + (keyProperties ?? InitKeyProperties()).Select(p => p.Name + "=" + p.GetValue(o).ToString()).Join("&");

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
			_children.Add(new ChildTreeLevelDescription<TResult> { Child = template, Where = where });
		}
	}

	public class ChildTreeLevelDescription<TResult>
	{
		public TreeLevelDescription<TResult> Child { get; set; }
		public Expression<Func<TResult, bool>> Where { get; set; }

	}

}
