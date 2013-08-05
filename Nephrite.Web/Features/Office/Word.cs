using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Web.UI.WebControls;
using Nephrite.Web.SettingsManager;

namespace Nephrite.Web.Office
{
    public static class Word
    {
        public static void Generate(string fileName, string html, string title, bool landscape)
        {
            string path = Settings.BaseControlsPath + "word.ascx";
            UserControl u = new UserControl();
            u = (UserControl)u.LoadControl(path);
            
            u.FindControl("Title").Controls.Add(new LiteralControl(title));
            if (landscape)
                u.FindControl("Section1").Controls.Add(new LiteralControl(@"size:841.9pt 595.3pt;
	mso-page-orientation:landscape;
	margin:1.0cm 2.0cm 42.5pt 2.0cm;
	mso-header-margin:35.4pt;
	mso-footer-margin:35.4pt;"));
            else
				u.FindControl("Section1").Controls.Add(new LiteralControl(@"margin:1.0cm 1.5cm 1.0cm 2.5cm;
	mso-header-margin:1.0cm;
	mso-footer-margin:1.0cm;
	mso-paper-source:0;"));
            u.FindControl("phMain").Controls.Add(new LiteralControl(html));

            HttpResponse response = HttpContext.Current.Response;
			if (HttpContext.Current.Request.Browser.Browser == "IE")
				response.HeaderEncoding = Encoding.Default;
			else
				response.HeaderEncoding = Encoding.UTF8;
			response.ContentType = "application/msword";
			if (!fileName.ToLower().EndsWith(".doc"))
				fileName += ".doc";
			
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
                    u.RenderControl(hw);
                    response.Write(sw.ToString());
                }
            }
			response.End();
        }

        public static byte[] GenerateBytes(string html, bool landscape)
        {
            string path = Settings.BaseControlsPath + "word.ascx";
            UserControl u = new UserControl();
            u = (UserControl)u.LoadControl(path);

            if (landscape)
                u.FindControl("Section1").Controls.Add(new LiteralControl(@"size:841.9pt 595.3pt;
	mso-page-orientation:landscape;
	margin:3.0cm 2.0cm 42.5pt 2.0cm;
	mso-header-margin:35.4pt;
	mso-footer-margin:35.4pt;"));
            else
				u.FindControl("Section1").Controls.Add(new LiteralControl(@"margin:1.0cm 1.5cm 1.0cm 2.5cm;
	mso-header-margin:1.0cm;
	mso-footer-margin:1.0cm;
	mso-paper-source:0;"));
            u.FindControl("phMain").Controls.Add(new LiteralControl(html));

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    u.RenderControl(hw);
                    return Encoding.UTF8.GetBytes(sw.ToString());
                }
            }
        }
    }
}
