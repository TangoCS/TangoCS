using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nephrite.Data;

namespace Nephrite.FileStorage
{
	public class LocalDiskStorageProvider : IStorageProvider
	{
		string _localPath;

		public LocalDiskStorageProvider(string localPath)
		{
			if (localPath.StartsWith("~"))
				localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, localPath.Substring(1));

			_localPath = localPath.Replace("/", Path.DirectorySeparatorChar.ToString());
		}

		public void SetData(IStorageFile file, byte[] bytes)
		{
			Directory.CreateDirectory(_localPath);
			var p = GetFullPath(file.Name, file.Extension);
			File.WriteAllBytes(p, bytes);
		}

		public byte[] GetData(IStorageFile file)
		{
			string p = GetFullPath(file.Name, file.Extension);
			if (!File.Exists(p)) return new byte[0];
			return File.ReadAllBytes(p);
		}

		public void DeleteData(IStorageFile file)
		{
			string p = GetFullPath(file.Name, file.Extension);
			if (File.Exists(p)) File.Delete(p);
		}

		public IStorageFile GetMetadata(IStorageFile file)
		{
			string p = GetFullPath(file.Name, file.Extension);
			var fd = new FileInfo(p);
			if (fd == null) return file;

			file.Extension = fd.Extension;
			file.Name = fd.Name;
			file.Length = fd.Length;
			file.LastModifiedDate = fd.LastWriteTime;
			
			return file;
		}

		string GetFullPath(string title, string extension)
		{
			string fileName = Regex.Replace(title, "[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]", "_");
			return Path.Combine(_localPath, fileName + extension);
		}

		public IEnumerable<IStorageFile> GetAllMetadata(IStorageFolder folder)
		{
			var di = new DirectoryInfo(_localPath);
			var res = from o in di.EnumerateFiles()
					  select new StorageFile(folder) { Extension = o.Extension, Name = o.Name, Length = o.Length, LastModifiedDate = o.LastWriteTime };
		
			return res;
		}
	}
}
