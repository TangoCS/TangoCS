using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Nephrite.Http;

namespace Nephrite
{
	public static partial class SimpleTypeExtensionsWeb
	{
		public static void DataBindOnce(this DropDownList ddl, object dataSource)
		{
			if (ddl.Items.Count == 0)
			{
				ddl.DataSource = dataSource;
				ddl.DataBind();
			}
		}

		public static void DataBindOnce(this DropDownList ddl, object dataSource, bool insertEmptyString)
		{
			if (ddl.Items.Count == 0)
			{
				ddl.DataSource = dataSource;
				ddl.DataBind();
				if (insertEmptyString) ddl.Items.Insert(0, "");
			}
		}

		public static void SetValue(this DropDownList ddl, object value)
		{
			if (value == null)
			{
				ddl.SelectedValue = null;
				return;
			}
			if (ddl.Items.FindByValue(value.ToString()) != null)
				ddl.SelectedValue = value.ToString();
		}

		public static int? GetValue(this DropDownList ddl)
		{
			return ddl.SelectedValue.ToInt32();
		}

		public static int GetValue(this DropDownList ddl, int defaultValue)
		{
			return ddl.SelectedValue.ToInt32(defaultValue);
		}

		public static Guid? GetValueGuid(this DropDownList ddl)
		{
			if (ddl.SelectedValue.IsEmpty()) return null;
			return new Guid(ddl.SelectedValue);
		}

		public static IEnumerable<ListItem> GetSelected(this CheckBoxList cbl)
		{
			return cbl.Items.OfType<ListItem>().Where(o => o.Selected);
		}
		public static IEnumerable<string> GetSelectedValues(this CheckBoxList cbl)
		{
			return cbl.Items.OfType<ListItem>().Where(o => o.Selected).Select(o => o.Value);
		}
		public static IEnumerable<int> GetSelectedValuesInt(this CheckBoxList cbl)
		{
			return cbl.Items.OfType<ListItem>().Where(o => o.Selected).Select(o => o.Value.ToInt32(0));
		}
		public static void SelectItems(this CheckBoxList cbl, IEnumerable<int> values)
		{
			foreach (ListItem li in cbl.Items.OfType<ListItem>().Where(o => values.Contains(o.Value.ToInt32(0))))
				li.Selected = true;
		}
		public static void SelectItems(this CheckBoxList cbl, IEnumerable<string> values)
		{
			foreach (ListItem li in cbl.Items.OfType<ListItem>().Where(o => values.Contains(o.Value)))
				li.Selected = true;
		}

		

		public static string RenderControl(this Control ctrl)
		{
			StringBuilder sb = new StringBuilder();
			StringWriter tw = new StringWriter(sb);
			HtmlTextWriter hw = new HtmlTextWriter(tw);

			ctrl.RenderControl(hw);
			return sb.ToString();
		}

		public static void Redirect(this HttpResponse response, string url, string target, string windowFeatures)
		{

			if ((String.IsNullOrEmpty(target) ||
				target.Equals("_self", StringComparison.OrdinalIgnoreCase)) &&
				String.IsNullOrEmpty(windowFeatures))
			{
				response.Redirect(url);
			}
			else
			{
				Page page = (Page)HttpContext.Current.Handler;
				if (page == null)
				{
					throw new InvalidOperationException(
						"Cannot redirect to new window outside Page context.");

				}

				url = page.ResolveClientUrl(url);

				string script;
				if (!String.IsNullOrEmpty(windowFeatures))
				{
					script = @"window.open(""{0}"", ""{1}"", ""{2}"");";
				}
				else
				{
					script = @"window.open(""{0}"", ""{1}"");";
				}

				script = String.Format(script, url, target, windowFeatures);
				ScriptManager.RegisterStartupScript(page,
					typeof(Page),
					"Redirect",
					script,
					true);
			}

		}

		

		/// <summary>
		/// Вывод текста исключения в поток
		/// </summary>
		/// <param name="ex"></param>
		/// <param name="writer"></param>
		public static void Render(this Exception ex, HtmlTextWriter writer)
		{
			writer.Write("<i>" + HttpContext.Current.Request.Url.AbsoluteUri + "</i>");
			writer.WriteBreak();
			writer.WriteBreak();
			Exception e = ex;
			while (e != null)
			{
				writer.Write("<b>");
				writer.Write(HttpUtility.HtmlEncode(e.Message));
				writer.Write("</b>");
				writer.WriteBreak();
				writer.WriteBreak();
				writer.Write(HttpUtility.HtmlEncode(e.StackTrace).Replace("\n", "<br />"));
				writer.WriteBreak();
				writer.WriteBreak();
				writer.WriteBreak();
				e = e.InnerException;
			}
		}
	}

	public static class UrlHelper
	{
		public static Url Current()
		{
			var request = HttpContext.Current.Request;
			return new Url(
				request.Url.PathAndQuery, 
				//request.QueryString.AllKeys.ToDictionary(k => k, k => request.QueryString[k]), 
				request.RequestContext.RouteData.Values);
		}

		public static void Go(this Url url)
		{
			HttpContext.Current.Response.Redirect(url);
		}
	}
}

namespace Nephrite.Multilanguage
{
	public static class LangExtenderWeb
	{
		public static string Lang(this System.Web.UI.Control ctrl, string lang, string data)
		{
			if (Language.Current.Code.ToUpper() == lang.ToUpper())
				return data;
			return String.Empty;
		}

		public static string Lang(this System.Web.UI.Control ctrl, string lang, string data, params string[] otherLang)
		{
			string lc = Language.Current.Code.ToUpper();
			if (lc == lang.ToUpper())
				return data;
			for (int i = 0; i < otherLang.Length; i += 2)
				if (otherLang[i].ToUpper() == lc)
					return otherLang[i + 1];
			return String.Empty;
		}
	}
}