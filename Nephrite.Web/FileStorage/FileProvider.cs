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

namespace Nephrite.Web.FileStorage
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
			var q = QueryHelpers.ParseQuery(query);
			string guid = q.Get("guid");
			string path = HttpUtility.UrlDecode(q.Get("path"));
			IDbFile dbFile = null;
		
			if (!String.IsNullOrEmpty(path))
				dbFile = FileStorageManager.GetFile(path);
			
			if (!String.IsNullOrEmpty(guid))
				dbFile = FileStorageManager.GetFile(guid.ToGuid());
			
			if (dbFile != null)
			{
				//if (!N_FolderSPMContext.Current.Check(dbFile.SPMActionItemGUID, 2, dbFile, true))
				//if (!SPM2.Check(2, dbFile.SPMActionItemGUID, dbFile, true))
				//{
				//	contentType = N_FolderSPMContext.Current.GetLastMessage();
				//	fileName = null;
				//	data = null;
				//	return false;
				//}
				fileName = dbFile.Title;
				data = dbFile.GetBytes();
				contentType = GetContentType(dbFile.Extension);

				if (logDownload)
				{
					IDC_FileStorage dc = (IDC_FileStorage)A.Model;
					IN_DownloadLog l = dc.NewIN_DownloadLog();
					dc.IN_DownloadLog.InsertOnSubmit(l);
					l.LastModifiedDate = DateTime.Now;
					l.LastModifiedUserID = Subject.Current.ID;
					l.FileGUID = dbFile.ID;
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

		public static string GetFileTemporary(string query)
		{
			byte[] data;
			string fileName;
			string contentType;
			if (GetFile(query, "", false, out data, out fileName, out contentType))
			{
				string fn = Path.GetTempFileName() + Path.GetExtension(fileName);
				File.WriteAllBytes(fn, data);
				return fn;
			}
			return "";
		}

		public static string GeneratePassword()
		{
			string chars = "1234567890abcdefghijklmnopqrstuvwxyz";
			Random r = new Random();
			string pass = "";
			int length = 10 + r.Next(10);
			for (int i = 0; i < length; i++)
				pass += chars[r.Next(chars.Length)];
			return pass;
		}
	}

	public enum SaveFileResult
	{
		OK,
		Oversize,
		WrongExt,
		AntiViralFailure,
		AntiViralSuspicion
	}

	public static class SaveFileResultExtensionMethods
	{
		public static string ToText(this SaveFileResult r)
		{
			return TextResource.Get("SaveFileResult." + r.ToString());
		}
	}
}
