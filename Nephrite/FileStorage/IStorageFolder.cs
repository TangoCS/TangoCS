using System.Collections.Generic;
using System.Linq;

namespace Nephrite.FileStorage
{
	public interface IStorageFolder
	{
		string Name { get; set; }
		int MaxFileSize { get; set; }

		IEnumerable<IStorageFolder> GetFolders();

		IStorageFile CreateFile(string id);
		IStorageFile GetFile(string id);
		IEnumerable<IStorageFile> GetFiles();
	}

	public interface IStorageFolder<TKey> 
	{
		TKey ID { get; set; }
		int MaxFileSize { get; set; }

		IEnumerable<IStorageFolder<TKey>> GetFolders();

		IStorageFile<TKey> CreateFile(TKey id, string name);
		IStorageFile<TKey> GetOrCreateFile(TKey id, string name);
        IStorageFile<TKey> GetOrCreateFile(string name);

        IStorageFile<TKey> GetFile(TKey id);
		IStorageFile<TKey> GetFileByName(string name);
		IEnumerable<IStorageFile<TKey>> GetFiles();
	}
}