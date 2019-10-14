using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tango.AccessControl;
using Tango.Data;
using Tango.Html;
using Tango.Localization;
using Tango.UI.Controls;

namespace Tango.UI.Std
{
	public abstract class abstract_list<TEntity, TResult> : ViewPagePart
	{
		[Inject]
		protected IAccessControl AccessControl { get; set; }
		[Inject]
		protected ITypeActivatorCache TypeActivatorCache { get; set; }


		protected string _qSearch = "";
		protected IFieldCollection<TEntity, TResult> _fields;
		//protected Action<ActionLink> _pagingAttributes => a => a.RunEvent(OnSetPage);

		protected IEnumerable<TResult> _result;
		protected int? _itemsCount;

		protected abstract int GetCount();
		protected abstract IEnumerable<TResult> GetPageData();
		protected abstract IFieldCollection<TEntity, TResult> FieldsConstructor();

		public ListFilter<TEntity> Filter { get; private set; }
		public Paging Paging { get; private set; }
		public Sorter<TEntity> Sorter { get; private set; }
		public ListRendererAbstract<TResult> Renderer { get; protected set; }

		public int ColumnCount => _fields.Cells.Count;

		protected virtual string FormTitle => Resources.CaptionPlural<TEntity>();
		protected virtual Func<string, Expression<Func<TEntity, bool>>> SearchExpression => null;

		protected virtual bool EnableViews => true;
		protected virtual bool EnableQuickSearch => true;

		protected virtual void Toolbar(LayoutWriter w)
		{
			PrepareResult();
			//w.PushPrefix(ID);
			w.Toolbar(t => ToolbarLeft(t), t => {	
				t.Item(tw => tw.Span(a => a.ID(Paging.ID)));
				t.ItemSeparator();
				ToolbarRight(t);
			});
			//w.PopPrefix();
		}

		protected void ToCreateNew<T>(MenuBuilder t, Action<ActionLink> attrs = null)
		{
			t.ItemSeparator();
			var key = typeof(T).GetResourceType().Name + "." + Constants.OpView;
			if (TypeActivatorCache.Get(key).HasValue)
			{
				var retUrl1 = new ActionLink(Context).To<T>(Constants.OpView).WithArg(Constants.Id, "@ID").Url;
				var retUrl0 = Context.CreateReturnUrl(1);
				t.ItemActionImageText(x => x.ToCreateNew<T>(AccessControl, null, retUrl1)
					.WithArg(Constants.ReturnUrl + "_0", retUrl0)
					.Set(attrs));
			}
			else
				t.ItemActionImageText(x => x.ToCreateNew<T>(AccessControl).Set(attrs));
		}

		protected void ToDeleteBulk(MenuBuilder t)
		{
			if (_fields.EnableSelect)
			{
				t.ItemSeparator();
				t.ItemActionTextBulk(x => x.ToDeleteBulk<TEntity>(AccessControl).AsDialog());
			}
		}

		protected virtual void ToolbarLeft(MenuBuilder t)
		{
			t.ItemFilter(Filter);
			ToCreateNew<TEntity>(t);
			ToDeleteBulk(t);
		}

		protected virtual void ToolbarRight(MenuBuilder t)
		{
			if (EnableQuickSearch)
				t.QuickSearch(this, Paging);
			if (EnableViews && Filter.FieldList.Count > 0)
			{
				t.ItemSeparator();
				t.ItemViews(Filter);
			}
		}


		protected virtual void FilterInit(ListFilter<TEntity> f) { }

		public override void OnInit()
		{
			if (ID.IsEmpty()) ID = GetType().Name;

			Renderer = new ListRenderer<TResult>(ID);

			Paging = CreateControl<Paging>("page", p => {
				p.PageIndex = Context.GetIntArg(p.ClientID, 1);
			});
			Sorter = CreateControl<Sorter<TEntity>>("sort", s => {
				s.OnSort = OnSetPage;
			});
			Filter = CreateControl<ListFilter<TEntity>>("filter", f => {
				FilterInit(f);
				f.FilterSubmitted += OnFilter;
			});

			_qSearch = Context.GetArg("qsearch");
			_fields = FieldsConstructor();
		}

