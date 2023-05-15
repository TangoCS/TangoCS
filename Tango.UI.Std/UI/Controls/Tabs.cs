using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tango.AccessControl;
using Tango.Html;
using Tango.Localization;
using Tango.UI.Std;

namespace Tango.UI.Controls
{
	public class Tabs : ViewComponent
	{
		const string PageSuffix = "tabpage";

		public List<TabPage> Pages { get; set; } = new List<TabPage>();

		public Action<LayoutWriter> BeforeTabs { get; set; }
		public Action<LayoutWriter> AfterTabs { get; set; }

		TabPage GetCurPage()
		{
			var curid = GetArg(ID)?.ToLower();
			return Pages.Where(page => page.ID.ToLower() == curid && !page.Disabled).FirstOrDefault() ?? Pages.FirstOrDefault();
		}

		IAccessControl _ac;
		bool acInitialized = false;
		IAccessControl ac
		{
			get
			{
				if (!acInitialized)
					_ac = Context.RequestServices.GetService(typeof(IAccessControl)) as IAccessControl;
				return _ac;
			}
		}

        public T CreateTabPage<T>(string id, TabPageOptions options, Action<T> setProperties = null)
            where T : ViewPagePart, new()

        {
            var attr = typeof(T).GetCustomAttribute<SecurableObjectAttribute>();
            if (attr != null && ac != null && !ac.Check(attr.Name))
                return null;

            var c = CreateControl(id, setProperties);
            c.IsLazyLoad = true;
            Pages.Add(new TabPage(options, c));
            return c;
        }

		//public T CreateTabPage<T>(string id, string title, Action<T> setProperties = null)
		//	where T : ViewPagePart, new() => CreateTabPage(id, title, setProperties);
		public T CreateTabPage<T>(string id, Action<LayoutWriter> title, Action<T> setProperties = null)
			where T : ViewPagePart, new() => CreateTabPage(id, (TabPageOptions)title, setProperties);

		public void OnPageSelect(ApiResponse response)
		{
			var curpage = GetCurPage();

			if (curpage != null)
				RenderTabContent(response, curpage);

			if (curpage != null && curpage.ID != GetArg(ID))
				response.ChangeUrl(new List<string> { ID }, new Dictionary<string, object> { { ID, curpage.ID } });
		}

		public void OnLoadPageSelect(ApiResponse response)
		{
			var curpage = GetCurPage();

			foreach (var p in Pages)
			{
				if (!p.IsAjax || p == curpage)
					RenderTabContent(response, p);
			}
		}

		void RenderTabContent(ApiResponse response, TabPage page)
		{
			var el = page.Content.Target as IViewElement;
			//Context.AddContainer = true;
			//Context.ContainerPrefix = ParentElement.ClientID;

			page.Container.ParentElement = ParentElement;
			page.Container.ID = $"{ID}_{page.ID}";
			page.Container.ProcessResponse(response, true, ParentElement.ClientID);

			response.WithWritersFor(page.Container);
			page.Content(response);
		}

		public void RenderTabs(LayoutWriter w)
		{
			w.Div(a => a.ID(ID).Data("parmname", ID).Class("tabs2"), () => {
				w.Ul(() => {
					if (BeforeTabs != null) w.Li(a => a.Class("beforetabs"), () => BeforeTabs(w));
					foreach (var p in Pages)
					{
						var pid = $"{ID}_{p.ID}";
						w.Li(a => a.Class("tablink"), () => {
							var curpage = GetCurPage();
							w.RadioButton(ClientID, pid + "_title", null, curpage.ID == p.ID, attr =>
							{
                                if (p.Disabled)
                                {
                                    attr.Custom("disabled", "disabled");
                                }
                            });
							w.Label(a =>
							{
								a.ID(pid + "_label")
								.For(pid + "_title")
								.Data("id", p.ID).Data("ajax", p.IsAjax).Data("useurlparm", true).Data("loaded", !p.IsAjax || curpage.ID == p.ID)
								.Data(p.DataCollection)
								.Set(p.Attributes)
								.OnClick("tabs.onselect(this)");
							},
							() => p.Title(w));
						});
					}
					if (AfterTabs != null) w.Li(a => a.Class("aftertabs"), () => AfterTabs(w));
				});
			});			
		}

