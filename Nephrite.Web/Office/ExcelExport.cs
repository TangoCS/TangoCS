using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Web.UI.WebControls;
using System.Text;


namespace Nephrite.Web.Office
{
	public class ExcelExport
	{
		/// <summary>
		/// Сформировать файл
		/// </summary>
		/// <param name="ctrl">Элемент управления, содержимое которого экспортируется в Excel
		/// (как правило, должен содержать html-код, корневым элементом которого является table</param>
		/// <param name="fileName">Имя файла</param>
		public static void Generate(Control ctrl, string fileName)
		{
			string path = Settings.BaseControlsPath + "excel.ascx";
			UserControl control = new UserControl();
			control = (UserControl)control.LoadControl(path);

			string text;
			using (StringWriter sw = new StringWriter())
			{
				using (HtmlTextWriter hw = new HtmlTextWriter(sw))
				{
					ctrl.RenderControl(hw);
					text = sw.ToString();
				}
			}

			((PlaceHolder)control.FindControl("phMain")).Controls.Add(new LiteralControl(text));

			HttpResponse response = HttpContext.Current.Response;
			response.AppendHeader("Content-Type", "application/vnd.ms-excel");
			if (!fileName.ToLower().EndsWith(".xls"))
				fileName += ".xls";
			response.Headers.Remove("Content-disposition");
			response.Charset = "utf-8";
			
			if (HttpContext.Current.Request.Browser.Browser == "IE")
			{
				response.HeaderEncoding = Encoding.Default;
				response.AppendHeader("content-disposition", "Attachment; FileName=\"" + HttpUtility.UrlPathEncode(fileName) + "\"");
			}
			else
			{
				response.HeaderEncoding = Encoding.UTF8;
				response.AppendHeader("content-disposition", "Attachment; FileName=\"" + fileName + "\"");
			}

			using (StringWriter sw = new StringWriter())
			{
				using (HtmlTextWriter hw = new HtmlTextWriter(sw))
				{
					control.RenderControl(hw);
					response.Write(sw.ToString());
				}
			}
			response.End();
		}

		public static void Generate(string html, string fileName)
		{
			string path = Settings.BaseControlsPath + "excel.ascx";
			UserControl control = new UserControl();
			control = (UserControl)control.LoadControl(path);

			((PlaceHolder)control.FindControl("phMain")).Controls.Add(new LiteralControl(html));

			HttpResponse response = HttpContext.Current.Response;
			response.AppendHeader("Content-Type", "application/vnd.ms-excel");

			response.Charset = "utf-8";
			
			if (HttpContext.Current.Request.Browser.Browser == "IE")
			{
				response.HeaderEncoding = Encoding.Default;
				response.AppendHeader("content-disposition", "Attachment; FileName=\"" + HttpUtility.UrlPathEncode(fileName) + "\"");
			}
			else
			{
				response.HeaderEncoding = Encoding.UTF8;
				response.AppendHeader("content-disposition", "Attachment; FileName=\"" + fileName + "\"");
			}
			using (StringWriter sw = new StringWriter())
			{
				using (HtmlTextWriter hw = new HtmlTextWriter(sw))
				{
					control.RenderControl(hw);
					response.Write(sw.ToString());
				}
			}
			response.End();
		}
	}
}