		public void PrepareResult()
		{
			if (_result != null) return; 

			_result = GetPageData();

			var i = _result.Count();

			_itemsCount = null;
			if (i <= Paging.PageSize && Paging.PageIndex <= 1)
				_itemsCount = i;
			if (Paging.PageIndex > 0 && _itemsCount == null)
				_itemsCount = GetCount();
		}

		public virtual void BeforeList(LayoutWriter w) { }
		public virtual void AfterList(LayoutWriter w) { }

		public virtual void Render(LayoutWriter w)
		{
			//w.PushPrefix(ClientID);
			PrepareResult();
			BeforeList(w);
			Renderer.Render(w, _result.Take(Paging.PageSize), _fields);
			AfterList(w);
			//w.PopPrefix();
		}

		public virtual void RenderPlaceHolder(LayoutWriter w)
		{
			w.PushPrefix(ID);
			w.Div(a => a.DataContainer("default", w.IDPrefix), () => {
				w.Div(a => a.ID(Sections.ContentTitle));
				w.Div(a => a.ID(Sections.ContentToolbar));
				w.Div(a => a.ID(Sections.ContentBody));
			});
			w.PopPrefix();
		}

		protected void RenderPaging(ApiResponse response)
		{
			if (Sections.RenderPaging)
				response.ReplaceWidget(Paging.ID, w => Paging.Render2(w, _itemsCount, OnSetPage, GetObjCount));
		}

		protected void RenderToolbar(ApiResponse response)
		{
			if (Sections.RenderToolbar)
				response.ReplaceWidget(Sections.ContentToolbar, Toolbar);
		}

		private void OnFilter(ApiResponse response)
		{
			if (Filter.Criteria.Count > 0)
			{
				if (Context.GetIntArg(Filter.ParameterName) == Filter.PersistentFilter.ID)
				{
					Paging.PageIndex = 1;
					response.AddWidget(Sections.ContentBody, Render);
					RenderToolbar(response);
					RenderPaging(response);
				}
				else
					response.RedirectTo(Context, a => a.ToReturnUrl(1)
						.WithArg(Filter.ParameterName, Filter.PersistentFilter.ID)
						.RemoveArg(Paging.ParameterName));
			}
			else
				response.RedirectTo(Context, a => a.ToReturnUrl(1)
						.RemoveArg(Filter.ParameterName)
						.RemoveArg(Paging.ParameterName));
		}

		public override void OnLoad(ApiResponse response)
		{
			if (Sections.RenderListOnLoad || !_qSearch.IsEmpty())
				response.AddWidget(Sections.ContentBody, Render);
			RenderToolbar(response);
			if (Sections.RenderContentTitle)
				response.AddWidget(Sections.ContentTitle, FormTitle);
			if (Sections.SetPageTitle)
				response.AddWidget("#title", FormTitle);
			RenderPaging(response);
		}

		public void OnQuickSearch(ApiResponse response)
		{
			response.AddWidget(Sections.ContentBody, Render);
			RenderPaging(response);
		}

		public void OnSetPage(ApiResponse response)
		{
			response.AddWidget(Sections.ContentBody, Render);
			RenderPaging(response);
		}

		public void GetObjCount(ApiResponse response)
		{
			_itemsCount = GetCount();
			RenderPaging(response);
		}

		protected IQueryable<TEntity> ApplyFilter(IQueryable<TEntity> q)
		{
			if (SearchExpression != null)
			{
				if (!_qSearch.IsEmpty()) q = q.Where(SearchExpression(_qSearch));
			}

			return Filter.ApplyFilter(q);
		}

		public ListSections Sections { get; set; } = new ListSections();
		public class ListSections
		{
			public string ContentBody { get; set; } = "contentbody";
			public string ContentToolbar { get; set; } = "contenttoolbar";
			public string ContentTitle { get; set; } = "contenttitle";
			public bool SetPageTitle { get; set; } = true;
			public bool RenderToolbar { get; set; } = true;
			public bool RenderContentTitle { get; set; } = true;
			public bool RenderPaging { get; set; } = true;
			public bool RenderListOnLoad { get; set; } = true;			
		}
	}


