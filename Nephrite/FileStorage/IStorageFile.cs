using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Nephrite.Identity;
using Nephrite.Data;
using System.IO;

namespace Nephrite.FileStorage
{
	public interface IStorage<T>
	{
		IStorageFolder GetFolder(T folderKey);
		IEnumerable<IStorageFolder> GetFolders(IStorageFolder parentFolder);
	}

	public interface IStorageFolder
	{
		Guid ID { get; set; }
		string Name { get; set; }

		IStorageProvider Provider { get; }
		IVirusChecker VirusChecker { get; }

		int MaxFileSize { get; }
		IStorageFolderType Type { get; }

		IStorageFile CreateFile(Guid id);
		IStorageFile CreateFile(string fullName);
		IStorageFile GetFile(Guid id);
		IStorageFile GetFile(string fullName);
		IEnumerable<IStorageFile> GetAllFiles();
	}

	public interface IStorageFolderType
	{
		List<string> AllowedExtensions { get; }
		string Name { get; }
		string Description { get; }
	}

	public interface IStorageFile
	{
		Guid ID { get; set; }
		string Name { get; set; }
		string Extension { get; set; }
		long Length { get; set; }
		DateTime LastModifiedDate { get; set; }
	
		IStorageFolder Folder { get; }
	}

	public static class IStorageFileExtensions
	{
		public static byte[] ReadAllBytes(this IStorageFile file)
		{
			return file.Folder.Provider.GetData(file);
		}

		public static string ReadAllText(this IStorageFile file)
		{
			return file.Folder.Provider.GetText(file);
		}

		public static XDocument ReadAllXml(this IStorageFile file)
		{
			return file.Folder.Provider.GetXml(file);
		}

		public static void WriteAllBytes(this IStorageFile file, byte[] bytes)
		{
			file.Folder.Provider.SetData(file, bytes);
		}

		public static void WriteAllText(this IStorageFile file, string text, Encoding encoding = null)
		{
			file.Folder.Provider.SetText(file, text, encoding);
		}

		public static void WriteAllXml(this IStorageFile file, XDocument xml)
		{
			file.Folder.Provider.SetXml(file, xml);
		}

		public static void Delete(this IStorageFile file)
		{
			file.Folder.Provider.DeleteData(file);
		}
	}

	public static class IHierarchicStorageExtensions
	{
		public static IStorageFile GetFile(this IStorage<string> storage, string pathAndFullName)
		{
			int i = pathAndFullName.LastIndexOf("/");
			if (i == pathAndFullName.Length) return null;

			IStorageFolder folder = null;
			string fullName = "";
			if (i < 0)
			{
				folder = storage.GetFolder("");
				fullName = pathAndFullName;
			}
			else
			{
				string path = pathAndFullName.Substring(0, i);
				fullName = pathAndFullName.Substring(i + 1);
				folder = storage.GetFolder(path);
			}
			if (folder == null) return null;
			return folder.GetFile(fullName);
		}

		public static IStorageFile GetFile(this IStorage<string> storage, Guid id)
		{
			var folder = storage.GetFolder("");
			if (folder == null) return null;

			return folder.GetFile(id);
		}
	}

	
}