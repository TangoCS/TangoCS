using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Nephrite.Web.FileStorage
{
	public class FolderPacker
	{
		public void PackFolder(Guid folderID)
		{
			var folder = FileStorageManager.GetFolder(folderID);

			MemoryStream ms = new MemoryStream();
			PackFolder(folder, ms);
			var r = HttpContext.Current.Response;
			r.AppendHeader("Content-Type", "application/x-zip-compressed");
			r.AppendHeader("Content-disposition", "attachment; filename=" + folder.Title + ".zip");
			ms.WriteTo(r.OutputStream);
			r.End();
		}

		void PackFolder(IDbFolder f, Stream s)
		{
			ZipConstants.DefaultCodePage = 866;
			using (ZipFile zf = ZipFile.Create(s))
			{
				AddFiles(zf, f, "");
				zf.Close();
			}
		}

		void AddFiles(ZipFile zf, IDbFolder f, string parent)
		{
			foreach (var file in FileStorageManager.DbFiles.Where(o => o.ParentFolderID == f.ID).ToList())
			{
				using (var s = new MemoryStream(file.GetBytes()))
				{
					zf.BeginUpdate();
					zf.Add(new MemoryDataStream(s), parent + f.Title + "\\" + file.Title);
					zf.CommitUpdate();
				}
			}
			foreach (var folder in FileStorageManager.DbFolders.Where(o => o.ParentFolderID == f.ID).ToList())
			{
				AddFiles(zf, folder, parent + f.Title + "\\");
			}
		}

		class MemoryDataStream : IStaticDataSource
		{
			private MemoryStream _ms;

			#region IStaticDataSource Member

			public Stream GetSource()
			{
				return _ms;
			}

			#endregion

			public MemoryDataStream(MemoryStream ms)
			{
				this._ms = ms;
				this._ms.Position = 0;
			}
		}
	}
}