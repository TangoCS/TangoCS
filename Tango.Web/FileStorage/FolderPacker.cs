using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Nephrite.FileStorage
{
	public class FolderPacker
	{
		public void PackFolder(IStorage storage, IStorageFolder f)
		{
			var r = HttpContext.Current.Response;
			r.AppendHeader("Content-Type", "application/x-zip-compressed");
			r.AppendHeader("Content-disposition", "attachment; filename=" + f.Name + ".zip");

			using (MemoryStream ms = new MemoryStream())
			{
				ZipConstants.DefaultCodePage = 866;
				using (ZipFile zf = ZipFile.Create(ms))
				{
					AddFiles(zf, storage, f, "");
					zf.Close();
				}				
				ms.WriteTo(r.OutputStream);
			}
			r.End();
		}

		void AddFiles(ZipFile zf, IStorage storage, IStorageFolder f, string parent)
		{
			foreach (var file in f.GetAllFiles())
			{
				using (var s = new MemoryStream(file.ReadAllBytes()))
				{
					zf.BeginUpdate();
					zf.Add(new MemoryDataStream(s), parent + f.Name + "\\" + file.Name);
					zf.CommitUpdate();
				}
			}

			foreach (var folder in storage.GetFolders(f))
			{
				AddFiles(zf, storage, folder, parent + f.Name + "\\");
			}
		}

		public class MemoryDataStream : IStaticDataSource
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