using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web.Model;
using System.IO;
using System.Web.UI.WebControls;
using System.Data.Linq;
using Nephrite.Web.SPM;
using Nephrite.Web.FileStorage;

namespace Nephrite.Web
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
			int id = query.GetQueryParameter("oid").ToInt32(0);
			//N_File f = null;
			Guid? g = null;
			if (id > 0)
			{
				g = AppWeb.DataContext.N_Files.Where(f1 => f1.FileID == id).Select(o => o.Guid).SingleOrDefault();
			}
			string path = HttpUtility.UrlDecode(query.GetQueryParameter("path"));
			if (!String.IsNullOrEmpty(path))
			{
				int i = path.LastIndexOf('/');
				string dir = path.Substring(0, i);
				string file = path.Substring(i + 1);

				g = (from files in AppWeb.DataContext.N_Files
					 where files.Title == file && files.N_Folder.FullPath == dir
					 select files.Guid).SingleOrDefault();
			}
			IDbFile dbFile = null;
			if (g != null)
				dbFile = FileStorageManager.GetFile(g.Value);

			string guid = query.GetQueryParameter("guid");
			if (!String.IsNullOrEmpty(guid))
				dbFile = FileStorageManager.GetFile(guid.ToGuid());
			
			if (dbFile != null)
			{
				if (!N_FolderSPMContext.Current.Check(dbFile.SPMActionItemGUID, 2, dbFile, true))
				//if (!SPM2.Check(2, dbFile.SPMActionItemGUID, dbFile, true))
				{
					contentType = N_FolderSPMContext.Current.GetLastMessage();
					fileName = null;
					data = null;
					return false;
				}
				fileName = dbFile.Title;
				data = dbFile.GetBytes();
				contentType = GetContentType(dbFile.Extension);

				if (logDownload)
				{
					N_DownloadLog l = new N_DownloadLog();
					AppWeb.DataContext.N_DownloadLogs.InsertOnSubmit(l);
					l.LastModifiedDate = DateTime.Now;
					l.LastModifiedUserID = Subject.Current.ID;
					l.FileID = AppWeb.DataContext.N_Files.Where(o => o.Guid == dbFile.ID).Select(o => o.FileID).Single();
					l.IP = ip;
					AppWeb.DataContext.SubmitChanges();
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
