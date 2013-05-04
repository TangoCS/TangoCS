using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq;

namespace Nephrite.Web
{
	public class DataHandler : IHttpHandler
	{
		class FileInfo
		{
			public int ID { get; set; }
			public DateTime LastModifiedDate { get; set; }
		}

		//static Func<Model.modelDataContext, string, string, FileInfo> getFileInfo;
		
		public bool IsReusable
		{
			get { return true; }
		}

		public void ProcessRequest(HttpContext context)
		{
			//if (provider == null)
			//{
			//    provider = (IFileProvider)Activator.CreateInstance(Settings.FileProviderAssembly, Settings.FileProviderClass).Unwrap();
			//}

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
			if (Query.GetString("timestamp") != "")
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

			//if (getFileInfo == null)
			//{
			//	getFileInfo = CompiledQuery.Compile<Model.modelDataContext, string, string, FileInfo>((db, f, d) =>
			//		(from files in db.N_Files
			//		 where files.Title == f && files.N_Folder.FullPath == d
			//		 select new FileInfo { ID = files.FileID, LastModifiedDate = files.LastModifiedDate }).SingleOrDefault());
			//}

			//var fi = getFileInfo(AppWeb.DataContext, file, dir);
			var fi = (from files in AppWeb.DataContext.N_Files
					  where files.Title == file && files.N_Folder.FullPath == dir
					  select new FileInfo { ID = files.FileID, LastModifiedDate = files.LastModifiedDate }).SingleOrDefault();
			if (fi != null)
				return String.Format("/Data.ashx?oid={0}&timestamp={1}", fi.ID, fi.LastModifiedDate.Ticks);
			return "";
		}

		public static string GetDataUrl(int id)
		{
			
			var f = (from files in AppWeb.DataContext.N_Files
					 where files.FileID == id
					 select new { files.FileID, files.LastModifiedDate }).SingleOrDefault();
			if (f != null)
				return String.Format("/Data.ashx?oid={0}&timestamp={1}", f.FileID, f.LastModifiedDate.Ticks);
			return "";
		}
	}
}
