using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace Nephrite.FileStorage
{
	public class FileHandler : IHttpHandler
	{
		public bool IsReusable
		{
			get { return true; }
		}

		public void ProcessRequest(HttpContext context)
		{
			byte[] data;
			string fileName;
			string contentType;
			bool flag = FileProvider.GetFile(context, true, out data, out fileName, out contentType);
			if (!flag)
			{
				if (!contentType.IsEmpty())
					context.Response.Write(contentType);
				else
					context.Response.Write("Файл был удален, не существует или недостаточно полномочий.");
				context.Response.End();
			}

			if (context.Request.Browser.Browser == "IE" || context.Request.Browser.Browser == "InternetExplorer")
			{
				context.Response.HeaderEncoding = Encoding.Default;
				context.Response.AppendHeader("content-disposition", "Attachment; FileName=\"" + HttpUtility.UrlPathEncode(fileName) + "\"");
			}
			else
			{
				context.Response.HeaderEncoding = Encoding.UTF8;
				context.Response.AppendHeader("content-disposition", "Attachment; FileName=\"" + fileName + "\"");
			}
			context.Response.ContentType = contentType; // "application/octet-stream";
			
			
			context.Response.OutputStream.Write(data, 0, data.Length);

			context.Response.End();
		}
	}
}