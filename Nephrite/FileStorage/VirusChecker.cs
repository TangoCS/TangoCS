using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Configuration;
using Nephrite.Identity;
using Nephrite.SettingsManager;
using Nephrite.Data;


namespace Nephrite.FileStorage
{
	public interface IVirusChecker
	{
		VirusCheckResult Check(string fileName, byte[] fileBytes);
	}

	public enum VirusCheckResult
	{
		OK, AntiViralFailure, AntiViralSuspicion
	}

	public interface IDC_VirusScan : IDataContext
	{
		IN_VirusScanLog NewIN_VirusScanLog();
		ITable<IN_VirusScanLog> IN_VirusScanLog { get; }
	}

	public interface IN_VirusScanLog : IEntity
	{
		int VirusScanLogID { get; set; }
		int LastModifiedUserID { get; set; }
		bool IsDeleted { get; set; }
		DateTime LastModifiedDate { get; set; }
		string Title { get; set; }
		int ResultCode { get; set; }
	}

    /// <summary>
    /// Проверка на вирусы
    /// </summary>
	public class DefaultVirusChecker : IVirusChecker
    {
		//public static SaveFileResult Check(FileUpload fileUpload)
		//{
		//	return Check(fileUpload.FileName, fileUpload.FileBytes);
		//}
		//public static SaveFileResult Check(HttpPostedFile postedFile)
		//{
		//	return Check(postedFile.FileName, ((MemoryStream)postedFile.InputStream).ToArray());
		//}

		IPersistentSettings _settings;
		IDC_VirusScan _dataContext;
		IIdentityManager<int> _identity;

		public DefaultVirusChecker(IIdentityManager<int> identity,  IPersistentSettings settings, IDC_VirusScan dataContext)
		{
			_settings = settings;
			_dataContext = dataContext;
			_identity = identity;
		}

		public VirusCheckResult Check(string fileName, byte[] fileBytes)
        {
			string enable = _settings.Get("EnableAntiViralCheck").ToLower();
            if (enable != "true" && enable != "1")
				return VirusCheckResult.OK;
			if (ConfigurationManager.AppSettings["SkipVirusCheck"] == "1")
				return VirusCheckResult.OK;

            string path = AppDomain.CurrentDomain.BaseDirectory + "AntiViralCheck\\";
            Directory.CreateDirectory(path);

            path += Guid.NewGuid().ToString();


			File.WriteAllBytes(path, fileBytes);
            //fileUpload.SaveAs(path);
            Thread.Sleep(1000);
			if (!File.Exists(path))
			{
				Log(2000, fileName);
				return VirusCheckResult.AntiViralFailure;
			}

            Process p = new Process();
			p.StartInfo = new ProcessStartInfo(_settings.Get("AntiViralProgramPath"),
				String.Format(_settings.Get("AntiViralProgramArgs"), path));
            p.Start();
			bool exit = p.WaitForExit(1000 * _settings.Get("AntiViralProgramTimeout").ToInt32(10));
            if (File.Exists(path))
            {
                try
                {
                    byte[] data = File.ReadAllBytes(path);
                }
                catch
                {
					Log(3000, fileName);
					return VirusCheckResult.AntiViralFailure;
                }
				File.Delete(path);
            }
            if (!exit)
            {
                p.Kill();
				Log(1000, fileName);
				return VirusCheckResult.AntiViralSuspicion;
            }
            else
            {
				Log(p.ExitCode, fileName);
				if (_settings.Get("AntiViralSuccessCodes").Split(',', ' ', ';').Contains(p.ExitCode.ToString()))
                {
					return VirusCheckResult.OK;
                }
				if (_settings.Get("AntiViralSuspicionCodes").Split(',', ' ', ';').Contains(p.ExitCode.ToString()))
                {
					enable = _settings.Get("AntiViralAllowSuspicionUpload").ToLower();
                    if (enable == "true" || enable == "1")
						return VirusCheckResult.OK;

					return VirusCheckResult.AntiViralSuspicion;
                }
				return VirusCheckResult.AntiViralFailure;
            }
        }

        void Log(int code, string fileName)
        {
			var l = _dataContext.NewIN_VirusScanLog();
			l.LastModifiedDate = DateTime.Now;
            l.LastModifiedUserID = _identity.CurrentSubject.ID;
            l.ResultCode = code;
			l.Title = fileName;
			_dataContext.IN_VirusScanLog.InsertOnSubmit(l);
        }
    }
}
