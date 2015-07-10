using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nephrite.Data;

namespace Nephrite.FileStorage
{
	public class StorageFolderType : IStorageFolderType
	{
		public List<string> AllowedExtensions { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}

	public class DefaultStorage : IStorage<string>
	{
		Dictionary<string, IStorageProvider> _cache = new Dictionary<string, IStorageProvider>();

		public void Add(string folderKey, IStorageProvider provider)
		{
			_cache.Add(folderKey, provider);
		}

		public IStorageFolder GetFolder(string folderKey)
		{
			return new VirtualFolder(_cache.Get(folderKey));
		}

		public IEnumerable<IStorageFolder> GetFolders(IStorageFolder parentFolder)
		{
			return null;
		}
	}

	public class VirtualFolder : IStorageFolder
	{
		public IStorageProvider Provider { get; private set; }

		public Guid ID { get; set; }
		public string Name { get; set; }

		public IVirusChecker VirusChecker { get; private set; }

		public int MaxFileSize { get; set; }
		public IStorageFolderType Type { get; set; }

		public VirtualFolder(IStorageProvider provider, IVirusChecker virusChecker = null)
		{
			Provider = provider;
			VirusChecker = virusChecker;
			Type = new StorageFolderType();
		}

		public IStorageFile CreateFile()
		{
			return CreateFile(Guid.NewGuid());
		}

		public IStorageFile CreateFile(Guid id)
		{
			var file = new StorageFile(this);
			file.ID = id;
			return file;
		}

		public IStorageFile CreateFile(string fullName)
		{
			var file = new StorageFile(this);
			int i = fullName.LastIndexOf(".");
			file.ID = Guid.NewGuid();
            file.Name = fullName;
			file.Extension = fullName.Substring(i);
			return file;
		}

		public IStorageFile GetFile(Guid id)
		{
			var file = CreateFile(id);
			return Provider.GetMetadata(file);	
		}

		public IStorageFile GetFile(string fullName)
		{
			var file = CreateFile(fullName);
			return Provider.GetMetadata(file);
		}


		public IQueryable<IStorageFile> GetAllFiles()
		{
			return Provider.GetAllMetadata(this);
		}
	}

	public class StorageFile : IStorageFile
	{
		public IStorageFolder Folder { get; private set; }

		public Guid ID { get; set; }
		public string Name { get; set; }
		public string Extension { get; set; }
		public long	Length { get; set; }
		public DateTime LastModifiedDate { get; set; }

		public StorageFile(IStorageFolder folder)
		{
			Folder = folder;
		}
	}

	public interface IDC_FileStorage : IDataContext
	{
		ITable<IN_DownloadLog> IN_DownloadLog { get; }
		IN_DownloadLog NewIN_DownloadLog();
	}

	public interface IN_DownloadLog : IEntity
	{
		int DownloadLogID { get; set; }
		int LastModifiedUserID { get; set; }
		Guid FileGUID { get; set; }
		bool IsDeleted { get; set; }
		DateTime LastModifiedDate { get; set; }
		string IP { get; set; }
	}
}
