using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace Nephrite.FileStorage.Std
{
	public class MixedDatabaseFolder : IStorageFolder
	{
		public string ID { get; set; }
		public int MaxFileSize { get; set; }

		public string Type { get; set; }
		public string Parameter { get; set; }

		static string selectFolder = @"select f.guid, f.title, f.path, fl.storagetype,
fl.maxfilesize, flt.extensions, flt.classname as typename, flt.title as typedescription
from n_folder f, n_filelibrary fl, n_filelibrarytype flt 
where f.filelibraryid = fl.filelibraryid and fl.filelibrarytypeid = flt.filelibrarytypeid";

		IDC_FileDataStorage _dc;
		MixedDatabaseFolderIdType _idType;
		IStorageFolder _realFolder;

		public MixedDatabaseFolder(IDC_FileDataStorage dc, MixedDatabaseFolderIdType idType = MixedDatabaseFolderIdType.GuidKey)
		{
			_dc = dc;
			_idType = idType;
		}
		
		IStorageFolder GetRealFolder()
		{
			if (_realFolder != null) return _realFolder;

			dynamic res = null;
			switch (_idType)
			{
				case MixedDatabaseFolderIdType.IntKey:
					res = _dc.Connection.Query(selectFolder + " and f.folderid = @p1", new { p1 = ID.ToInt32() }).FirstOrDefault();
					break;
				case MixedDatabaseFolderIdType.GuidKey:
					res = _dc.Connection.Query(selectFolder + " and f.guid = @p1", new { p1 = ID.ToGuid() }).FirstOrDefault();
					break;
				case MixedDatabaseFolderIdType.Path:
					res = _dc.Connection.Query(selectFolder + " and f.path = @p1", new { p1 = ID }).FirstOrDefault();
					break;
				default:
					break;
			}

			if (res == null) return null;

			string t = res.storagetype;
			string path = res.path;
			int maxfilesize = res.maxfilesize;

			Type = t;
			Parameter = res.path;
			//AllowedExtensions = ((string)res.extensions).Split(new char[] { ',' }).Select(o => o.Replace("*", "")).ToList()
			MaxFileSize = maxfilesize;

			if (Type == "B")
			{
				_realFolder = new DatabaseFolder(_dc);
			}
			else if (Type == "D")
			{
				_realFolder = new LocalDiskFolder(Parameter);
			}
			else
				return null;

			_realFolder.ID = ID;
			_realFolder.MaxFileSize = MaxFileSize;

			return _realFolder;
		}

		public IStorageFile CreateFile(string id)
		{		
			return GetRealFolder()?.CreateFile(id);
		}

		public IStorageFile GetFile(string id)
		{
			return GetRealFolder()?.GetFile(id);
		}

		public IEnumerable<IStorageFile> GetFiles()
		{
			return GetRealFolder()?.GetFiles();
		}

		public IEnumerable<IStorageFolder> GetFolders()
		{
			IEnumerable<dynamic> folders = null;

			if (!ID.IsEmpty())
			{
				folders = _dc.Connection.Query(@"select f.guid, f.title, f.path, fl.storagetype,
fl.maxfilesize, flt.extensions, flt.classname as typename, flt.title as typedescription
from n_folder f, n_filelibrary fl, n_filelibrarytype flt, n_folder p 
where f.filelibraryid = fl.filelibraryid and fl.filelibrarytypeid = flt.filelibrarytypeid and f.parentid = p.folderid and p.guid = @p1", new { p1 = ID.ToGuid() });
			}
			else
			{
				folders = _dc.Connection.Query(@"select f.guid, f.title, f.path, fl.storagetype,
fl.maxfilesize, flt.extensions, flt.classname as typename, flt.title as typedescription
from n_folder f, n_filelibrary fl, n_filelibrarytype flt
where f.filelibraryid = fl.filelibraryid and fl.filelibrarytypeid = flt.filelibrarytypeid and f.parentid is null");
			}

			List<IStorageFolder> res = new List<IStorageFolder>();
			foreach (var folder in folders)
			{
				var f = new MixedDatabaseFolder(_dc);

				string id = folder.guid;
				string t = folder.storagetype;
				string path = folder.path;
				int maxfilesize = folder.maxfilesize;

				f.ID = id;
				f.MaxFileSize = maxfilesize;
				f.Parameter = path;
				f.Type = t;

				res.Add(f);
			}

			return res;
		}
	}

	public enum MixedDatabaseFolderIdType
	{
		IntKey,
		GuidKey,
		Path
	}
}
