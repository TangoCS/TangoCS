using System;
using System.Net;
using System.Text;
using Tango.Html;

namespace Tango.UI
{
	public class WordRenderer
	{
		public string Title { get; protected set; }
		public bool IsLandscape { get; protected set; } = false;
		public Action<LayoutWriter> Body { get; set; }

		string landscape = "size:841.9pt 595.3pt; mso-page-orientation:landscape; margin:1.0cm 2.0cm 42.5pt 2.0cm; mso-header-margin:35.4pt; mso-footer-margin:35.4pt;";
		string portrait = "margin:1.0cm 1.5cm 1.0cm 2.5cm; mso-header-margin:1.0cm; mso-footer-margin:1.0cm; mso-paper-source:0;";

		public void Render(LayoutWriter w)
		{
			w.DocType();
			w.Html(a => a.Xmlns("o", "urn:schemas-microsoft-com:office:office")
				.Xmlns("w", "urn:schemas-microsoft-com:office:word")
				.Xmlns("http://www.w3.org/TR/REC-html40"), () => {
					w.Head(() => {
						w.HeadTitle(Title);
						w.HeadMeta(a => a.HttpEquiv("content-type").Content("text/html; charset=utf-8"));
						w.Write(@"
<!--[if gte mso 9]>
<xml>
 <w:WordDocument>
  <w:View>Print</w:View>
  <w:Zoom>100</w:Zoom>
 </w:WordDocument>
</xml>
<![endif]-->
<style>
<!--
@page Section1 {" + (IsLandscape ? landscape : portrait) + @"}
div.Section1 {page:Section1;mso-fareast-font-family:'Times New Roman'}
table.bordered { border-collapse: collapse }
table.bordered td { border:solid 1px black; padding:0.1cm; }
-->
</style>
					");
					});
					w.Body(() => {
						w.Div(a => a.Class("Section1"), () => Body?.Invoke(w));
					});
				});
		}
	}

	public class ExcelRenderer
	{
		public string Title { get; protected set; }
		public Action<LayoutWriter> Body { get; set; }

		public void Render(LayoutWriter w)
		{
			w.DocType();
			w.Html(a => a.Xmlns("v", "urn:schemas-microsoft-com:office:vml")
				.Xmlns("o", "urn:schemas-microsoft-com:office:office")
				.Xmlns("x", "urn:schemas-microsoft-com:office:excel")
				.Xmlns("http://www.w3.org/TR/REC-html40"), () => {
				w.Head(() => {
					w.HeadTitle(Title);
					w.HeadMeta(a => a.HttpEquiv("content-type").Content("text/html; charset=utf-8"));
				});
				w.Body(() => Body?.Invoke(w));
			});
		}
	}


	public class WordResult : HttpResult
	{
		public WordResult(string fileName, WordRenderer renderer)
		{
			ContentType = "application/msword";
			fileName = fileName.Replace(" ", "_");
			if (!fileName.ToLower().EndsWith(".doc")) fileName += ".doc";
			Headers.Add("content-disposition", "attachment; filename=\"" + WebUtility.UrlEncode(fileName) + "\"");
			ContentFunc = ctx => {
				var w = new LayoutWriter(ctx);
				renderer.Render(w);
				return Encoding.UTF8.GetBytes(w.ToString());
			};
		}
	}

	public class ExcelResult : HttpResult
	{
		public ExcelResult(string fileName, ExcelRenderer renderer)
		{
			ContentType = "application/msexcel";
			fileName = fileName.Replace(" ", "_");
			if (!fileName.ToLower().EndsWith(".xls")) fileName += ".xls";
			Headers.Add("content-disposition", "attachment; filename=\"" + WebUtility.UrlEncode(fileName) + "\"");
			ContentFunc = ctx => {
				var w = new LayoutWriter(ctx);
				renderer.Render(w);
				return Encoding.UTF8.GetBytes(w.ToString());
			};
		}
	}
}
