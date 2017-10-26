using System.Collections.Generic;
using System.Linq;

namespace Tango.FileStorage
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
		string FullPath { get; set; }
		int MaxFileSize { get; set; }

		IEnumerable<IStorageFolder<TKey>> GetFolders(int offset = 0, int limit = 50);

		IStorageFile<TKey> CreateFile(TKey id, string name);
		IStorageFile<TKey> GetOrCreateFile(TKey id, string name);
        IStorageFile<TKey> GetOrCreateFile(string name);

        IStorageFile<TKey> GetFile(TKey id);
		IStorageFile<TKey> GetFileByName(string name);

		bool HasFiles();
		IEnumerable<IStorageFile<TKey>> GetFiles(int offset = 0, int limit = 50);
	}
}