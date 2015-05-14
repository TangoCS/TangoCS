using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using Npgsql;

namespace Nephrite.AzureBackup
{
	public static class BackupPostgreSQL
	{
		public static void Run(string connectionString, string localPath, string sharePath, string connectionStringAzure, string domain, string username, string password)
		{
			string fullpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BackupPGSQL");
			Directory.CreateDirectory(fullpath);

			var b = new NpgsqlConnectionStringBuilder(connectionString);
			string filename = b.Database + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".dump";
			string pathbackup = Path.Combine(localPath, filename);

			var p = Process.Start(
				new ProcessStartInfo
				{
					FileName = "backup_pgsql.bat",
					Arguments = string.Format("{0} {1} {2} {3} {4} {5}", b.Host, b.Port, b.UserName, Encoding.UTF8.GetString(b.PasswordAsByteArray), pathbackup, b.Database),
					WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
					WindowStyle = ProcessWindowStyle.Hidden,
					CreateNoWindow = true,
					UseShellExecute = false
				});

			p.WaitForExit(900000);
			p.Close();

			string sharefile = Path.Combine(sharePath, filename);

			fullpath = Path.Combine(fullpath, filename);

			//File.Delete(fullpath);

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

			string zipname = fullpath.Replace(".dump", ".zip");
			using (ZipFile zf = ZipFile.Create(zipname))
			{
				zf.BeginUpdate();
				zf.Add(fullpath, filename);
				zf.CommitUpdate();
				zf.Close();
			}

			var azurebackup = new AzureBackup(connectionStringAzure);
			azurebackup.Save(zipname, Path.GetFileName(zipname), "backup-pgsql-" + b.Database.ToLower());

			File.Delete(fullpath);
			File.Delete(zipname);
		}
	}
}
