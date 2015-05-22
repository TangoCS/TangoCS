using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.MVC;


namespace Nephrite.FileStorage
{
	public class DataHandler : IHttpHandler
	{
		class FileInfo
		{
			public Guid ID { get; set; }
			public DateTime LastModifiedDate { get; set; }
		}
		
		public bool IsReusable
		{
			get { return true; }
		}

		public void ProcessRequest(HttpContext context)
		{
			byte[] data;
			string fileName;
			string contentType;
			bool flag = FileProvider.GetFile(context, out data, out fileName, out contentType);
			if (!flag)
			{
				context.Response.Write("Файл удален или не существует");
				context.Response.End();
			}

			context.Response.ContentType = contentType;

			if (!context.Request.QueryString["timestamp"].IsEmpty())
				context.Response.Expires = 50000;
			//context.Response.AppendHeader("content-disposition", "Attachment; FileName=\"" + fileName + "\"");
			context.Response.OutputStream.Write(data, 0, data.Length);

			context.Response.End();
		}
	}
}
