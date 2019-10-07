using System.Data;
using Dapper;
using NHibernate.Event;
using Abc.Model;
using Tango.FileStorage.Std.Model;
using System.Threading;
using System.Threading.Tasks;

namespace Abc
{
	public class FileStorageListenerDB2 : IFileStorageListener
	{
		IDbConnection _dc;
		public FileStorageListenerDB2(IDbConnection dc)
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
@"DECLARE GLOBAL TEMPORARY TABLE SESSION.FOLDERS
(
	FolderID INT, Path VARCHAR(2048), GuidPath VARCHAR(2048)
) ON COMMIT PRESERVE ROWS;

insert into SESSION.FOLDERS (FolderID, Path, GuidPath)
with folders (FolderID, Path, GuidPath) as
	(select FolderID, 
        coalesce((select p.Path || '/' 
                  from N_Folder p
                 where p.FolderID = ins.ParentID), '') || Title as FullPath,
        cast(coalesce((select p.GuidPath || '/' 
                  from N_Folder p
                 where p.FolderID = ins.ParentID), '') || cast(Guid as varchar(36)) as varchar(2048)) as GuidPath
	  from N_Folder ins where ins.Guid = ?
	 union all
	select N_Folder.FolderID,
	       folders.Path || '/' || N_Folder.Title,
	       cast(folders.GuidPath || '/' || cast(N_Folder.Guid as varchar(36)) as varchar(2048))
	  from folders, N_Folder
	 where N_Folder.ParentID = folders.FolderID)
	 
select FolderID, Path, GuidPath
from folders;

UPDATE N_Folder
SET (Path, GuidPath) = (select folders.Path, folders.GuidPath
FROM SESSION.FOLDERS folders
WHERE folders.FolderID = N_Folder.FolderID)
WHERE FolderID in (select folders.FolderID FROM SESSION.FOLDERS folders);

DROP TABLE SESSION.FOLDERS;";
	}
}
