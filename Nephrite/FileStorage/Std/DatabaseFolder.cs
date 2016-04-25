using System;
using System.Linq;
using Nephrite.Data;
using System.Collections.Generic;
using System.IO;

namespace Nephrite.FileStorage.Std
{
	public class DatabaseFolder : IStorageFolder
	{
		public string ID { get; set; }
		public int MaxFileSize { get; set; }

		IDC_FileDataStorage _dc;

		public DatabaseFolder(IDC_FileDataStorage dataContext)
		{
			_dc = dataContext;
		}

		public IStorageFile CreateFile(string id)
		{
			return new DatabaseFile(_dc, ID.ToGuid()) { ID = id };
		}

		public IEnumerable<IStorageFile> GetFiles()
		{
			var data = _dc.IDbFileData;
			var id = Guid.Empty;
			if (!ID.IsEmpty())
			{
				id = ID.ToGuid();
				data = data.Where(o => o.Owner == id);
			}
			return from o in data
				   select new DatabaseFile(_dc, id) {
					   ID = Convert.ToString(o.FileGUID),
					   Name = o.Title,
					   Extension = o.Extension,
					   Length = o.Size,
					   LastModifiedDate = o.LastModifiedDate
				   };
		}

		public IStorageFile GetFile(string id)
		{
			var data = _dc.IDbFileData;
			var owner = Guid.Empty;
			Guid guid = Guid.Empty;
			
			if (!ID.IsEmpty())
			{
				owner = ID.ToGuid();
				data = data.Where(o => o.Owner == owner);
			}

			if (Guid.TryParse(id, out guid))
			{
				data = data.Where(o => o.FileGUID == guid);
			}
			else
			{
				data = data.Where(o => o.Title.ToLower() == id.ToLower());
			}

			return (from o in data
					select new DatabaseFile(_dc, owner) {
					   ID = Convert.ToString(o.FileGUID),
					   Name = o.Title,
					   Extension = o.Extension,
					   Length = o.Size,
					   LastModifiedDate = o.LastModifiedDate
				   }).FirstOrDefault();
		}

		public IEnumerable<IStorageFolder> GetFolders()
		{
			throw new NotSupportedException();
		}

		public IStorageFile GetOrCreateFile(string id)
		{
			var file = GetFile(id);
			if (file == null)
			{
				
				Guid guid;
				if (Guid.TryParse(id, out guid))
				{
					file = CreateFile(id);
				}
				else
				{
					file = CreateFile(Guid.NewGuid().ToString());
					file.Name = id;
					file.Extension = Path.GetExtension(id);
					file.LastModifiedDate = DateTime.Now;
				}
			}
			return file;
		}
	}

	public class DatabaseFile : IStorageFile
	{
		public string ID { get; set; }

		public string Name { get; set; }
		public string Extension { get; set; }
		public DateTime LastModifiedDate { get; set; }
		public long Length { get; set; }

		IDC_FileDataStorage _dc;
		Guid _owner;

		public DatabaseFile(IDC_FileDataStorage dataContext, Guid owner)
		{
			_dc = dataContext;
			_owner = owner;
		}

		public void Delete()
		{
			_dc.DeleteAllOnSubmit(_dc.IDbFileData.Where(o => o.FileGUID == ID.ToGuid()));
		}

		public byte[] ReadAllBytes()
		{
			var fd = _dc.IDbFileData.FirstOrDefault(o => o.FileGUID == ID.ToGuid());
			if (fd == null || fd.Data == null)
				return new byte[0];
			return fd.Data;
		}

		public void WriteAllBytes(byte[] bytes)
		{
			var id = ID.ToGuid();
			var fileData = _dc.IDbFileData.FirstOrDefault(o => o.FileGUID == id);
			if (fileData == null)
			{
				fileData = _dc.NewIDbFileData(id);
				_dc.InsertOnSubmit(fileData);
			}

			fileData.FileGUID = id;
			fileData.Title = Name;
			fileData.Size = bytes.Length;
			fileData.Data = bytes;
			fileData.Extension = Extension;
			if (_owner != Guid.Empty)
				fileData.Owner = _owner;
			else
				fileData.Owner = null;
			fileData.LastModifiedDate = DateTime.Now;
		}
	}

	public interface IDC_FileDataStorage : IDataContext
	{
		IDbFileData NewIDbFileData(Guid ID);
		IQueryable<IDbFileData> IDbFileData { get; }
	}

	public interface IDbFileData : IEntity
	{
		byte[] Data { get; set; }
		long Size { get; set; }
		string Title { get; set; }
		string Extension { get; set; }
		Guid FileGUID { get; set; }
		Guid? Owner { get; set; }
		DateTime LastModifiedDate { get; set; }
	}
}
