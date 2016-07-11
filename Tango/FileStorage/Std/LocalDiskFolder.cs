using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tango.FileStorage.Std
{
	public class LocalDiskFolder : IStorageFolder
	{
		string _virtualPath;

		public LocalDiskFolder(string virtialPath)
		{
			_virtualPath = virtialPath;
		}

		public string Name { get; set; }
		public string FullPath { get; set; }
		public int MaxFileSize { get; set; }

		public IStorageFile CreateFile(string id)
		{
			return new LocalDiskFile(_virtualPath, id);
		}

		public IEnumerable<IStorageFile> GetFiles()
		{
			var di = new DirectoryInfo(LocalDiskUtils.GetLocalPath(_virtualPath));
			return from o in di.EnumerateFiles()
					  select new LocalDiskFile(_virtualPath, o.Name) {
						  Name = o.Name,
						  Extension = o.Extension,
						  LastModifiedDate = o.LastWriteTime
					  };
		}

		public IStorageFile GetFile(string id)
		{
			var fd = new FileInfo(LocalDiskUtils.GetLocalFullName(_virtualPath, id));
			if (fd == null) return null;

			var file = new LocalDiskFile(_virtualPath, id);
			file.Extension = fd.Extension;
			file.Name = fd.Name;
			file.LastModifiedDate = fd.LastWriteTime;

			return file;
		}

		public IEnumerable<IStorageFolder> GetFolders()
		{
			return Directory.GetDirectories(LocalDiskUtils.GetLocalPath(_virtualPath)).Select(o => 
				new LocalDiskFolder(_virtualPath + Path.DirectorySeparatorChar.ToString() + o)
			);
		}
	}

	public class LocalDiskFile : IStorageFile
	{
		string _virtualFolder;

		public string ID => LocalDiskUtils.GetVirtualFullName(_virtualFolder, Name);		
		public string Name { get; set; }
		public string Extension { get; set; }
		public long Length => new FileInfo(ID).Length;
		public DateTime LastModifiedDate { get; set; }

		public LocalDiskFile(string virtualFolder, string name)
		{
			_virtualFolder = virtualFolder;
			Name = name;
		}

		public void Delete()
		{
			File.Delete(ID);
		}

		public byte[] ReadAllBytes()
		{
			return File.ReadAllBytes(ID);
		}

		public void WriteAllBytes(byte[] bytes)
		{
			File.WriteAllBytes(ID, bytes);			
		}
	}

	static class LocalDiskUtils
	{
		static string regex = "[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]";

		public static string GetVirtualFullName(string virtualPath, string fileName)
		{
			return Path.Combine(virtualPath, Regex.Replace(fileName, regex, "_"));
		}

		public static string GetLocalFullName(string virtualPath, string fileName)
		{
			return Path.Combine(GetLocalPath(virtualPath), Regex.Replace(fileName, regex, "_"));
		}

		public static string GetLocalPath(string virtualPath)
		{
			string localPath = "";
			localPath = virtualPath.Replace("/", Path.DirectorySeparatorChar.ToString());
			localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, localPath);

			if (!Directory.Exists(localPath)) Directory.CreateDirectory(localPath);

			return localPath;
		}
	}
}
