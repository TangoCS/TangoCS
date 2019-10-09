using System.Collections.Generic;
using System.Linq;
using Dapper;
using Tango.Data;
using System;

namespace Tango.FileStorage.Std
{
//	public class MixedDatabaseFolder : IStorageFolder<Guid>
//	{
//		public Guid ID { get; set; }
//		public int MaxFileSize { get; set; }

//		public string Type { get; set; }
//		public string Parameter { get; set; }

//		static string selectFolder = @"select f.guid, f.title, f.path, fl.storagetype,
//fl.maxfilesize, flt.extensions, flt.classname as typename, flt.title as typedescription
//from n_folder f, n_filelibrary fl, n_filelibrarytype flt 
//where f.filelibraryid = fl.filelibraryid and fl.filelibrarytypeid = flt.filelibrarytypeid";

//		IDataContext _dc;
//		MixedDatabaseFolderSearchType _idType;
//		IStorageFolder _realFolder;
//		string _searchString;

//		public MixedDatabaseFolder(IDataContext dc, string searchString, MixedDatabaseFolderSearchType searchType = MixedDatabaseFolderSearchType.ByGuidKey)
//		{
//			_dc = dc;
//			_idType = searchType;
//			_searchString = searchString;

//			CreateLocalFolder = p => new LocalDiskFolder(p);
//			CreateDatabaseFolder = d => new DatabaseFolder(d);
//		}

//		public Func<string, IStorageFolder> CreateLocalFolder;
//		public Func<IDataContext, IStorageFolder> CreateDatabaseFolder;




//		IStorageFolder GetRealFolder()
//		{
//			if (_realFolder != null) return _realFolder;

//			dynamic res = null;
//			switch (_idType)
//			{
//				case MixedDatabaseFolderSearchType.ByIntKey:
//					res = _dc.Connection.Query(selectFolder + " and f.folderid = @p1", new { p1 = _searchString.ToInt32() }).FirstOrDefault();
//					break;
//				case MixedDatabaseFolderSearchType.ByGuidKey:
//					res = _dc.Connection.Query(selectFolder + " and f.guid = @p1", new { p1 = _searchString.ToGuid() }).FirstOrDefault();
//					break;
//				case MixedDatabaseFolderSearchType.ByPath:
//					res = _dc.Connection.Query(selectFolder + " and f.path = @p1", new { p1 = _searchString }).FirstOrDefault();
//					break;
//				default:
//					break;
//			}

//			if (res == null) return null;

//			string t = res.storagetype;
//			string path = res.path;
//			int maxfilesize = res.maxfilesize;
//			Guid id = res.guid;

//			Type = t;
//			Parameter = res.path;
//			//AllowedExtensions = ((string)res.extensions).Split(new char[] { ',' }).Select(o => o.Replace("*", "")).ToList()
//			MaxFileSize = maxfilesize;

//			if (Type == "B")
//			{
//				_realFolder = CreateDatabaseFolder(_dc);
//			}
//			else if (Type == "D")
//			{
//				_realFolder = CreateLocalFolder(Parameter);
//			}
//			else
//				return null;

//			_realFolder.ID = id;
//			_realFolder.MaxFileSize = MaxFileSize;

//			return _realFolder;
//		}

//		public IStorageFile<Guid> CreateFile(Guid id)
//		{		
//			return GetRealFolder()?.CreateFile(id);
//		}

//		public IStorageFile<Guid> GetFile(Guid id)
//		{
//			return GetRealFolder()?.GetFile(id);
//		}

//		public IEnumerable<IStorageFile<Guid>> GetFiles()
//		{
//			return GetRealFolder()?.GetFiles();
//		}

//		public IEnumerable<IStorageFolder<Guid>> GetFolders()
//		{
//			IEnumerable<dynamic> folders = null;

//			if (!ID.IsEmpty())
//			{
//				folders = _dc.Connection.Query(@"select f.guid, f.title, f.path, fl.storagetype,
//fl.maxfilesize, flt.extensions, flt.classname as typename, flt.title as typedescription
//from n_folder f, n_filelibrary fl, n_filelibrarytype flt, n_folder p 
//where f.filelibraryid = fl.filelibraryid and fl.filelibrarytypeid = flt.filelibrarytypeid and f.parentid = p.folderid and p.guid = @p1", new { p1 = ID.ToGuid() });
//			}
//			else
//			{
//				folders = _dc.Connection.Query(@"select f.guid, f.title, f.path, fl.storagetype,
//fl.maxfilesize, flt.extensions, flt.classname as typename, flt.title as typedescription
//from n_folder f, n_filelibrary fl, n_filelibrarytype flt
//where f.filelibraryid = fl.filelibraryid and fl.filelibrarytypeid = flt.filelibrarytypeid and f.parentid is null");
//			}

//			List<IStorageFolder<Guid>> res = new List<IStorageFolder<Guid>>();
//			foreach (var folder in folders)
//			{
//				string id = folder.guid;
//				string t = folder.storagetype;
//				string path = folder.path;
//				int maxfilesize = folder.maxfilesize;

//				var f = new MixedDatabaseFolder(_dc, id, MixedDatabaseFolderSearchType.ByGuidKey);
//				f.MaxFileSize = maxfilesize;
//				f.Parameter = path;
//				f.Type = t;

//				res.Add(f);
//			}

//			return res;
//		}
//	}

//	public enum MixedDatabaseFolderSearchType
//	{
//		ByIntKey,
//		ByGuidKey,
//		ByPath
//	}
}