	public abstract class default_list<TEntity, TResult> : abstract_list<TEntity, TResult>
	{
		[Inject]
		protected IDataContext DataContext { get; set; }

		protected virtual IQueryable<TEntity> Data => DataContext.GetTable<TEntity>().Filtered();
		protected abstract IQueryable<TResult> Selector(IQueryable<TEntity> data);

		protected override int GetCount()
		{
			return ApplyFilter(Data).Count();
		}

		protected override IEnumerable<TResult> GetPageData()
		{
			_fields.GroupSorting.Reverse();
			foreach (var gs in _fields.GroupSorting)
				Sorter.InsertOrderBy(gs.SeqNo, gs.SortDesc, true);

			var res = Selector(Paging.Apply(Sorter.Apply(ApplyFilter(Data)), true));
			return res.ToList();
			//var expr = SelectExpression();
			//return expr != null ? res.Select(SelectExpression()).ToList() : res.Cast<TResult>().ToList();
		}

		protected override IFieldCollection<TEntity, TResult> FieldsConstructor()
		{
			var f = new FieldCollection<TEntity, TResult>(Context, Sorter, Filter);
			f.RowAttributes += (a, o, i) => a.ZebraStripping(i.RowNum);
			FieldsInit(f);
			return f;
		}

		protected abstract void FieldsInit(FieldCollection<TEntity, TResult> fields);
		//protected abstract Expression<Func<TEntity, TResult>> SelectExpression();
	}

	public abstract class default_list<T> : default_list<T, T>
	{
		//protected override Expression<Func<T, T>> SelectExpression() => o => o;
		protected override IQueryable<T> Selector(IQueryable<T> data) => data;
	}

	public class ListGroup<TResult>
	{
		public Func<TResult, string> ValueFunc;
		public RenderGroupCellDelegate<TResult> Cell;
		public string DefaultValue { get; set; }
		public Dictionary<int, Action<LayoutWriter,TResult>> Cells { get; private set; } = new Dictionary<int, Action<LayoutWriter, TResult>>();
	}

	public class ColumnHeader
	{
		public Action<LayoutWriter> Content { get; set; }
		public Action<ThTagAttributes> Attributes { get; set; }
		
		public ColumnHeader() { }
		public ColumnHeader(Action<ThTagAttributes> attrs, Action<LayoutWriter> content)
		{
			Attributes = attrs;
			Content = content;
		}
	}

	public class ListColumn<TResult>
	{
		public RenderRowCellDelegate<TResult> Content { get; set; }
		public RowCellAttributesDelegate<TResult> Attributes { get; set; }
		public RowCellFlagDelegate<TResult> Visible { get; set; } = (o, i) => true;

		public ListColumn() { }
		public ListColumn(RowCellAttributesDelegate<TResult> attrs, RenderRowCellDelegate<TResult> content)
		{
			Attributes = attrs;
			Content = content;
		}
	}

	public delegate void RenderHeaderDelegate(LayoutWriter w, IEnumerable<Action<LayoutWriter>> headers);
	public delegate void RenderGroupCellDelegate<TResult>(LayoutWriter w, TResult obj);
	public delegate void RenderGroupRowDelegate<TResult>(TResult obj, string groupTitle, RenderGroupCellDelegate<TResult> renderGroupCell);
	public delegate void RenderRowDelegate<TResult>(TResult obj, RowInfo row);
	public delegate void RenderRowCellDelegate<TResult>(LayoutWriter w, TResult obj, RowInfo row);
	public delegate void RowCellAttributesDelegate<TResult>(TdTagAttributes w, TResult obj, RowInfo row);
	public delegate bool RowCellFlagDelegate<TResult>(TResult obj, RowInfo row);

	public class ListGroupSorting
	{
		public int SeqNo;
		public bool SortDesc;
	}

	public struct RowInfo
	{
		public int RowNum { get; set; }
		public int Level { get; set; }
	}

	public static class ListRowExtensions
	{
		public static void ZebraStripping(this TagAttributes a, int rnum)
		{
			if (rnum % 2 != 0) a.Class("alt");
		}
	}
}
