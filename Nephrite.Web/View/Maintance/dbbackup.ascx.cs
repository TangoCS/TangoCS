using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip;
using Nephrite.Core;

namespace Nephrite.Web.View.Maintance
{
    public partial class dbbackup : ViewControl
    {
        protected string BackupDir;

        protected void Page_Load(object sender, EventArgs e)
        {
            SetTitle("Резервное копирование");

            BackupDir = AppDomain.CurrentDomain.BaseDirectory + "DbBackup";
            Directory.CreateDirectory(BackupDir);
        }

        protected void bCreateBackup_Click(object sender, EventArgs e)
        {
            SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

            ServerConnection sc = b.IntegratedSecurity ? new ServerConnection(b.DataSource) : new ServerConnection(b.DataSource, b.UserID, b.Password);
            sc.Connect();
            Server server = new Server(sc);

            Backup sbackup = new Backup();

            sbackup.Action = BackupActionType.Database;
            sbackup.BackupSetDescription = "Full Backup";

            sbackup.BackupSetName = "Database Backup";
            sbackup.Database = b.InitialCatalog;

            BackupDeviceItem dbi;
            string filename = b.InitialCatalog + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".bak";

            string fullpath = BackupDir + Path.DirectorySeparatorChar + filename;

            if (!b.DataSource.ToUpper().StartsWith("(LOCAL)"))
                dbi = new BackupDeviceItem(Path.Combine(ConfigurationManager.AppSettings["MssqlBackupPath"], filename), DeviceType.File);
            else
                dbi = new BackupDeviceItem(fullpath, DeviceType.File);
            sbackup.Devices.Add(dbi);

            sbackup.Incremental = false;
            sbackup.LogTruncation = BackupTruncateLogType.Truncate;
            sbackup.SqlBackup(server);
            sbackup.Devices.Remove(dbi);

            if (!b.DataSource.ToUpper().StartsWith("(LOCAL)"))
            {
				if (ConfigurationManager.AppSettings["BackupAccountUsername"].IsEmpty())
				{
					File.Copy(Path.Combine(ConfigurationManager.AppSettings["MssqlBackupFolder"], filename),
							fullpath, true);
				}
				else
				{
					using (new Impersonation(ConfigurationManager.AppSettings["BackupAccountDomain"],
						ConfigurationManager.AppSettings["BackupAccountUsername"],
						ConfigurationManager.AppSettings["BackupAccountPassword"]))
					{
						// Скопировать бэкап из сетевой папки
						File.Copy(Path.Combine(ConfigurationManager.AppSettings["MssqlBackupFolder"], filename),
							fullpath, true);
					}
				}
            }
            
            using (ZipFile zf = ZipFile.Create(fullpath.Replace(".bak", ".zip")))
            {
                zf.BeginUpdate();
                zf.Add(fullpath, Path.GetFileName(fullpath));
                if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\images"))
                    PackDirectory(zf, AppDomain.CurrentDomain.BaseDirectory + "\\images", "images");
                zf.CommitUpdate();
                zf.Close();
            }

			if (ConfigurationManager.AppSettings["BackupAccountUsername"].IsEmpty())
			{
				File.Delete(fullpath);
			}
			else
			{
				using (new Impersonation(ConfigurationManager.AppSettings["BackupAccountDomain"],
						ConfigurationManager.AppSettings["BackupAccountUsername"],
						ConfigurationManager.AppSettings["BackupAccountPassword"]))
				{
					File.Delete(fullpath);
				}
			}

            Response.Redirect(Request.Url.ToString());
        }

        void PackDirectory(ZipFile zf, string srcdir, string target)
        {
            zf.AddDirectory(target);
            foreach (var f in Directory.GetFiles(srcdir, "*.*", SearchOption.TopDirectoryOnly))
                zf.Add(f, target + "\\" + Path.GetFileName(f));
            foreach (var d in Directory.GetDirectories(srcdir, "*.*", SearchOption.TopDirectoryOnly))
                PackDirectory(zf, srcdir + "\\" + Path.GetFileName(d), target + "\\" + Path.GetFileName(d));
        }

        protected void fileDelete_Click(object sender, EventArgs e)
        {
            string name = BackupDir + Path.DirectorySeparatorChar + fileDelete.Value;
            if (File.Exists(name))
                File.Delete(name);

            Response.Redirect(Request.Url.ToString());
        }
    }
}
