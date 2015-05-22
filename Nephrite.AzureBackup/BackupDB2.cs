using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBM.Data.DB2;
using ICSharpCode.SharpZipLib.Zip;

namespace Nephrite.AzureBackup
{
	public static class BackupDB2
	{
		public static void Run(string connectionString, string localPath, string sharePath, string connectionStringAzure, string domain, string username, string password)
		{
			string fullpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BackupDB2");
			Directory.CreateDirectory(fullpath);

			var db = new DB2ConnectionStringBuilder(connectionString);
			string pathbackup = localPath;

			using (var con = new DB2Connection(db.ConnectionString))
			{
				con.Open();

				using (var cmd = new DB2Command())
				{
					cmd.Connection = con;
					cmd.CommandTimeout = 600;
					cmd.CommandText = "CALL SYSPROC.ADMIN_CMD('QUIESCE DB IMMEDIATE FORCE CONNECTIONS')";
					try
					{
						cmd.ExecuteNonQuery();
					}
					catch {}
					cmd.CommandText = string.Format("CALL SYSPROC.ADMIN_CMD('BACKUP DB {0} ONLINE to {1} COMPRESS INCLUDE LOGS WITHOUT PROMPTING')", db.Database, pathbackup);
					cmd.ExecuteNonQuery();
					cmd.CommandText = "CALL SYSPROC.ADMIN_CMD('UNQUIESCE DB')";
					try
					{
						cmd.ExecuteNonQuery();
					}
					catch {}
				}
				con.Close();
			}

			string sharefile = Directory.GetFiles(sharePath).First();

			fullpath = Path.Combine(fullpath, Path.GetFileName(sharefile));

			File.Delete(fullpath);

			if (!File.Exists(sharefile)) return;

			if (string.IsNullOrEmpty(username))
			{
				File.Move(sharefile, fullpath);
			}
			else
			{
				using (new Nephrite.Core.Impersonation(domain, username, password))
				{
					// Переместить бэкап из сетевой папки
					File.Move(sharefile, fullpath);
				}
			}

			string zipname = fullpath.Replace(Path.GetExtension(fullpath), ".zip");
			using (ZipFile zf = ZipFile.Create(zipname))
			{
				zf.BeginUpdate();
				zf.Add(fullpath, Path.GetFileName(fullpath));
				zf.CommitUpdate();
				zf.Close();
			}

			try
			{
				var azurebackup = new AzureBackup(connectionStringAzure);
				azurebackup.Save(zipname, Path.GetFileName(zipname), "backup-db2-" + db.Database.ToLower());
			}
			catch
			{
				File.Delete(fullpath);
				File.Delete(zipname);
				throw;
			}
			File.Delete(fullpath);
			File.Delete(zipname);
		}
	}
}
