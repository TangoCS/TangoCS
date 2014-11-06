using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Linq.Expressions;
using System.Data.Linq;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Text;

namespace Nephrite.Web.FileStorage
{
	public static class FileStorageManager
	{
		public static IQueryable<IDbItem> DbItems
		{
			get { return dc.IDbItem; }
		}

		public static event EventHandler<FileEventArgs> OnFileCreated;
		//public static event EventHandler<FileEventArgs> OnFileModified;
		public static event EventHandler<FileEventArgs> OnFileDeleted;

		public static event EventHandler<FolderEventArgs> OnFolderCreated;
		//public static event EventHandler<FolderEventArgs> OnFolderModified;
		public static event EventHandler<FolderEventArgs> OnFolderDeleted;

		[ThreadStatic]
		static Dictionary<string, IDbFolder> _folderCache;

		static Dictionary<string, IDbFolder> folderCache
		{
			get
			{
				if (HttpContext.Current != null)
				{
					if (HttpContext.Current.Items["FileRepositoryFolderCache"] == null)
						HttpContext.Current.Items["FileRepositoryFolderCache"] = new Dictionary<string, IDbFolder>();

					return (Dictionary<string, IDbFolder>)HttpContext.Current.Items["FileRepositoryFolderCache"];
				}
				else
				{
					if (_folderCache == null) _folderCache = new Dictionary<string, IDbFolder>();
					return _folderCache;
				}
			}
		}


		#region Работа с папками
		public static IQueryable<IDbFolder> DbFolders
		{
			get { return dc.IDbFolder; }
		}

		public static IDbFolder GetFolder(Guid folderID)
		{
			return dc.IDbFolder.FirstOrDefault(o => o.ID == folderID);
		}

		public static IDbFolder GetFolder(string path)
		{
			string folderTitle = path;
			string folderPath = null;
			var index = path.IndexOfAny(new char[] { '/', '\\' });
			if (index > 0)
			{
				folderPath = path.Substring(0, index);
				folderTitle = path.Substring(index + 1);
			}
			return dc.IDbFolder.FirstOrDefault(o => ((o.Path == folderPath && folderPath != null) || !o.ParentFolderID.HasValue) && o.Title == folderTitle);
		}

		public static IDbFolder CreateFolder(string fullpath)
		{
			return CreateFolder(Guid.NewGuid(), fullpath);
		}

		public static IDbFolder CreateFolder(Guid id, string fullpath)
		{
			if (folderCache.ContainsKey(fullpath.ToLower()))
				return folderCache[fullpath.ToLower()];
			//var items = App.DataContext.GetTable<Solution.Model.DbFolder>();
			var item = dc.IDbFolder.FirstOrDefault(o => o.FullPath == fullpath);
			if (item != null)
				return item;

			string title = fullpath.IndexOf('/') >= 0 ? fullpath.Substring(fullpath.LastIndexOf('/') + 1) : fullpath;
			item = dc.NewIDbFolder(id);
			folderCache.Add(fullpath.ToLower(), item);
			item.Title = title.IsEmpty() ? "new folder" : title;
			if (title != fullpath && fullpath.Substring(0, fullpath.LastIndexOf('/')) != "")
			{
				var pfolder = CreateFolder(fullpath.Substring(0, fullpath.LastIndexOf('/')));
				item.SetParentFolder(pfolder);

				var parameter = pfolder.GetStorageParameter();
				var type = pfolder.GetStorageType();
				if (type == FileStorageType.FileSystem)
					parameter = System.IO.Path.Combine(parameter, pfolder.Title);
				item.SetStorageInfo(type, parameter);
			}
			else
			{
				item.SetStorageInfo(FileStorageType.LocalDatabase, "");
			}
			dc.IDbFolder.InsertOnSubmit(item);
			item.CheckValid();

			if (OnFolderCreated != null)
				OnFolderCreated(null, new FolderEventArgs { Folder = item });

			return item;
		}
		public static IDbFolder CreateFolder(Guid id, Guid parentid)
		{
			var parent = GetFolder(parentid);
			if (parent == null) throw new Exception("Parent folder not found");

			var item = dc.NewIDbFolder(id);
			item.SetParentFolder(parent);
			item.SetStorageInfo(parent.GetStorageType(), "");
			dc.IDbFolder.InsertOnSubmit(item);
			//item.CheckValid();

			if (OnFolderCreated != null)
				OnFolderCreated(null, new FolderEventArgs { Folder = item });

			return item;
		}


