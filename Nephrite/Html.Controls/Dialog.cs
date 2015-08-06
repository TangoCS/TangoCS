using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite.Html.Controls
{
	public class Dialog
	{
		public static string OKCancelButtons
		{
			get
			{
				HtmlWriter w = new HtmlWriter();
				w.Button(null, "ОК", (a) =>
				{
					a.Class = "ms-ButtonHeightWidth";
					a.OnClick = "dialog.submit(this)";
				});
				w.Write("&nbsp;");
				w.Button(null, "Отмена", (a) =>
				{
					a.Class = "ms-ButtonHeightWidth";
					a.OnClick = "dialog.hide(this)";
				});
				return w.ToString();
			}
		}
	}

	public class DialogOptions
	{
		public string Name { get; set; }
		public string Url { get; set; }
		public string ParentName { get; set; }
	}
}
