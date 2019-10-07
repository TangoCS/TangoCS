using System;
using System.Collections.Generic;
using System.Linq;
using Tango.Html;
using Tango.UI.Std;

namespace Tango.UI.Controls
{
	public class Tabs : ViewComponent
	{
		const string PageSuffix = "tabpage";

		public List<TabPage> Pages { get; set; } = new List<TabPage>();

		string _curid;
		TabPage _curpage;

		public override void AfterInit()
		{
			_curid = GetArg(ID);
			_curpage = Pages.Where(o => o.ID == _curid).FirstOrDefault() ?? Pages.FirstOrDefault();
		}

		public void OnPageSelect(ApiResponse response)
		{
			if (_curpage != null)
			{
				var el = _curpage.Content.Target as IViewElement;
				//Context.AddContainer = true;
				//Context.ContainerPrefix = ParentElement.ClientID;

				_curpage.Container.ParentElement = ParentElement;
				_curpage.Container.ID = $"{ID}_{_curpage.ID}";
				_curpage.Container.ProcessResponse(response, true, ParentElement.ClientID);

				response.WithWritersFor(_curpage.Container);
				_curpage.Content(response);
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
							w.RadioButton(ClientID, pid + "_title", null, _curpage.ID == p.ID);
							w.Label(a => a.ID(pid + "_label").For(pid + "_title")
								.Data("id", p.ID).Data("ajax", p.IsAjax).Data("useurlparm", true).Data("loaded", !p.IsAjax || _curpage.ID == p.ID)
								.OnClick("tabs.onselect(this)"), () => w.Write(p.Title));
						});
					}
				});
			});
		}

		public void RenderPages(LayoutWriter w)
		{
			w.Div(a => a.Class("tabs2").ID(ID + "_pages"), () => {
				foreach (var p in Pages)
				{
					var pid = $"{ID}_{p.ID}";
					w.Div(a => a.ID($"{pid}_{PageSuffix}").Class(_curpage.ID == p.ID ? "selected" : "").DataContainer("tabpage", pid));
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
			Content = response => element.OnLoad(response);
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