		public static IDbFolder CreateFolder(string title, string path)
		{
			return CreateFolder(title + "/" + path);
		}

		public static void DeleteFolder(IDbFolder folder)
		{
			var folders = DbFolders.Where(o => o.ParentFolderID == folder.ID).ToList();
			foreach (var child in folders)
				DeleteFolder(child);

			var files = DbFiles.Where(o => o.ParentFolderID == folder.ID).ToList();
			foreach (var file in files)
				DeleteFile(file);

			dc.IDbFolder.DeleteOnSubmit(folder);
			if(OnFolderDeleted != null)
				OnFolderDeleted(null, new FolderEventArgs { Folder = folder });
		}

		/// <summary>
		/// Удалить папку
		/// </summary>
		public static void DeleteFolder(Guid folderID)
		{
			var folder = GetFolder(folderID);
			DeleteFolder(folder);
		}

		public static void DeleteFolder(string fullPath)
		{
			var folder = GetFolder(fullPath);
			if (folder != null)
				DeleteFolder(folder);
		}
		#endregion

		#region Работа с файлами
		public static IQueryable<IDbFile> DbFiles
		{
			get { return dc.IDbFile; }
		}

		public static IDbFile GetFile(Guid fileID)
		{
			return dc.IDbFile.FirstOrDefault(o => o.ID == fileID);
		}

		public static IDbFile GetFile(string fullPath)
		{
			var index = fullPath.LastIndexOfAny(new char[] { '/', '\\' });
			var path = index > 0 ? fullPath.Substring(0, index) : null;
			var title = fullPath.Substring(index + 1);
			if (path.IsEmpty())
				return dc.IDbFile.FirstOrDefault(o => o.ParentFolderID == null && o.Title == title);
			else
				return dc.IDbFile.FirstOrDefault(o => o.Path == path && o.Title == title);
		}

		public static IDbFile CreateFile(string title, string path)
		{
			return CreateFile(Guid.NewGuid(), title, path);
		}

		public static IDbFile CreateFile(Guid id, string title, string path)
		{
			var item = dc.NewIDbFile(id);
			item.Title = title;
			//item.ID = id;

			if (!path.IsEmpty())
			{
				var pfolder = CreateFolder(path);
				item.SetParentFolder(pfolder);
			}

			dc.IDbFile.InsertOnSubmit(item);

			if (OnFileCreated != null)
				OnFileCreated(null, new FileEventArgs { File = item });
			return item;
		}

		/// <summary>
		/// Удалить файл
		/// </summary>
		public static void DeleteFile(IDbFile file)
		{
			DeleteFileData(file);
			dc.IDbFile.DeleteOnSubmit(file);
			if (OnFileDeleted != null)
				OnFileDeleted(null, new FileEventArgs { File = file });
		}

		/// <summary>
		/// Удалить файл
		/// </summary>
		public static void DeleteFile(Guid fileID)
		{
			var file = GetFile(fileID);
			if (file != null)
				DeleteFile(file);
		}

		/// <summary>
		/// Удалить файл
		/// </summary>
		/// <param name="fullPath">Полный путь</param>
		public static void DeleteFile(string fullPath)
		{
			var file = GetFile(fullPath);
			if (file != null)
				DeleteFile(file);
		}

		/// <summary>
		/// Сериализация
		/// </summary>
		/// <param name="file">файл</param>
		/// <returns>Элемент XML</returns>
		public static XElement SerializeToXml(this IDbFile file)
		{
			var xe = new XElement("File");
			xe.Add(new XElement("Title", file.Title));
			xe.Add(new XElement("Path", file.Path));
			xe.Add(new XElement("Data", Convert.ToBase64String(file.GetBytes())));
			xe.Add(new XElement("ID", file.ID));
			if (file.FeatureGUID.HasValue)
				xe.Add(new XElement("FeatureGUID", file.FeatureGUID.Value));
			return xe;
		}

