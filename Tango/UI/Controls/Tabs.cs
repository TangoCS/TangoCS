using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tango.Html;

namespace Tango.UI.Controls
{
	public class Tabs : ViewComponent
	{
		public List<TabPage> Pages { get; set; } = new List<TabPage>();

		public void OnPageSelect(ApiResponse response)
		{
			var p = GetArg(ID);
			var page = Pages.Where(o => o.ID == p).FirstOrDefault();

			if (page != null)
				response.AddWidget(ParentElement, p, w => page.Content(w));			
		}

		public void Render(LayoutWriter w)
		{
			w.Includes.Add("tango/tabs.js");
			w.Div(a => a.ID(ID).Data("parmname", ID).Class("tabs2"), () => {
				RenderPages(w);
			});		
		}

		void RenderPages(LayoutWriter w)
		{
			var defid = GetArg(ID);
			var def = Pages.Where(o => o.ID == defid).FirstOrDefault() ?? Pages.FirstOrDefault();

			w.Ul(() => {
				foreach (var p in Pages)
				{
					w.Li(() => {
						w.RadioButton(ClientID, p.ID + "_title", null, def.ID == p.ID);
						w.Label(a => a.For(p.ID + "_title")
							.Data("id", p.ID).Data("ajax", p.IsAjax).Data("useurlparm", true).Data("loaded", !p.IsAjax || def.ID == p.ID)
							.OnClick("tabs.onselect(this)"), () => w.Write(p.Title));
					});
				}
			});
			foreach (var p in Pages)
			{
				w.Div(a => a.ID(p.ID).Class(def.ID == p.ID ? "selected" : ""), () => {
					if (!p.IsAjax || def.ID == p.ID)
						p.Content(w);
				});
			}
		}
	}

	public class TabPage
	{
		public string ID { get; set; }
		public string Title { get; set; }
		public Action<LayoutWriter> Content { get; set; }
		public bool IsAjax { get; set; }

		//public TabPage() { }
		public TabPage(string id, string title, Action<LayoutWriter> content, bool isAjax = false)
		{
			ID = id;
			Title = title;
			Content = content;
			IsAjax = isAjax;
		}
	}
}
