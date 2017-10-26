using System;

namespace Tango.FileStorage
{
	public interface IStorageManager<TKey>
	{
		IStorageFolder<TKey> GetFolder(TKey id = default(TKey), string path = null);
	}

	public static class StorageManagerExtensions
	{
		public static IStorageFile<T> GetFile<T>(this IStorageManager<T> storage, T id, T folderid = default(T))
		{
			return storage.GetFolder(folderid).GetFile(id);
		}

		public static IStorageFile<T> CreateFile<T>(this IStorageFolder<T> folder, string name)
		{
			return folder.CreateFile(default(T), name);
		}
	}
}
