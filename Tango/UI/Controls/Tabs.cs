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

		string _curid;
		TabPage _curpage;

		public override void OnInit()
		{
			_curid = GetArg(ID);
			_curpage = Pages.Where(o => o.ID == _curid).FirstOrDefault() ?? Pages.FirstOrDefault();
		}

		public void OnPageSelect(ApiResponse response)
		{
			if (_curpage != null)
				response.WithNamesAndWritersFor(ParentElement).AddWidget(_curid + "_page", _curpage.Render);			
		}

		public void Render(LayoutWriter w)
		{
			RenderTabs(w);
			RenderPages(w);
		}

		public void RenderTabs(LayoutWriter w)
		{
			w.Div(a => a.ID(ID).Data("parmname", ID).Class("tabs2"), () => {
				w.Ul(() => {
					foreach (var p in Pages)
					{
						w.Li(() => {
							w.RadioButton(ClientID, p.ID + "_title", null, _curpage.ID == p.ID);
							w.Label(a => a.For(p.ID + "_title")
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
					w.Div(a => a.ID(p.ID + "_page").Class(_curpage.ID == p.ID ? "selected" : ""), () => {
						if (!p.IsAjax || _curpage.ID == p.ID)
							p.Render(w);
					});
				}
			});
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

		public void Render(LayoutWriter w)
		{
			w.PushID(ID);
			Content(w);
			w.PopID();
		}
	}
}
