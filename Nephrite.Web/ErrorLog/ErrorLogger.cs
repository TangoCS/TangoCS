using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using System.Collections.Specialized;
using System.IO;
//using System.Data.Linq;
using System.Text.RegularExpressions;
using Nephrite.SettingsManager;


namespace Nephrite.Web.ErrorLog
{
    public static class ErrorLogger
    {
		public static int Log(Exception exception)
        {
			string errorInfo = DateTime.Now.ToString()+Environment.NewLine;
            try
            {
				if (exception is HttpException)
				{
					string errorCode = ((HttpException)exception).ErrorCode.ToString("X");
					if (errorCode == "0x800703E3" || errorCode == "0x800704CD" || errorCode == "0x80070040")
						return 0;
				}

				//if (exception.Message == "This is an invalid webresource request." || exception.Message == "This is an invalid script resource request.")
				//	return 0;
				//if (HttpContext.Current != null && HttpContext.Current.Request.Url.ToString().ToLower().Contains("rss.aspx"))
				//	return 0;

				HttpRequest r = HttpContext.Current.Request;
				// Проверить, не входит ли URL в список запрещенных к логированию
				string[] blockedUrlPatterns = AppSettings.Get("ErrorLoggingBlockedUrlPatterns").Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (var pattern in blockedUrlPatterns)
					if (Regex.IsMatch(r.Url.AbsoluteUri, pattern))
						return 0;

                string errortext = "";
                Exception e = exception;
                while (e != null)
                {
                    errortext += e.Message + Environment.NewLine + e.StackTrace ?? String.Empty;
                    e = e.InnerException;
                    if (e != null)
                        errortext += Environment.NewLine + Environment.NewLine;
                }
				errorInfo += errortext;
				using (var dc = (IDC_ErrorLog)A.Model.NewDataContext())
                {
					//dc.CommandTimeout = 300;
                    IErrorLog l = dc.NewIErrorLog();
                    l.ErrorDate = DateTime.Now;
                    l.ErrorText = errortext;
                    l.Headers = "";
                    for (int i = 0; i < r.Headers.Count; i++)
                        l.Headers += r.Headers.GetKey(i) + ": " + r.Headers[i] + Environment.NewLine;
					errorInfo += "[Headers]" + Environment.NewLine + l.Headers ?? "";
                    l.RequestType = r.RequestType;
					errorInfo += "[RequestType]" + Environment.NewLine + l.RequestType + Environment.NewLine;
                    l.Url = r.Url.AbsoluteUri;
					errorInfo += "[Url]" + Environment.NewLine + l.Url + Environment.NewLine;
					l.UrlReferrer = r.UrlReferrer == null ? "" : r.UrlReferrer.AbsoluteUri;
					errorInfo += "[UrlReferrer]" + Environment.NewLine + l.UrlReferrer + Environment.NewLine;

					l.UserName = HttpContext.Current.User.Identity.Name;
					errorInfo += "[UserName]" + Environment.NewLine + l.UserName + Environment.NewLine;

                    l.UserAgent = r.UserAgent;
					errorInfo += "[UserAgent]" + Environment.NewLine + l.UserAgent + Environment.NewLine;
                    l.UserHostAddress = r.UserHostAddress;
					errorInfo += "[UserHostAddress]" + Environment.NewLine + l.UserHostAddress + Environment.NewLine;
					l.UserHostName = r.UserHostName;

					l.SqlLog = "";//dataContext.Log.ToString();
					List<IDataContext> loggedDC = new List<IDataContext>();
                    foreach (var item in HttpContext.Current.Items.Values.OfType<IDataContext>())
                    {
						if (loggedDC.Contains(item))
							continue;
						loggedDC.Add(item);
                        if (item.Log != null)
                            l.SqlLog += "\r\n\r\n>>>>> " + item.GetType().FullName + " <<<<<\r\n" + item.Log.ToString();
                    }
					//foreach (var item in HttpContext.Current.Items.Values.OfType<HCoreDataContext>())
					//{
					//	if (item.Log != null)
					//		l.SqlLog += "\r\n\r\n>>>>> " + item.GetType().FullName + " <<<<<\r\n" + item.Log.ToString();
					//}
					errorInfo += "[SqlLog]" + Environment.NewLine + l.SqlLog;
					dc.IErrorLog.InsertOnSubmit(l);
                    dc.SubmitChanges();
					return l.ErrorLogID;
                }
            }
            catch(Exception errorException)
            {
				try
				{
					string path = HttpContext.Current.Server.MapPath("~/App_Data/Errors");
					Directory.CreateDirectory(path);
					errorInfo += "[ErrorLog Error]" + Environment.NewLine + errorException.Message + Environment.NewLine + errorException.StackTrace;
					File.WriteAllText(Path.Combine(path, "error_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt"), errorInfo);
				}
				catch { }
            }

			return 0;
        }
    }

	public interface IDC_ErrorLog : IDataContext
	{
		ITable<IErrorLog> IErrorLog { get; }
		IErrorLog NewIErrorLog();
	}

	public interface IErrorLog : IEntity
	{
		int ErrorLogID { get; set; }
		System.DateTime ErrorDate { get; set; }
		string ErrorText { get; set; }
		string Url { get; set; }
		string UrlReferrer { get; set; }
		string UserHostName { get; set; }
		string UserHostAddress { get; set; }
		string UserAgent { get; set; }
		string RequestType { get; set; }
		string Headers { get; set; }
		string SqlLog { get; set; }
		string UserName { get; set; }
		byte[] Hash { get; set; }
		System.Nullable<int> SimilarErrorID { get; set; }
	}
}