using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web.FileStorage
{
	public interface IFileRepository
	{
		IQueryable<IDbItem> DbItems { get; }
		IQueryable<IDbFolder> DbFolders { get; }
		IQueryable<IDbFile> DbFiles { get; }
		IDbFolder CreateFolder(string title, string path);
		void DeleteFolder(Guid folderID);
		IDbFile CreateFile(string title, string path);
		void DeleteFile(Guid fileID);
		IDbFolder CreateFolder(string fullpath);
	}
}