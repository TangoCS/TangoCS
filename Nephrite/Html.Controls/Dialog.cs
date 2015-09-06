using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nephrite.AccessControl;
using Nephrite.MVC;
using Newtonsoft.Json;

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

	public static class DialogExtensions
	{
		public static ActionLink ActionOpenDialogLink(this HtmlHelper html, DialogOptions options, string title = null, Action<ATagAttributes> customATagAttributes = null)
		{
			return new OpenDialogLink(html.UrlHelper, html.AccessControl).Link(options, title, customATagAttributes).UseRoute("api");
		}
	}

	public class OpenDialogLink : ActionLink
	{
		Action<ATagAttributes> _aTagAttributes = null;

		public OpenDialogLink(IUrlHelper urlHelper, IAccessControl accessControl) : base(urlHelper, accessControl) { }

		public OpenDialogLink Link(DialogOptions options, string title = null, Action<ATagAttributes> customATagAttributes = null)
		{
			if (!title.IsEmpty()) Title = title;

			_aTagAttributes = a => {
				options.Url = Url;
				a.Href = "#";
				a.OnClick = "dialog.show(this, " + JsonConvert.SerializeObject(options, Json.CamelCase) + ")";
				if (customATagAttributes != null) customATagAttributes(a);
			};
			return this;
		}

		public override string ToString()
		{
			HtmlWriter w = new HtmlWriter();
			w.A(_aTagAttributes, Title);
			return w.ToString();
		}
	}
}
