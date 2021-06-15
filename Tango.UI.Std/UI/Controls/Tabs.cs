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

		TabPage GetCurPage()
		{
			var curid = GetArg(ID);
			return Pages.Where(o => o.ID == curid).FirstOrDefault() ?? Pages.FirstOrDefault();
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

		public T CreateTabPage<T>(string id, string title, Action<T> setProperties = null)
			where T : ViewPagePart, new()
		
		{
			var attr = typeof(T).GetCustomAttribute<SecurableObjectAttribute>();
			if (attr != null && ac != null && !ac.Check(attr.Name))
				return null;

			var c = CreateControl(id, setProperties);
			c.IsLazyLoad = true;
			Pages.Add(new TabPage(title, c));
			return c;
		}

		public void OnPageSelect(ApiResponse response)
		{
			var curpage = GetCurPage();

			if (curpage != null)
			{
				var el = curpage.Content.Target as IViewElement;
				//Context.AddContainer = true;
				//Context.ContainerPrefix = ParentElement.ClientID;

				curpage.Container.ParentElement = ParentElement;
				curpage.Container.ID = $"{ID}_{curpage.ID}";
				curpage.Container.ProcessResponse(response, true, ParentElement.ClientID);

				response.WithWritersFor(curpage.Container);
				curpage.Content(response);
			}
		}

		public void RenderTabs(LayoutWriter w)
		{
			w.Div(a => a.ID(ID).Data("parmname", ID).Class("tabs2"), () => {
				w.Ul(() => {
					foreach (var p in Pages)
					{
						var pid = $"{ID}_{p.ID}";
						w.Li(() => {
							var curpage = GetCurPage();
							w.RadioButton(ClientID, pid + "_title", null, curpage.ID == p.ID);
							w.Label(a => a.ID(pid + "_label").For(pid + "_title")
								.Data("id", p.ID).Data("ajax", p.IsAjax).Data("useurlparm", true).Data("loaded", !p.IsAjax || curpage.ID == p.ID)
								.Set(p.Attributes)
								.OnClick("tabs.onselect(this)"), () => w.Write(p.Title));
						});
					}
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
					w.Div(a => a.ID($"{pid}_{PageSuffix}").Class(curpage.ID == p.ID ? "selected" : "").DataContainer("tabpage", container));
				}
			});
		}
	}

	public class TabPage
	{
		public string ID { get; set; }
		public string Title { get; set; }
		public Action<ApiResponse> Content { get; set; }
		public bool IsAjax { get; set; }
		public ViewContainer Container { get; set; }
		
		//!TODO: подумать над преобразованием.
		public Action<LabelTagAttributes> Attributes { get; set; }

		public TabPage(string id, string title, Action<ApiResponse> content, bool isAjax = false)
		{
			ID = id;
			Title = title;
			Content = content;
			IsAjax = isAjax;
			Container = new TabPageContainer();
		}

		public TabPage(string title, ViewPagePart element)
		{
			ID = element.ID;
			Title = title;
			Content = response => {
				element.IsLazyLoad = false;
				element.RunOnEvent();
				element.OnLoad(response);
			};
			IsAjax = true;
			Container = new TabPageContainer2(element.GetContainer());
		}
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
