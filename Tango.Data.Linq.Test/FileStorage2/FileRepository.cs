using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web;
using Nephrite.Web.FileStorage;

namespace Solution.Features.FileStorage
{
	public class FileRepository : IFileRepository
	{
		Dictionary<string, IDbFolder> folderCache
		{
			get
			{
				if (HttpContext.Current.Items["FileRepositoryFolderCache"] == null)
					HttpContext.Current.Items["FileRepositoryFolderCache"] = new Dictionary<string, IDbFolder>();
				return (Dictionary<string, IDbFolder>)HttpContext.Current.Items["FileRepositoryFolderCache"];
			}
		}

		public IQueryable<IDbItem> DbItems
		{
			get
			{
				return App.DataContext.GetTable<Solution.Model.DbItem>().OrderBy(o => o.Title).OrderBy(o => o.Type).Cast<IDbItem>();
			}
		}

		public IQueryable<IDbFolder> DbFolders
		{
			get
			{
				return App.DataContext.GetTable<Solution.Model.DbFolder>().OrderBy(o => o.Title).Cast<IDbFolder>();
			}
		}

		public IQueryable<IDbFile> DbFiles
		{
			get
			{
				return App.DataContext.GetTable<Solution.Model.DbFile>().OrderBy(o => o.Title).Cast<IDbFile>();
			}
		}

		public IDbFolder CreateFolder(string fullpath)
		{
			if (folderCache.ContainsKey(fullpath.ToLower()))
				return folderCache[fullpath.ToLower()];
			var items = App.DataContext.GetTable<Solution.Model.DbFolder>();
			var item = items.FirstOrDefault(o => o.FullPath == fullpath);
			if (item != null)
				return item;

			string title = fullpath.IndexOf('/') >= 0 ? fullpath.Substring(fullpath.LastIndexOf('/') + 1) : fullpath;
			item = new Solution.Model.DbFolder();
			folderCache.Add(fullpath.ToLower(), item);
			item.Title = title.IsEmpty() ? "new folder" : title;
			if (title != fullpath && fullpath.Substring(0, fullpath.LastIndexOf('/')) != "")
			{
				var pfolder = CreateFolder(fullpath.Substring(0, fullpath.LastIndexOf('/')));
				item.SetParentFolder(pfolder);

				var parameter = pfolder.GetStorageParameter();
				var type = pfolder.GetStorageType();
				if (type == FileStorageType.FileSystem)
					parameter = System.IO.Path.Combine(parameter, pfolder.Title);
				item.SetStorageInfo(type, parameter);
			}
			else
			{
				item.SetStorageInfo(FileStorageType.LocalDatabase, "");
			}
			items.InsertOnSubmit(item);
			item.CheckValid();
			return item;
		}

		public IDbFolder CreateFolder(string title, string path)
		{
			return CreateFolder(path + "/" + title);
		}

		public void DeleteFolder(Guid folderID)
		{
			var items = App.DataContext.GetTable<Solution.Model.DbFolder>();
			items.DeleteAllOnSubmit(items.Where(o => o.ID == folderID));
		}


		public IDbFile CreateFile(string title, string path)
		{
			var items = App.DataContext.GetTable<Solution.Model.DbFile>();
			var item = new Solution.Model.DbFile();
			item.Title = title;
			
			if (!path.IsEmpty())
			{
				var pfolder = CreateFolder(path);
				item.SetParentFolder(pfolder);
			}
			
			items.InsertOnSubmit(item);
			return item;
		}

		public void DeleteFile(Guid fileID)
		{
			var items = App.DataContext.GetTable<Solution.Model.DbFile>();
			items.DeleteAllOnSubmit(items.Where(o => o.ID == fileID));
		}
	}
}