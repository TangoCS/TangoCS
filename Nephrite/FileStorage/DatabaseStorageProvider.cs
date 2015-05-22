using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nephrite.Data;

namespace Nephrite.FileStorage
{
	public class DatabaseStorageProvider : IStorageProvider
	{
		IDC_FileDataStorage _dc;

		public DatabaseStorageProvider(IDC_FileDataStorage dataContext)
		{
			_dc = dataContext;
		}

		public void SetData(IStorageFile file, byte[] bytes)
		{
			var fileData = _dc.IDbFileData.SingleOrDefault(o => o.FileGUID == file.ID);
			if (fileData == null)
			{
				fileData = _dc.NewIDbFileData(file.ID);
				_dc.IDbFileData.InsertOnSubmit(fileData);
			}

			fileData.Title = file.Name;
			fileData.Size = bytes.Length;
			fileData.Data = bytes;
			fileData.Extension = file.Extension;
			fileData.Owner = file.Folder.ID;
			fileData.LastModifiedDate = DateTime.Now;
		}

		public byte[] GetData(IStorageFile file)
		{
			var fd = _dc.IDbFileData.FirstOrDefault(o => o.FileGUID == file.ID);
			if (fd == null || fd.Data == null)
				return new byte[0];
			return fd.Data;
		}

		public void DeleteData(IStorageFile file)
		{
			_dc.IDbFileData.DeleteAllOnSubmit(_dc.IDbFileData.Where(o => o.FileGUID == file.ID));
		}

		public IStorageFile GetMetadata(IStorageFile file)
		{
			var fd = (from o in _dc.IDbFileData
					 where o.FileGUID == file.ID
					 select new { o.Extension, o.Title, o.Size, o.LastModifiedDate }).FirstOrDefault();
			if (fd == null) return file;

			file.Extension = fd.Extension;
			file.Name = fd.Title;
			file.Length = fd.Size;
			file.LastModifiedDate = fd.LastModifiedDate;
			return file;
		}

		public IEnumerable<IStorageFile> GetAllMetadata(IStorageFolder folder)
		{
			return from o in _dc.IDbFileData
				   where o.Owner == folder.ID
				   select new StorageFile(folder) { Extension = o.Extension, Name = o.Title, Length = o.Size, ID = o.FileGUID, LastModifiedDate = o.LastModifiedDate };
		}
	}

	public interface IDC_FileDataStorage : IDataContext
	{
		IDbFileData NewIDbFileData(Guid ID);
		ITable<IDbFileData> IDbFileData { get; }
	}

	public interface IDbFileData : IEntity
	{
		byte[] Data { get; set; }
		int Size { get; set; }
		string Title { get; set; }
		string Extension { get; set; }
		Guid FileGUID { get; set; }
		Guid? Owner { get; set; }
		DateTime LastModifiedDate { get; set; }
	}
}
