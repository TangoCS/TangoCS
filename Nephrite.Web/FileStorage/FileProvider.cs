using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.UI.WebControls;

using Nephrite.Identity;
using Nephrite.Multilanguage;
using Nephrite.Http;
using Nephrite.MVC;
using Nephrite.Web;
using Microsoft.Framework.DependencyInjection;

namespace Nephrite.FileStorage
{
	public class FileProvider
	{
		public static bool GetFile(HttpContext context, out byte[] data, out string fileName, out string contentType)
		{
			return GetFile(context.Request.Url.Query, context.Request.UserHostAddress, false, out data, out fileName, out contentType);
		}
		public static bool GetFile(HttpContext context, bool logDownload, out byte[] data, out string fileName, out string contentType)
		{
			return GetFile(context.Request.Url.Query, context.Request.UserHostAddress, logDownload, out data, out fileName, out contentType);
		}
		public static bool GetFile(string query, string ip, bool logDownload, out byte[] data, out string fileName, out string contentType)
		{
			string ownerguid = HttpContext.Current.Request.QueryString.Get("o");
			string guid = HttpContext.Current.Request.QueryString.Get("guid");
			string path = HttpUtility.UrlDecode(HttpContext.Current.Request.QueryString.Get("path"));
			IStorageFile file = null;
			IStorageFolder folder = null;
			var c = DI.RequestServices.GetService<IStorage<string>>();

			if (!String.IsNullOrEmpty(path))
			{
				folder = c.GetFolder(path);
			}

			if (!String.IsNullOrEmpty(ownerguid))
			{
				folder = c.GetFolder(ownerguid);				
			}
			if (folder != null) file = folder.GetFile(guid.ToGuid());

			if (file != null)
			{
				//if (!N_FolderSPMContext.Current.Check(dbFile.SPMActionItemGUID, 2, dbFile, true))
				//if (!SPM2.Check(2, dbFile.SPMActionItemGUID, dbFile, true))
				//{
				//	contentType = N_FolderSPMContext.Current.GetLastMessage();
				//	fileName = null;
				//	data = null;
				//	return false;
				//}
				fileName = file.Name;
				data = file.ReadAllBytes();
				contentType = GetContentType(file.Extension);

				if (logDownload)
				{
					IDC_FileStorage dc = (IDC_FileStorage)A.Model;
					IN_DownloadLog l = dc.NewIN_DownloadLog();
					dc.IN_DownloadLog.InsertOnSubmit(l);
					l.LastModifiedDate = DateTime.Now;
					l.LastModifiedUserID = Subject.Current.ID;
					l.FileGUID = file.ID;
					l.IP = ip;
					dc.SubmitChanges();
				}

				return true;
			}
			fileName = null;
			data = null;
			contentType = null;
			return false;
		}

		static string GetContentType(string ext)
		{
			switch (ext)
			{
				case ".css":
					return "text/css";
				case ".swf":
					return "application/x-shockwave-flash";
				case ".xls":
					return "application/vnd.ms-excel";
				case ".xlsx":
					return "application/vnd.ms-excel";
				default:
					return "application/octet-stream";
			}

		}

		//public static string GetFileTemporary(string query)
		//{
		//	byte[] data;
		//	string fileName;
		//	string contentType;
		//	if (GetFile(query, "", false, out data, out fileName, out contentType))
		//	{
		//		string fn = Path.GetTempFileName() + Path.GetExtension(fileName);
		//		File.WriteAllBytes(fn, data);
		//		return fn;
		//	}
		//	return "";
		//}

		//public static string GeneratePassword()
		//{
		//	string chars = "1234567890abcdefghijklmnopqrstuvwxyz";
		//	Random r = new Random();
		//	string pass = "";
		//	int length = 10 + r.Next(10);
		//	for (int i = 0; i < length; i++)
		//		pass += chars[r.Next(chars.Length)];
		//	return pass;
		//}
	}

	public enum SaveFileResult
	{
		OK,
		Oversize,
		WrongExt,
		AntiViralFailure,
		AntiViralSuspicion
	}

	//public static class SaveFileResultExtensionMethods
	//{
	//	public static string ToText(this SaveFileResult r)
	//	{
	//		return TextResource.Get("SaveFileResult." + r.ToString());
	//	}
	//}
}