		public static IDbFile DeserializeFromXml(XElement element)
		{
			string title = (string)element.Element("Title");
			string path = (string)element.Element("Path");
			Guid id = (Guid)element.Element("ID");
			Guid? featureGuid = element.Element("FeatureGUID") == null ? (Guid?)null : (Guid)element.Element("FeatureGUID");
			byte[] data = Convert.FromBase64String(element.Element("Data").Value);
			var file = GetFile(id);
			if (file == null)
			{
				file = CreateFile(title, path);
				file.GetType().GetField("_ID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(file, id);
			}
			else
			{
				file.Title = title;
			}
			file.FeatureGUID = featureGuid;
			file.Write(data);
			return file;
		}

		static IDbFile WriteAll(string fullpath, bool submitChanges, Action<IDbFile> a)
		{
			var f = GetFile(fullpath);
			if (f == null)
			{
				int i = fullpath.LastIndexOf('/');
				string name = fullpath.Substring(i + 1);
				string path = i > 0 ? fullpath.Substring(0, i) : "";
				f = CreateFile(name, path);
			}
			a(f);
			if (!f.CheckValid())
				throw new Exception(f.GetValidationMessages().Select(o => o.Message).Join("\r\n"));
			if (submitChanges)
				dc.SubmitChanges();
			return f;
		}
		public static IDbFile WriteAllText(string fullpath, string text)
		{
			return WriteAll(fullpath, true, f => { f.WriteText(text); });
		}
		public static IDbFile WriteAllText(string fullpath, string text, bool submitChanges)
		{
			return WriteAll(fullpath, submitChanges, f => { f.WriteText(text); });
		}

		public static IDbFile WriteAllBytes(string fullpath, byte[] data)
		{
			return WriteAll(fullpath, true, f => { f.Write(data); });
		}

		public static IDbFile WriteAllBytes(string fullpath, byte[] data, bool submitChanges)
		{
			return WriteAll(fullpath, submitChanges, f => { f.Write(data); });
		}
		#endregion

		#region Работа с данными
		static IDC_FileStorage dc { get { return (IDC_FileStorage)A.Model; } }

		public static void StoreData(IDbFile file, byte[] bytes)
		{
			// Определить тип хранения файла
			switch (file.GetStorageType())
			{
				case FileStorageType.LocalDatabase:
					StoreInLocalDatabase(file.ID, bytes, file.Extension);
					break;
				case FileStorageType.RemoteDatabase:
					StoreInRemoteDatabase(file.ID, file.Extension, bytes, file.GetStorageParameter());
					break;
				case FileStorageType.FileSystem:
					StoreInFileSystem(file, bytes);
					break;
				case FileStorageType.External:
					throw new NotImplementedException();
			}
		}

		static void StoreInLocalDatabase(Guid fileGuid, byte[] bytes, string extension)
		{
			var fileData = dc.IDbFileData.SingleOrDefault(o => o.FileGUID == fileGuid);
			if (fileData == null)
			{
				fileData = dc.NewIDbFileData(fileGuid);
				dc.IDbFileData.InsertOnSubmit(fileData);
			}

			fileData.Data = bytes;
			fileData.Extension = extension;
		}

		static void StoreInRemoteDatabase(Guid fileGuid, string extension, byte[] bytes, string connectionString)
		{
			using (var remoteDC = (IDC_FileStorage)A.Model.NewDataContext(connectionString))
			{
				var fileData = remoteDC.IDbFileData.SingleOrDefault(o => o.FileGUID == fileGuid);
				if (fileData == null)
				{
					fileData = remoteDC.NewIDbFileData(fileGuid);
					remoteDC.IDbFileData.InsertOnSubmit(fileData);
				}

				fileData.Data = bytes;
				fileData.Extension = extension;
				remoteDC.SubmitChanges();
			}
		}

		static void StoreInFileSystem(IDbFile file, byte[] bytes)
		{
			//var origin = ((ITable)dc.IDbFile).GetOriginalEntityState(file) as IDbFile;
			string fullFilePath = GetFsFullPath(file);
			//if (origin != null)
			//{
			//	string originFullPath = GetFsFullPath(origin);
			//	if (originFullPath != fullFilePath && File.Exists(originFullPath) && !File.Exists(fullFilePath))
			///		File.Move(originFullPath, fullFilePath);
			//}
			
			File.WriteAllBytes(fullFilePath, bytes);
		}

		static string GetFsFullPath(IDbFile file)
		{
			string fullFilePath = file.GetStorageParameter();
			if (fullFilePath == null)
				return null;
			if (fullFilePath.StartsWith("~"))
				fullFilePath = HttpContext.Current.Server.MapPath(fullFilePath);
			fullFilePath = System.IO.Path.Combine(fullFilePath, file.Path.Substring(file.Path.LastIndexOf('/') + 1).Replace("/", "\\"));
			Directory.CreateDirectory(fullFilePath);
			string fileName = Regex.Replace(file.Title, "[" + Regex.Escape(
				new string(Path.GetInvalidFileNameChars())) + "]", "_");
			if (Path.Combine(fullFilePath, fileName).Length >= 230)
			{
				return Path.Combine(fullFilePath, file.ID.ToString() + file.Extension);
			}
			else
				return Path.Combine(fullFilePath, fileName);
		}

		public static byte[] GetBytes(IDbFile file)
		{
			switch (file.GetStorageType())
			{
				case FileStorageType.LocalDatabase:
					return GetBytesFromDC(dc, file.ID);
				case FileStorageType.RemoteDatabase:
					using (var rdc = (IDC_FileStorage)A.Model.NewDataContext(file.GetStorageParameter())) return GetBytesFromDC(rdc, file.ID);
				case FileStorageType.FileSystem:
					return GetBytesFromFileSystem(file);
				case FileStorageType.External:
					throw new NotImplementedException();
				default:
					return null;
			}
		}

		static byte[] GetBytesFromDC(IDC_FileStorage dc, Guid fileGuid)
		{
			var fd = dc.IDbFileData.FirstOrDefault(o => o.FileGUID == fileGuid);
			if (fd == null || fd.Data == null)
				return new byte[0];
			return fd.Data;
		}

		static byte[] GetBytesFromFileSystem(IDbFile file)
		{
			string fullFilePath = GetFsFullPath(file);
			if (!File.Exists(fullFilePath))
				return new byte[0];
			return File.ReadAllBytes(fullFilePath);
		}

		static void DeleteFileData(IDbFile file)
		{
			switch (file.GetStorageType())
			{
				case FileStorageType.LocalDatabase:
					dc.IDbFileData.DeleteAllOnSubmit(dc.IDbFileData.Where(o => o.FileGUID == file.ID));
					break;
				case FileStorageType.RemoteDatabase:
					using (var rdc = (IDC_FileStorage)A.Model.NewDataContext(file.GetStorageParameter()))
					{
						rdc.IDbFileData.DeleteAllOnSubmit(dc.IDbFileData.Where(o => o.FileGUID == file.ID));
						rdc.SubmitChanges();
					}
					break;
				case FileStorageType.FileSystem:
					string fullFilePath = file.GetStorageParameter();
					if (fullFilePath.StartsWith("~"))
						fullFilePath = HttpContext.Current.Server.MapPath(fullFilePath);
					fullFilePath = System.IO.Path.Combine(fullFilePath, file.Path.Substring(file.Path.LastIndexOf('/') + 1).Replace("/", "\\"));
					var fullFilePath2 = Path.Combine(fullFilePath, file.Title);

					if (File.Exists(fullFilePath2))
						File.Delete(fullFilePath2);
					break;
				case FileStorageType.External:
					throw new NotImplementedException();
			}
		}
		#endregion
	}

	public class FileEventArgs : EventArgs
	{
		public IDbFile File {get; internal set;}
	}

	public class FolderEventArgs : EventArgs
	{
		public IDbFolder Folder {get; internal set;}
	}
}