		public void RenderPages(LayoutWriter w, Action<TagAttributes> pagesContainerAttrs = null)
		{
			w.Div(a => a.Class("tabs2_pages").ID(ID + "_pages").Set(pagesContainerAttrs), () => {
				var curpage = GetCurPage();
				foreach (var p in Pages)
				{
					var pid = $"{ID}_{p.ID}";
					var container = $"{ClientID}_{p.ID}";
					w.Div(a => a.ID($"{pid}_{PageSuffix}").Class(curpage.ID == p.ID ? "selected" : "").DataIsContainer("tabpage", container));
				}
			});
		}

		public void ForcePageReload(ApiResponse response, string id)
		{
			var clientID = GetClientID($"{ID}_{id}_label");
			var page = Pages.Where(p => p.ID == id).Single();
			var value = !page.IsAjax;
			response.SetElementAttribute(clientID, "data-loaded", value.ToString());
		}
	}

	public class TabPageOptions
	{
		public Action<LayoutWriter> Title { get; set; }
		public bool Disabled { get; set; } = false;
		public bool IsAjax { get; set; } = true;

		public static implicit operator TabPageOptions(string title)
		{
			return new TabPageOptions { Title = w => w.Write(title) };
		}

		public static implicit operator TabPageOptions(Action<LayoutWriter> title)
		{
			return new TabPageOptions { Title = title };
		}
	}

	public class TabPage
	{
		public string ID { get; set; }
		public Action<LayoutWriter> Title { get; set; }
		public bool Disabled { get; set; } = false;
		public Action<ApiResponse> Content { get; set; }
		public bool IsAjax { get; set; }
		public ViewContainer Container { get; set; }
		
		//!TODO: подумать над преобразованием.
		public Action<LabelTagAttributes> Attributes { get; set; }

		public DataCollection DataCollection { get; set; }

        public TabPage(string id, TabPageOptions opt, Action<ApiResponse> content, bool isAjax = false)
        {
            ID = id;
			Title = opt.Title;
            Content = content;
            IsAjax = opt.IsAjax;
			Disabled = opt.Disabled;
            Container = new TabPageContainer();
        }
   //     public TabPage(string id, string title, Action<ApiResponse> content, bool isAjax = false) : 
			//this(id, w => w.Write(title), content, isAjax) { }

        public TabPage(TabPageOptions opt, ViewPagePart element)
		{
            ID = element.ID;
            Title = opt.Title;
            Content = response => {
                element.IsLazyLoad = false;
                element.RunOnEvent();
                element.OnLoad(response);
            };
            DataCollection = element.DataCollection;
            IsAjax = opt.IsAjax;
			Disabled = opt.Disabled;
            Container = new TabPageContainer2(element.GetContainer());
        }

  //      public TabPage(string title, ViewPagePart element) : this((TabPageOptions)title, element) 
		//{ 
		//}
	}

	public class TabPageContainer : ViewContainer
	{
		public TabPageContainer()
		{
			Mapping.Add("contentbody", "contentbody");
			Mapping.Add("contenttoolbar", "contenttoolbar");
		}

		public override void Render(ApiResponse response)
		{
			response.AddWidget($"{ID}_tabpage", w => {
				w.Div(a => a.ID("contenttoolbar"));
				w.Div(a => a.ID("contentbody"));
			});
		}
	}

	public class TabPageContainer2 : ViewContainer
	{
		ViewContainer _underlying;

		public TabPageContainer2(ViewContainer underlying)
		{
			underlying.ParentElement = this;
			_underlying = underlying;
		}

		public override void OnInit()
		{
			Mapping.Add("container", $"{ID}_tabpage");
			Mapping.Add("content", $"{ID}_content");
			_underlying.ToRemove.Add("contentheader");
		}

		public override void Render(ApiResponse response)
		{
			_underlying.Render(response);
		}
	}
}
