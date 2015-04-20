using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.MVC;


namespace Nephrite.Web.FileStorage
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

		public static string GetDataUrl(string path)
		{
			int i = path.LastIndexOf('/');
			string dir = path.Substring(0, i);
			string file = path.Substring(i + 1);

			var fi = (from files in FileStorageManager.DbFiles
					  from folders in FileStorageManager.DbFolders
					  where files.Title == file && files.ParentFolderID == folders.ID && folders.FullPath == dir
					  select new FileInfo { ID = files.ID, LastModifiedDate = files.LastModifiedDate }).SingleOrDefault();
			if (fi != null)
				return String.Format("/Data.ashx?guid={0}&timestamp={1}", fi.ID, fi.LastModifiedDate.Ticks);
			return "";
		}

		public static string GetDataUrl(Guid id)
		{

			var f = (from files in FileStorageManager.DbFiles
					 where files.ID == id
					 select new { files.ID, files.LastModifiedDate }).SingleOrDefault();
			if (f != null)
				return String.Format("/Data.ashx?guid={0}&timestamp={1}", f.ID, f.LastModifiedDate.Ticks);
			return "";
		}
	}
}
