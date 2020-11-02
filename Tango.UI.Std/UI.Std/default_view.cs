using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tango.AccessControl;
using Tango.Data;
using Tango.UI.Controls;

namespace Tango.UI.Std
{
	public abstract class default_view<T> : ViewPagePart
		where T: class
	{
		[Inject]
		protected IAccessControl AccessControl { get; set; }

		public override ViewContainer GetContainer() => new ViewEntityContainer();

		T _viewData = null;

		public default_view()
		{
			ID = GetType().Name;
		}

		bool _viewDataLoaded = false;
		public virtual T ViewData
		{
			get
			{
				if (!_viewDataLoaded)
				{
					_viewData = GetExistingEntity();
					_viewDataLoaded = true;
				}
				return _viewData;
			}
			set
			{
				_viewData = value;
				_viewDataLoaded = true;
			}
		}

		protected virtual bool ObjectNotExists => ViewData == null;

		protected List<IFieldGroup> groups = new List<IFieldGroup>();
		protected TGr AddFieldGroup<TGr>(TGr group)
			where TGr : IFieldGroup
		{
			group.Init(this);
			groups.Add(group);
			return group;
		}

		protected virtual string FormTitle => ViewData is IWithTitle ? (ViewData as IWithTitle).Title : "";

		protected abstract T GetExistingEntity();

		protected virtual void Toolbar(LayoutWriter w)
		{
			w.Toolbar(t => ToolbarLeft(t), t => ToolbarRight(t));
		}

		protected virtual void ToolbarLeft(MenuBuilder t)
		{
			t.ItemBack();
		}

		protected virtual void ToolbarRight(MenuBuilder t) { }

		protected abstract void Form(LayoutWriter w);

		protected virtual void ButtonsBar(LayoutWriter w)
		{
			//w.ButtonsBar_view();
		}

		protected virtual void LinkedData(LayoutWriter w) { }
		protected virtual void Footer(LayoutWriter w) { }

		public override void AfterInit()
		{
			if (groups.Count == 0)
			{
				var grProps = GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
					.Where(o => typeof(IFieldGroup).IsAssignableFrom(o.PropertyType));
				foreach (var prop in grProps)
				{
					var gr = Activator.CreateInstance(prop.PropertyType) as IFieldGroup;
					gr.InjectProperties(Context.RequestServices);
					AddFieldGroup(gr);
					prop.SetValue(this, gr);
				}
			}
			BeforeFieldsInit();
			groups.ForEach(g => g.SetViewData(ViewData));
		}

		protected virtual void BeforeFieldsInit()
		{
		}

        public override void OnLoad(ApiResponse response)
		{
			if (ObjectNotExists)
			{
				response.AddWidget("form", w => {
					w.Div(Resources.Get("Common.ObjectNotExists"));
				});
			}
			else
			{
				response.AddWidget(Sections.ContentBody, w => {
					Form(w);
					ButtonsBar(w);
				});
				response.SetContentBodyMargin();
				response.AddAdjacentWidget(Sections.ContentBody, "linked", AdjacentHTMLPosition.BeforeEnd, w => {
					LinkedData(w);
					Footer(w);
				});

				if (Sections.RenderContentTitle)
					response.AddWidget(Sections.ContentTitle, FormTitle);
				if (Sections.SetPageTitle)
					response.AddWidget("#title", FormTitle);
				if (Sections.RenderToolbar)
					response.AddWidget(Sections.ContentToolbar, w => Toolbar(w));

				foreach (var r in Context.EventReceivers)
					if (r.ParentElement.ClientID == this.ClientID && r is Tabs tabs)
						tabs.OnPageSelect(response);
			}
		}

		public ViewSections Sections { get; set; } = new ViewSections();
		public class ViewSections
		{
			public string ContentBody { get; set; } = "contentbody";
			public string ContentToolbar { get; set; } = "contenttoolbar";
			public string ContentTitle { get; set; } = "contenttitle";
			public bool SetPageTitle { get; set; } = true;
			public bool RenderToolbar { get; set; } = true;
			public bool RenderContentTitle { get; set; } = true;
		}
    }

    public abstract class default_view<T, TKey> : default_view<T>
		where T : class, IWithKey<T, TKey>, new()
	{
		[Inject]
		protected IDataContext DataContext { get; set; }

		protected override void ToolbarLeft(MenuBuilder t)
		{
			base.ToolbarLeft(t);
			t.ItemSeparator();
			t.ItemActionImageText(x => x.ToEdit(AccessControl, ViewData));
			t.ItemSeparator();
			t.ItemActionImageText(x => x.ToDelete(AccessControl, ViewData, Context.ReturnUrl.Get(1))
				.WithArg(Constants.ReturnUrl + "_0", Context.CreateReturnUrl(1)));
			if (ViewData is IWithLogicalDelete)
				t.ItemActionImageText(x => x.ToUndelete(AccessControl, ViewData));
		}

		protected override T GetExistingEntity()
		{
			var obj = new T();
			var id = Context.GetArg<TKey>(Constants.Id);
			return DataContext.GetTable<T>().Filtered().FirstOrDefault(obj.KeySelector(id));
		}
	}

	public abstract class default_view_rep<T, TKey> : default_view<T>
		where T : class, IWithKey<T, TKey>, new()
	{
		[Inject]
		protected IDatabase Database { get; set; }

		protected override void ToolbarLeft(MenuBuilder t)
		{
			base.ToolbarLeft(t);
			t.ItemSeparator();
			t.ItemActionImageText(x => x.ToEdit(AccessControl, ViewData));
			t.ItemSeparator();
			t.ItemActionImageText(x => x.ToDelete(AccessControl, ViewData, Context.ReturnUrl.Get(1))
				.WithArg(Constants.ReturnUrl + "_0", Context.CreateReturnUrl(1)));
			if (ViewData is IWithLogicalDelete)
				t.ItemActionImageText(x => x.ToUndelete(AccessControl, ViewData));
		}

		protected override T GetExistingEntity()
		{
			var id = Context.GetArg<TKey>(Constants.Id);
			var obj = Database.Repository<T>().GetById(id);
			return obj;
		}
	}

	public abstract class default_view<T, TKey, TUser> : default_view<T, TKey>
		where T : class, IWithKey<T, TKey>, new()
		where TUser : IWithTitle
	{
		protected override void Footer(LayoutWriter w)
		{
			if (ViewData is IWithUserTimeStamp<TUser>)
				w.LastModifiedBlock(ViewData as IWithUserTimeStamp<TUser>);
			if (ViewData is IWithUserTimeStampEx<TUser>)
				w.TimeStampExBlock(ViewData as IWithUserTimeStampEx<TUser>);
		}
	}
}
