using System;
using System.Linq;
using Tango.Data;
using System.Collections.Generic;
using System.IO;
using Tango.FileStorage.Std.Model;

namespace Tango.FileStorage.Std
{
	public class DatabaseStorageManager : IStorageManager<Guid>
	{
		IDataContext _dataContext;

		public DatabaseStorageManager(IDataContext dataContext)
		{
			_dataContext = dataContext;
		}

		public IStorageFolder<Guid> GetFolder(Guid id = default(Guid), string fullPath = null)
		{
			return new DatabaseFolder(_dataContext, id) {
				FullPath = fullPath
			};
		}
	}

	public class DatabaseFolder : IStorageFolder<Guid>
    {
        public Guid ID { get; set; }
		public string FullPath { get; set; }
		public int MaxFileSize { get; set; }

		public IDataContext DataContext { get; private set; }

		public DatabaseFolder(IDataContext dataContext, Guid id)
		{
			DataContext = dataContext;
            ID = id;
		}

		public DatabaseFolder(IDataContext dataContext) : this(dataContext, Guid.Empty)
		{

		}

        public IStorageFile<Guid> CreateFile(Guid id)
        {
            return new DatabaseFile(this, id, false);
        }

        public IEnumerable<IStorageFile<Guid>> GetFiles(int offset = 0, int limit = 50)
        {
            var data = DataContext.N_FileData();
            if (ID != Guid.Empty)
                data = data.Where(o => o.Owner == ID);
			if (offset > 0)
				data = data.Skip(offset);

			return data.Take(limit).Select(o => new DatabaseFile(this, o.FileGUID, true)
                   {
                       Name = o.Title,
                       Extension = o.Extension,
                       Length = o.Size,
                       LastModifiedDate = o.LastModifiedDate
                   });
        }

		public bool HasFiles()
		{
			var data = DataContext.N_FileData();
			if (ID != Guid.Empty)
				data = data.Where(o => o.Owner == ID);
			return data.Any();
		}

		public IStorageFile<Guid> GetFile(Guid id)
        {
            var data = DataContext.N_FileData();
            if (ID != Guid.Empty)
                data = data.Where(o => o.Owner == ID);
            data = data.Where(o => o.FileGUID == id);

            return (from o in data
					select new DatabaseFile(this, o.FileGUID, true) 
					{
                        Name = o.Title,
                        Extension = o.Extension,
                        Length = o.Size,
                        LastModifiedDate = o.LastModifiedDate
                    }).FirstOrDefault();
        }

        public IStorageFile<Guid> GetFileByName(string name)
        {
            var data = DataContext.N_FileData();
            if (ID != Guid.Empty)
            {
                data = data.Where(o => o.Owner == ID);
            }
            data = data.Where(o => o.Title == name);

            return (from o in data
					select new DatabaseFile(this, o.FileGUID, true)
					{
                        Name = o.Title,
                        Extension = o.Extension,
                        Length = o.Size,
                        LastModifiedDate = o.LastModifiedDate
                    }).FirstOrDefault();
        }

        public IEnumerable<IStorageFolder<Guid>> GetFolders(int offset = 0, int limit = 50)
        {
            throw new NotSupportedException();
        }

        public IStorageFile<Guid> GetOrCreateFile(Guid id, string name)
        {
            var file = GetFile(id);
            if (file == null)
            {
                file = CreateFile(id);
                file.Name = name;
                file.Extension = Path.GetExtension(name);
                file.LastModifiedDate = DateTime.Now;
            }
            return file;
        }
        public IStorageFile<Guid> GetOrCreateFile(string name)
        {
            var file = GetFileByName(name);
            if (file == null)
            {
                file = CreateFile(Guid.NewGuid());
                file.Name = name;
                file.Extension = Path.GetExtension(name);
                file.LastModifiedDate = DateTime.Now;
            }
            return file;
        }

		public IStorageFile<Guid> CreateFile(Guid id, string name)
		{
            return new DatabaseFile(this, id, false) { Name = name, Extension = Path.GetExtension(name) };
		}
	}

    public class DatabaseFile : IStorageFile<Guid>
    {
        public Guid ID { get; private set; }
		public string FullPath { get; private set; }

		public string Name { get; set; }
        public string Extension { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public long Length { get; set; }

		public bool Exists { get; private set; }

		IDataContext _dc;
        Guid _owner;

        public DatabaseFile(DatabaseFolder folder, Guid id, bool exists = true)
        {
            _dc = folder.DataContext;
            _owner = folder.ID;
			ID = id == Guid.Empty ? Guid.NewGuid() : id;
			FullPath = folder.FullPath;
			Exists = exists;
		}

        public void Delete()
        {
			if (Exists)
				_dc.DeleteAllOnSubmit(_dc.N_FileData().Where(o => o.FileGUID == ID));
        }

        public byte[] ReadAllBytes()
        {
			if (!Exists) return new byte[0];
			var fd = _dc.N_FileData().FirstOrDefault(o => o.FileGUID == ID);
            if (fd == null || fd.Data == null)
                return new byte[0];
            return fd.Data;
        }

        public void WriteAllBytes(byte[] bytes)
        {
			N_FileData fileData = null;
			if (Exists) fileData = _dc.N_FileData().FirstOrDefault(o => o.FileGUID == ID);
            if (!Exists || fileData == null)
            {
                fileData = new N_FileData();
                _dc.InsertOnSubmit(fileData);
            }

            fileData.FileGUID = ID;
            fileData.Title = Name;
            fileData.Size = bytes.Length;
            fileData.Data = bytes;
            fileData.Extension = Extension;
            if (_owner != Guid.Empty)
                fileData.Owner = _owner;
            else
                fileData.Owner = null;
			//fileData.Path = FullPath;
			fileData.LastModifiedDate = DateTime.Now;
        }
    }
}
