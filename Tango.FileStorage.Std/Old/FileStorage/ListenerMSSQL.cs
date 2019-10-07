using System.Data;
using NHibernate.Event;
using Abc.Model;
using Dapper;
using Tango.FileStorage.Std.Model;
using System.Threading;
using System.Threading.Tasks;

namespace Abc
{
	public class FileStorageListenerMSSQL : IFileStorageListener
	{
		IDbConnection _dc;
		public FileStorageListenerMSSQL(IDbConnection dc)
		{
			_dc = dc;
		}

		public void OnPostUpdate(PostUpdateEvent e)
		{
			if (e.Entity is N_Folder)
			{
				var f = e.Entity as N_Folder;
				_dc.Execute(_ui_folder, f.Guid.ToString());
			}
		}

		public void OnPostInsert(PostInsertEvent e)
		{
			if (e.Entity is N_Folder)
			{
				var f = e.Entity as N_Folder;
				_dc.Execute(_ui_folder, f.Guid.ToString());
			}
		}

		public Task OnPostUpdateAsync(PostUpdateEvent @event, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		public Task OnPostInsertAsync(PostInsertEvent @event, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		string _ui_folder =
@"with folders (FolderID, Path, GuidPath) as
	(select FolderID, 
        coalesce((select p.Path + '/' 
                  from N_Folder p
                 where p.FolderID = ins.ParentID), '') + Title as Path,
        cast(isnull((select p.GuidPath + '/' 
                  from N_Folder p
                 where p.FolderID = ins.ParentID), '') + cast(Guid as nvarchar(36)) as nvarchar(2048)) as GuidPath
	  from N_Folder ins where ins.Guid = ?
	 union all
	select N_Folder.FolderID,
	       folders.Path + '/' + N_Folder.Title,
	       cast(folders.GuidPath + '/' + cast(N_Folder.Guid as nvarchar(36)) as nvarchar(2048))
	  from folders, N_Folder
	 where N_Folder.ParentID = folders.FolderID)
UPDATE N_Folder
SET Path = folders.Path, GuidPath = folders.GuidPath
FROM folders
WHERE folders.FolderID = N_Folder.FolderID";


	}
}
