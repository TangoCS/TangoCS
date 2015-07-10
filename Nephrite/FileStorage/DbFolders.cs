using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Framework.DependencyInjection;
using Nephrite.Data;

namespace Nephrite.FileStorage
{
	public class DbFolders : IStorage<Guid>, IStorage<string>, IStorage<int>
	{
		IDbConnection _connection;		 

		public DbFolders(IDbConnection connection)
		{
			_connection = connection;
		}

		string selectFolder = @"select f.guid, f.title, f.path, fl.storagetype,
fl.maxfilesize, flt.extensions, flt.classname as typename, flt.title as typedescription
from n_folder f, n_filelibrary fl, n_filelibrarytype flt 
where f.filelibraryid = fl.filelibraryid and fl.filelibrarytypeid = flt.filelibrarytypeid";

		public IStorageFolder GetFolder(Guid folderKey)
		{
			var res = _connection.Query(selectFolder + " and f.guid = @p1", new { p1 = folderKey }).FirstOrDefault();
			return Parse(res);
		}

		public IStorageFolder GetFolder(string folderKey)
		{
			var res = _connection.Query(selectFolder + " and f.path = @p1", new { p1 = folderKey }).FirstOrDefault();
			return Parse(res);
		}

		public IStorageFolder GetFolder(int folderKey)
		{
			var res = _connection.Query(selectFolder + " and f.folderid = @p1", new { p1 = folderKey }).FirstOrDefault();
			return Parse(res);
		}

		IStorageFolder Parse(dynamic res)
		{
			if (res == null) return null;

			VirtualFolder f = null;
			string t = res.storagetype;
			if (t == "B") f = new VirtualFolder(new DatabaseStorageProvider(DI.RequestServices.GetService<IDC_FileDataStorage>()));
			if (t == "D") f = new VirtualFolder(new LocalDiskStorageProvider(res.path));
			if (f == null) return null;

			f.ID = res.guid;
			f.Name = res.title;
			f.Type = new StorageFolderType
			{
				Description = res.typedescription,
				Name = res.typename,
				AllowedExtensions = ((string)res.extensions).Split(new char[] { ',' }).Select(o => o.Replace("*", "")).ToList()
			};
			f.MaxFileSize = res.maxfilesize;
			

			return f;
		}

		public IEnumerable<IStorageFolder> GetFolders(IStorageFolder parentFolder)
		{
			IEnumerable<dynamic> folders = null;

			if (parentFolder != null)
			{
				folders = _connection.Query(@"select f.guid, f.title, f.path, fl.storagetype,
fl.maxfilesize, flt.extensions, flt.classname as typename, flt.title as typedescription
from n_folder f, n_filelibrary fl, n_filelibrarytype flt, n_folder p 
where f.filelibraryid = fl.filelibraryid and fl.filelibrarytypeid = flt.filelibrarytypeid and f.parentid = p.folderid and p.guid = @p1", new { p1 = parentFolder.ID });
			}
			else
			{
				folders = _connection.Query(@"select f.guid, f.title, f.path, fl.storagetype,
fl.maxfilesize, flt.extensions, flt.classname as typename, flt.title as typedescription
from n_folder f, n_filelibrary fl, n_filelibrarytype flt
where f.filelibraryid = fl.filelibraryid and fl.filelibrarytypeid = flt.filelibrarytypeid and f.parentid is null");
			}

			List<IStorageFolder> res = new List<IStorageFolder>();
			foreach (var folder in folders)
				res.Add(Parse(folder));

			return res;
		}
	}
}
