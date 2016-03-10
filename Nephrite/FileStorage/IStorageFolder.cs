using System.Collections.Generic;
using System.Linq;

namespace Nephrite.FileStorage
{
	public interface IStorageFolder
	{
		string ID { get; set; }
		int MaxFileSize { get; set; }

		IEnumerable<IStorageFolder> GetFolders();

		IStorageFile CreateFile(string id);
		IStorageFile GetFile(string id);
		IEnumerable<IStorageFile> GetFiles();
	}
}