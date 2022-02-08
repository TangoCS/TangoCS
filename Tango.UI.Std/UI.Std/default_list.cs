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
		[Inject]
		protected IListPagingRenderer PagingRenderer { get; set; }
		[Inject]
		protected IPersistentFilterStore<int> filterStore { get; set; }

		protected string _qSearch = "";
		protected InputName _qSearchParmName => new InputName { Name = GetClientID("qsearch"), ID = "qsearch" };
		protected IFieldCollection<TEntity, TResult> _fields;
		protected IFieldCollection<TEntity, TResult> Fields
		{
			get
			{
				ForceFieldsInit();
				return _fields;
			}
		}
		protected void ForceFieldsInit()
		{
			if (_fields == null)
				_fields = FieldsConstructor();
		}
		//protected Action<ActionLink> _pagingAttributes => a => a.RunEvent(OnSetPage);

		protected IEnumerable<TResult> _result;
		protected int? _itemsCount;

		protected abstract int GetCount();
		protected abstract IEnumerable<TResult> GetPageData();
		public abstract IEnumerable<TResult> GetAllData();
		protected abstract IFieldCollection<TEntity, TResult> FieldsConstructor();

		public ListFilter<TEntity> Filter { get; private set; }
		public Paging Paging { get; private set; }
		public Sorter<TEntity> Sorter { get; private set; }
		public ListRendererAbstract<TResult> Renderer { get; protected set; }

		public int ColumnCount => Fields.Cells.Count;

		protected virtual string FormTitle => Resources.CaptionPlural<TEntity>();
		protected virtual Func<string, Expression<Func<TEntity, bool>>> SearchExpression => null;
		protected virtual string SearchExpressionTooltip { get; }

		protected virtual bool EnableViews => true;
		protected virtual bool EnableQuickSearch => true;
		protected virtual bool ShowFilterV2 => true;
		protected virtual bool EnableSelect => false;
		protected virtual bool GenerateClientViewData => false;

		protected virtual bool EnableHover => false;
		protected virtual bool EnableKeyboard => false;

		protected virtual void Toolbar(LayoutWriter w)
		{
			PrepareResult();

			ToolbarTop(w);

			w.Toolbar(t => ToolbarLeft(t), t => {
				t.Item(tw => tw.Span(a => a.ID(Paging.ID)));
				t.ItemSeparator();
				ToolbarRight(t);
			});

			ToolbarBottom(w);
		}

		protected virtual void ToolbarTop(LayoutWriter w) { }
		protected virtual void ToolbarBottom(LayoutWriter w)
		{
			if (ShowFilterV2 && Filter.Criteria.Count > 0)
			{
				w.Div(a => a.Class("inlinefilter selectedcontainer"), () => {
					foreach (var i in Filter.Criteria)
					{
						w.Div(a => a.Class("selected object"), () => {
							w.Span(i.Title + " " + i.Condition + " " + i.ValueTitle);
							//w.A(a => a.Class("close").OnClickPostEvent(Filter.OnInlineCriterionRemoved).DataParm("removedcriterion", i.GetHashCode().ToString()), () => w.Icon("close"));
						});
					}
				});
			}
		}


		protected void ToDeleteBulk(MenuBuilder t)
		{
			if (Fields.EnableSelect)
			{
				t.ItemSeparator();
				t.ItemActionTextBulk(x => x.ToDeleteBulk<TEntity>(AccessControl).AsDialog());
			}
		}

		protected virtual void ToolbarLeft(MenuBuilder t)
		{
			t.ItemFilter(Filter);
			t.ToCreateNew<TEntity>();
			ToDeleteBulk(t);
		}

		protected virtual void ToolbarRight(MenuBuilder t)
		{
			if (EnableQuickSearch)
				t.QuickSearch(this, Paging, _qSearchParmName, SearchExpressionTooltip);
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
				var size = Context.GetIntArg(GetClientID("psize"));
				if (size != null)
					p.PageSize = size.Value;
			});
			Sorter = CreateControl<Sorter<TEntity>>("sort", s => {
				s.OnSort = OnSetPage;
			});
			Filter = CreateControl<ListFilter<TEntity>>("filter", f => {
				f.FieldsInit = () => {
					FilterInit(f);
					_fields = FieldsConstructor();
				};
				f.FilterSubmitted += OnFilter;
				f.AllowDefaultFilters = () => EnableViews;
			});

			_qSearch = Context.GetArg(_qSearchParmName.Name);
		}

		public virtual void PrepareResult()
		{
			if (_result != null) return;

			_result = GetPageData();

			var i = _result?.Count() ?? 0;

			_itemsCount = null;
			if (i <= Paging.PageSize && Paging.PageIndex <= 1)
				_itemsCount = i;
			if (Paging.PageIndex > 0 && _itemsCount == null)
				_itemsCount = GetCount();
		}

		public virtual void BeforeList(LayoutWriter w) { }
		public virtual void AfterList(LayoutWriter w) { }

		void Render(ApiResponse response)
		{
			PrepareResult();
			response.AddWidget(Sections.ContentBody, Render);
		}

		public virtual void Render(LayoutWriter w)
		{
			if (!Sections.RenderPaging)
				Paging.PageSize = int.MaxValue - 1;
			//w.PushPrefix(ClientID);
			PrepareResult();
			BeforeList(w);
			Renderer.Render(w, _result.Take(Paging.PageSize), Fields);
			AfterList(w);			
			//w.PopPrefix();
		}

		public virtual void RenderPlaceHolder(LayoutWriter w)
		{
			w.PushPrefix(ID);
			w.Div(a => a.ID("container").DataIsContainer("default", w.IDPrefix), () => {
				w.Div(a => a.ID(Sections.ContentTitle));
				w.Div(a => a.ID(Sections.ContentToolbar));
				w.Div(a => a.ID(Sections.ContentBody));
			});
			w.PopPrefix();
		}

		protected virtual void AfterRender(ApiResponse response)
		{
			if (Sections.RenderPaging)
			{
				response.ReplaceWidget(Paging.ID, w => {
					var opt = new PagingRenderOptions {
						ItemsCount = _itemsCount,
						PageActionAttributes = a => a.RunEvent(OnSetPage),
						ObjCountActionAttributes = a => a.PostEvent(GetObjCount),
						GoToPageActionAttributes = a => a.OnEnterPostEvent(OnSetPage),
						SetPageSizeActionAttributes = a => a.DataEvent(OnSetPage).OnChangeRunHref()
					};
					PagingRenderer.Render(Paging, w, opt);
				});
			}
		}

		public void RenderToolbar(ApiResponse response)
		{
			if (Sections.RenderToolbar)
				response.AddWidget(Sections.ContentToolbar, Toolbar);
		}

		private void OnFilter(ApiResponse response)
		{
			response.WithNamesAndWritersFor(this);

			if (Filter.Criteria.Count > 0)
			{
				if (Context.GetIntArg(Filter.ParameterName) != Filter.PersistentFilter.ID)
				{
					//TODO: изменение Context.AllArgs надо бы сделать внутри ChangeUrl
					Context.AllArgs.Remove(Paging.ParameterName);
					Context.AllArgs.Remove(_qSearchParmName.Name);
					Context.AllArgs[Filter.ParameterName] = Filter.PersistentFilter.ID;
					response.ChangeUrl(
						new List<string> { Filter.ParameterName, Paging.ParameterName, _qSearchParmName.Name },
						new Dictionary<string, object> { [Filter.ParameterName] = Filter.PersistentFilter.ID }
					);
				}
			}
			else
			{
				//TODO: изменение Context.AllArgs надо бы сделать внутри ChangeUrl
				Context.AllArgs.Remove(Paging.ParameterName);
				Context.AllArgs.Remove(_qSearchParmName.Name);
				Context.AllArgs[Filter.ParameterName] = 0;
				response.ChangeUrl(
						new List<string> { Filter.ParameterName, Paging.ParameterName, _qSearchParmName.Name },
						new Dictionary<string, object> { [Filter.ParameterName] = 0 }
					);
			}

			Paging.PageIndex = 1;
			Render(response);
			RenderToolbar(response);
			AfterRender(response);
		}

		public override void OnLoad(ApiResponse response)
		{
			if (Sections.RenderListOnLoad || !_qSearch.IsEmpty())
				Render(response);
			RenderToolbar(response);
			if (Sections.RenderContentTitle)
				response.AddWidget(Sections.ContentTitle, FormTitle);
			if (Sections.SetPageTitle)
				response.AddWidget("#title", FormTitle);
			if (GenerateClientViewData)
				response.State.Add(ClientID, new { rows = _result });
			AfterRender(response);
		}

		public void OnSetView(ApiResponse response)
		{
			Render(response);
			RenderToolbar(response);
			AfterRender(response);
		}

		public void OnQuickSearch(ApiResponse response)
		{
			Render(response);
			AfterRender(response);
		}

		public void OnSetPage(ApiResponse response)
		{
			var page_go = Context.GetIntArg("go");
			if (page_go != null)
				Paging.PageIndex = page_go.Value;

			Render(response);
			AfterRender(response);
		}

		public void GetObjCount(ApiResponse response)
		{
			_itemsCount = GetCount();
			AfterRender(response);
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

	public abstract class abstract_list<T> : abstract_list<T, T>
	{
		protected override IFieldCollection<T, T> FieldsConstructor()
		{
			var f = new FieldCollection<T>(Context, Sorter, Filter);
			if (EnableHover)
				f.ListAttributes += a => a.Class("hover");
			if (EnableKeyboard)
				f.ListAttributes += a => a.Class("kb");
			f.RowAttributes += (a, o, i) => a.ZebraStripping(i.RowNum);
			FieldsInit(f);
			return f;
		}

		protected abstract void FieldsInit(FieldCollection<T> fields);
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
			Fields.GroupSorting.Reverse();
			foreach (var gs in Fields.GroupSorting)
				Sorter.InsertOrderBy(gs.SeqNo, gs.SortDesc, true);

			var res = Selector(Paging.Apply(Sorter.Apply(ApplyFilter(Data)), true));
			return res.ToList();
		}

		public override IEnumerable<TResult> GetAllData()
		{
			return Selector(Sorter.Apply(Data));
		}

		protected override IFieldCollection<TEntity, TResult> FieldsConstructor()
		{
			var f = new FieldCollection<TEntity, TResult>(Context, Sorter, Filter);
			f.RowAttributes += (a, o, i) => a.ZebraStripping(i.RowNum);
			FieldsInit(f);
			return f;
		}

		protected abstract void FieldsInit(FieldCollection<TEntity, TResult> fields);
	}

	public abstract class default_list<T> : default_list<T, T>
	{
		protected override IQueryable<T> Selector(IQueryable<T> data) => data;
	}

	public class ListGroup<TResult>
	{
		public Func<TResult, string> ValueFunc;
		public RenderGroupCellDelegate<TResult> Cell;
		public string DefaultValue { get; set; }
		public Action<TResult, GroupRowDescription<TResult>> Cells { get; set; }
	}

	public class GroupRowDescription<TResult>
	{
		public List<(Action<TdTagAttributes> attributes, Action<LayoutWriter> inner)> Cells { get; } =
			new List<(Action<TdTagAttributes> attributes, Action<LayoutWriter> inner)>();
		public int? ColumnsCount { get; set; }
		public void AddCell(Action<TdTagAttributes> attributes = null, Action<LayoutWriter> inner = null)
		{
			Cells.Add((attributes, inner));
		}
	}

	public interface IColumnHeader
	{
		IEnumerable<ColumnHeader> AsEnumerable();
	}

	public interface IListColumn<TResult>
	{
		IEnumerable<ListColumn<TResult>> AsEnumerable(TResult o, RowInfo<TResult> r);
	}

	public class ColumnHeader : IColumnHeader
	{
		public Action<LayoutWriter> Content { get; set; }
		public Action<ThTagAttributes> Attributes { get; set; }

		public ColumnHeader() { }
		public ColumnHeader(Action<ThTagAttributes> attrs, Action<LayoutWriter> content)
		{
			Attributes = attrs;
			Content = content;
		}
		public ColumnHeader(string title)
		{
			Content = w => w.Write(title);
		}

		public IEnumerable<ColumnHeader> AsEnumerable()
		{
			yield return this;
		}
	}

	public class CustomColumnHeader : IColumnHeader
	{
		Func<IEnumerable<ColumnHeader>> _headers;

		public CustomColumnHeader(Func<IEnumerable<ColumnHeader>> headers)
		{
			_headers = headers;
		}

		public IEnumerable<ColumnHeader> AsEnumerable() => _headers?.Invoke();
	}

	public class ListColumn<TResult> : IListColumn<TResult>
	{
		public RenderRowCellDelegate<TResult> Content { get; set; }
		public RowCellAttributesDelegate<TResult> Attributes { get; set; }

		public ListColumn() { }
		public ListColumn(RowCellAttributesDelegate<TResult> attrs, RenderRowCellDelegate<TResult> content)
		{
			Attributes = attrs;
			Content = content;
		}

		public IEnumerable<ListColumn<TResult>> AsEnumerable(TResult o, RowInfo<TResult> r)
		{
			yield return this;
		}
	}

	public class CustomListColumn<TResult> : IListColumn<TResult>
	{
		Func<TResult, RowInfo<TResult>, IEnumerable<ListColumn<TResult>>> _columns;

		public CustomListColumn(Func<TResult, RowInfo<TResult>, IEnumerable<ListColumn<TResult>>> columns)
		{
			_columns = columns;
		}

		public IEnumerable<ListColumn<TResult>> AsEnumerable(TResult o, RowInfo<TResult> r) => _columns?.Invoke(o, r);
	}

	public delegate void RenderHeaderDelegate(LayoutWriter w, IEnumerable<Action<LayoutWriter>> headers);
	public delegate void RenderGroupCellDelegate<TResult>(LayoutWriter w, TResult obj);
	public delegate void RenderGroupRowDelegate<TResult>(TResult obj, string groupTitle, RenderGroupCellDelegate<TResult> renderGroupCell);
	public delegate void RenderRowDelegate<TResult>(TResult obj, RowInfo<TResult> row);
	public delegate void RenderRowCellDelegate<TResult>(LayoutWriter w, TResult obj, RowInfo<TResult> row);
	public delegate void RowCellAttributesDelegate<TResult>(TdTagAttributes w, TResult obj, RowInfo<TResult> row);
	public delegate bool RowCellFlagDelegate<TResult>(TResult obj, RowInfo<TResult> row);

	public class ListGroupSorting
	{
		public int SeqNo;
		public bool SortDesc;
	}

	public class RowInfo<TResult>
	{
		public TResult PrevRowData { get; set; }
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

	public static class ToolbarExtensions
	{
		public static void ToCreateNew<T>(this MenuBuilder t, Action<ActionLink> attrs = null, bool imageOnly = false, string returnAction = null, string returnID = "@ID")
		{
			if (returnAction is null)
				returnAction = Constants.OpView;
			
			var ac = t.Context.RequestServices.GetService(typeof(IAccessControl)) as IAccessControl;
			var tac = t.Context.RequestServices.GetService(typeof(ITypeActivatorCache)) as ITypeActivatorCache;

			t.ItemSeparator();
			var key = typeof(T).GetResourceType().Name + "." + returnAction;
			Action<ActionLink> attrs1 = null;
			if (tac.Get(key).HasValue)
			{
				var retUrl1 = new ActionLink(t.Context).To<T>(returnAction).WithArg(Constants.Id, returnID).Url;
				var retUrl0 = t.Context.CreateReturnUrl(1);
				attrs1 = x => x.ToCreateNew<T>(ac, null, retUrl1)
					.WithArg(Constants.ReturnUrl + "_0", retUrl0)
					.Set(attrs);
			}
			else
				attrs1 = x => x.ToCreateNew<T>(ac).Set(attrs);

			if (imageOnly)
				t.ItemActionImage(attrs1);
			else
				t.ItemActionImageText(attrs1);
		}

		public static void ToCreateNew(this MenuBuilder t, string service, string action,
			Action<ActionLink> attrs = null, bool imageOnly = false)
		{
			var ac = t.Context.RequestServices.GetService(typeof(IAccessControl)) as IAccessControl;
			t.ItemSeparator();
			Action<ActionLink> attrs1 = x => x.To(service, action, ac).WithImage("New").Set(attrs);

			if (imageOnly)
				t.ItemActionImage(attrs1);
			else
				t.ItemActionImageText(attrs1);
		}
	}
}
