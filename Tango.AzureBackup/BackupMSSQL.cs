using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;

namespace Tango.AzureBackup
{
	public static class BackupMSSQL
	{
		public static void Run(string connectionString, string localPath, string sharePath, string connectionStringAzure, string domain, string username, string password)
		{
			string fullpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BackupMSSQL");
			Directory.CreateDirectory(fullpath);

			SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(connectionString);
			string filename = b.InitialCatalog + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".bak";
			string pathbackup = Path.Combine(localPath, filename);

			using (var con = new SqlConnection(b.ConnectionString))
			{
				con.Open();

				using (var cmd = new SqlCommand())
				{
					cmd.Connection = con;
					cmd.CommandTimeout = 600;
					cmd.CommandText = string.Format("EXEC BackupDatabase '{0}'", pathbackup);

					cmd.ExecuteNonQuery();
				}
				con.Close();
			}

			string sharefile = Path.Combine(sharePath, filename);

			fullpath = Path.Combine(fullpath, filename);

			File.Delete(fullpath);

			if (!File.Exists(sharefile)) return;

			if (string.IsNullOrEmpty(username))
			{
				File.Move(sharefile, fullpath);
			}
			else
			{
				using (new Impersonation(domain, username, password))
				{
					// Переместить бэкап из сетевой папки
					File.Move(sharefile, fullpath);
				}
			}

			string zipname = fullpath.Replace(".bak", ".zip");
			using (ZipFile zf = ZipFile.Create(zipname))
			{
				zf.BeginUpdate();
				zf.Add(fullpath, filename);
				zf.CommitUpdate();
				zf.Close();
			}

			try
			{
				var azurebackup = new AzureBackup(connectionStringAzure);
				azurebackup.Save(zipname, Path.GetFileName(zipname), "backup-mssql-" + b.InitialCatalog.ToLower());
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
