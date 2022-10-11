using System;
using Tango.Html;

namespace Tango.UI.Controls
{
	public static class LayoutWriterTabsExtensions
	{
		public static void Tabs(this LayoutWriter w, string id, params StaticTabPage[] tabPages)
		{
			w.Div(a => a.ID(id).Class("tabs2"), () => {
				w.Ul(() => {            
                 
                    for (int i = 0; i < tabPages.Length; i++)
                    {
                        var p = tabPages[i];
                        w.Li(a => a.Class("tablink"), () =>
                        {                           
                            w.RadioButton(id, p.ID + "_title", null, isChecked: tabPages[i].Selected.HasValue ? tabPages[i].Selected.Value : (i == 0 ? true : false));
                            w.Label(a => a.For(p.ID + "_title").Data("id", p.ID).OnClick("tabs.onselect(this)"), () => w.Write(p.Title));
                        });
                    }
                });
			});
			w.Div(a => a.ID(id + "_pages").Class("tabs2_pages"), () => {
				for (int i = 0; i < tabPages.Length; i++)
				{                   
                    var p = tabPages[i];
                    w.Div(a => a.ID(p.ID + "_tabpage").Style(tabPages[i].Style).Class(tabPages[i].Selected.HasValue ? (tabPages[i].Selected.Value ? "selected" : "") : (i == 0 ? "selected": "")), () => p.Content(w));                    
                }
			});
		}
	}

	public class StaticTabPage
	{
		public string ID { get; set; }
		public string Title { get; set; }
		public Action<LayoutWriter> Content { get; set; }
        public bool? Selected { get; set; }
		public string Style { get; set; }
		public StaticTabPage(string id, string title, Action<LayoutWriter> content, bool? selected = null, string style = null)
		{
			ID = id;
			Title = title;
			Content = content;
            Selected = selected;
			Style = style;

		}

		public void Render(LayoutWriter w)
		{
			w.PushPrefix(ID);
			Content(w);
			w.PopPrefix();
		}
	}
}
