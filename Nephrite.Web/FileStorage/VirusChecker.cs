using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Configuration;

using Nephrite.Identity;
using Nephrite.SettingsManager;


namespace Nephrite.Web.FileStorage
{
    /// <summary>
    /// Проверка на вирусы
    /// </summary>
    public static class VirusChecker
    {
        /// <summary>
        /// Проверить загруженный файл на вирусы
        /// </summary>
        /// <param name="fileUpload">Контрол загрузки файла</param>
        /// <returns>Сообщение</returns>
		public static SaveFileResult Check(FileUpload fileUpload)
		{
			return Check(fileUpload.FileName, fileUpload.FileBytes);
		}
		public static SaveFileResult Check(HttpPostedFile postedFile)
		{
			return Check(postedFile.FileName, ((MemoryStream)postedFile.InputStream).ToArray());
		}

		public static SaveFileResult Check(string fileName, byte[] fileBytes)
        {
            string enable = AppSettings.Get("EnableAntiViralCheck").ToLower();
            if (enable != "true" && enable != "1")
				return SaveFileResult.OK;
			if (ConfigurationManager.AppSettings["SkipVirusCheck"] == "1")
				return SaveFileResult.OK;

            string path = AppDomain.CurrentDomain.BaseDirectory + "AntiViralCheck\\";
            Directory.CreateDirectory(path);

            path += Guid.NewGuid().ToString();


			File.WriteAllBytes(path, fileBytes);
            //fileUpload.SaveAs(path);
            Thread.Sleep(1000);
			if (!File.Exists(path))
			{
				Log(2000, fileName);
				return SaveFileResult.AntiViralFailure;
			}

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(AppSettings.Get("AntiViralProgramPath"),
                String.Format(AppSettings.Get("AntiViralProgramArgs"), path));
            p.Start();
            bool exit = p.WaitForExit(1000 * AppSettings.Get("AntiViralProgramTimeout").ToInt32(10));
            if (File.Exists(path))
            {
                try
                {
                    byte[] data = File.ReadAllBytes(path);
                }
                catch
                {
					Log(3000, fileName);
					return SaveFileResult.AntiViralFailure;
                }
				File.Delete(path);
            }
            if (!exit)
            {
                p.Kill();
				Log(1000, fileName);
				return SaveFileResult.AntiViralSuspicion;
            }
            else
            {
				Log(p.ExitCode, fileName);
                if (AppSettings.Get("AntiViralSuccessCodes").Split(',', ' ', ';').Contains(p.ExitCode.ToString()))
                {
					return SaveFileResult.OK;
                }
                if (AppSettings.Get("AntiViralSuspicionCodes").Split(',', ' ', ';').Contains(p.ExitCode.ToString()))
                {
                    enable = AppSettings.Get("AntiViralAllowSuspicionUpload").ToLower();
                    if (enable == "true" || enable == "1")
						return SaveFileResult.OK;

					return SaveFileResult.AntiViralSuspicion;
                }
				return SaveFileResult.AntiViralFailure;
            }
        }

        static void Log(int code, string fileName)
        {
			var dc = (IDC_FileStorage)A.Model;
			var l = dc.NewIN_VirusScanLog();
			l.LastModifiedDate = DateTime.Now;
            l.LastModifiedUserID = Subject.Current.ID;
            l.ResultCode = code;
			l.Title = fileName;
			dc.IN_VirusScanLog.InsertOnSubmit(l);
        }
    }
}
