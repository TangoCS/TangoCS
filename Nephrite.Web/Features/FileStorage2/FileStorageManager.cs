﻿using System;
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
		public static IFileRepository FileRepository { get; set; }

		public static IQueryable<IDbItem> DbItems
		{
			get { return FileRepository.DbItems; }
		}

		public static event EventHandler<FileEventArgs> OnFileCreated;
		public static event EventHandler<FileEventArgs> OnFileModified;
		public static event EventHandler<FileEventArgs> OnFileDeleted;

		public static event EventHandler<FolderEventArgs> OnFolderCreated;
		public static event EventHandler<FolderEventArgs> OnFolderModified;
		public static event EventHandler<FolderEventArgs> OnFolderDeleted;

		#region Работа с папками
		public static IQueryable<IDbFolder> DbFolders
		{
			get { return FileRepository.DbFolders; }
		}

		public static IDbFolder GetFolder(Guid folderID)
		{
			return FileRepository.DbFolders.FirstOrDefault(o => o.ID == folderID);
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
			return FileRepository.DbFolders.FirstOrDefault(o => ((o.Path == folderPath && folderPath != null) || !o.ParentFolderID.HasValue) && o.Title == folderTitle);
		}

		public static IDbFolder CreateFolder(string title, string path)
		{
			var folder = FileRepository.CreateFolder(title, path);
			if (OnFolderCreated != null)
				OnFolderCreated(FileRepository, new FolderEventArgs { Folder = folder });
			return folder;
		}

		public static void DeleteFolder(IDbFolder folder)
		{
			var folders = DbFolders.Where(o => o.ParentFolderID == folder.ID).ToList();
			foreach (var child in folders)
				DeleteFolder(child);

			var files = DbFiles.Where(o => o.ParentFolderID == folder.ID).ToList();
			foreach (var file in files)
				DeleteFile(file);

			FileRepository.DeleteFolder(folder.ID);
			if(OnFolderDeleted != null)
				OnFolderDeleted(FileRepository, new FolderEventArgs { Folder = folder });
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
			get { return FileRepository.DbFiles; }
		}

		public static IDbFile GetFile(Guid fileID)
		{
			return FileRepository.DbFiles.FirstOrDefault(o => o.ID == fileID);
		}

		public static IDbFile GetFile(string fullPath)
		{
			var index = fullPath.LastIndexOfAny(new char[] { '/', '\\' });
			var path = index > 0 ? fullPath.Substring(0, index) : null;
			var title = fullPath.Substring(index + 1);
			return FileRepository.DbFiles.FirstOrDefault(o => ((o.Path == path && !path.IsEmpty()) || (o.ParentFolderID == null && path.IsEmpty())) && o.Title == title);
		}

		public static IDbFile CreateFile(string title, string path)
		{
			IDbFile file = FileRepository.CreateFile(title, path);
			if (OnFileCreated != null)
				OnFileCreated(FileRepository, new FileEventArgs { File = file });
			return file;
		}

		/// <summary>
		/// Удалить файл
		/// </summary>
		public static void DeleteFile(IDbFile file)
		{
			DeleteFileData(file);
			FileRepository.DeleteFile(file.ID);
			if (OnFileDeleted != null)
				OnFileDeleted(FileRepository, new FileEventArgs { File = file });
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

		public static void WriteAllText(string fullpath, string text)
		{
			var f = GetFile(fullpath);
			if (f == null)
			{
				int i = fullpath.LastIndexOf('/');
				string name = fullpath.Substring(i + 1);
				string path = i > 0 ? fullpath.Substring(0, i) : "";
				f = CreateFile(name, path);
			}
			f.WriteText(text);
			if (!f.CheckValid())
				throw new Exception(f.GetValidationMessages().Select(o => o.Message).Join("\r\n"));
			DC.SubmitChanges();
		}
		#endregion

		#region Работа с данными
		static DataContext DC { get { return Nephrite.Web.Base.Model; } }

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
			var fileData = DC.GetTable<DbFileData>().SingleOrDefault(o => o.FileGUID == fileGuid);
			if (fileData == null)
			{
				fileData = new DbFileData
				{
					FileGUID = fileGuid
				};
				DC.GetTable<DbFileData>().InsertOnSubmit(fileData);
			}

			fileData.Data = new Binary(bytes);
			fileData.Extension = extension;
		}

		static void StoreInRemoteDatabase(Guid fileGuid, string extension, byte[] bytes, string connectionString)
		{
			using (var remoteDC = new DataContext(connectionString))
			{
				var fileData = remoteDC.GetTable<DbFileData>().SingleOrDefault(o => o.FileGUID == fileGuid);
				if (fileData == null)
				{
					fileData = new DbFileData
					{
						FileGUID = fileGuid
					};
					remoteDC.GetTable<DbFileData>().InsertOnSubmit(fileData);
				}

				fileData.Data = new Binary(bytes);
				fileData.Extension = extension;
				remoteDC.SubmitChanges();
			}
		}

		static void StoreInFileSystem(IDbFile file, byte[] bytes)
		{
			var origin = DC.GetTable(file.GetType()).GetOriginalEntityState(file) as IDbFile;
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
					return GetBytesFromDC(DC, file.ID);
				case FileStorageType.RemoteDatabase:
					using (var dc = new DataContext(file.GetStorageParameter())) return GetBytesFromDC(dc, file.ID);
				case FileStorageType.FileSystem:
					return GetBytesFromFileSystem(file);
				case FileStorageType.External:
					throw new NotImplementedException();
				default:
					return null;
			}
		}

		static byte[] GetBytesFromDC(DataContext dc, Guid fileGuid)
		{
			var fd = dc.GetTable<DbFileData>().FirstOrDefault(o => o.FileGUID == fileGuid);
			if (fd == null || fd.Data == null)
				return new byte[0];
			return fd.Data.ToArray();
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
					DC.GetTable<DbFileData>().DeleteAllOnSubmit(DC.GetTable<DbFileData>().Where(o => o.FileGUID == file.ID));
					break;
				case FileStorageType.RemoteDatabase:
					using (var dc = new DataContext(file.GetStorageParameter()))
					{
						dc.GetTable<DbFileData>().DeleteAllOnSubmit(dc.GetTable<DbFileData>().Where(o => o.FileGUID == file.ID));
						dc.SubmitChanges();
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