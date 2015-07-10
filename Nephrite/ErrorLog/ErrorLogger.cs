using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Nephrite.Http;
using Nephrite.Data;

namespace Nephrite.ErrorLog
{
	public interface IErrorLogger
	{
		int Log(Exception exception);
    }

	public class ErrorLogger : IErrorLogger
    {
		IHttpContext _httpContext;
		IDC_ErrorLog _dataContext;
		Func<bool> _exceptionFilter;

		public ErrorLogger(
			IHttpContext httpContext,
			IDC_ErrorLog dataContext,
			Func<bool> exceptionFilter = null
			)
		{
			_httpContext = httpContext;
			_dataContext = dataContext;
			_exceptionFilter = exceptionFilter;
		}

		public int Log(Exception exception)
        {
			StringBuilder errorInfo = new StringBuilder();
			errorInfo.AppendLine(DateTime.Now.ToString());
            try
            {
				if (_exceptionFilter != null)
					if (!_exceptionFilter()) return 0;

				IHttpRequest r = null;
				if (_httpContext != null) r = _httpContext.Request;

				//if (exception is HttpException)
				//{
				//	string errorCode = ((HttpException)exception).ErrorCode.ToString("X");
				//	if (errorCode == "0x800703E3" || errorCode == "0x800704CD" || errorCode == "0x80070040")
				//		return 0;
				//}

				//if (exception.Message == "This is an invalid webresource request." || exception.Message == "This is an invalid script resource request.")
				//	return 0;
				//if (HttpContext.Current != null && HttpContext.Current.Request.Url.ToString().ToLower().Contains("rss.aspx"))
				//	return 0;

				// Проверить, не входит ли URL в список запрещенных к логированию
				//string[] blockedUrlPatterns = AppSettings.Get("ErrorLoggingBlockedUrlPatterns").Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
				//foreach (var pattern in blockedUrlPatterns)
				//	if (Regex.IsMatch(r.Url.AbsoluteUri, pattern))
				//		return 0;

                StringBuilder errortext = new StringBuilder();
                Exception e = exception;				
                while (e != null)
                {
                    errortext.AppendLine(e.Message).AppendLine(e.StackTrace ?? String.Empty);
                    e = e.InnerException;
                }

				errorInfo.Append(errortext.ToString());

				using (var dc = _dataContext.NewDataContext() as IDC_ErrorLog)
                {
                    IErrorLog l = dc.NewIErrorLog();
                    l.ErrorDate = DateTime.Now;
                    l.ErrorText = errortext.ToString();
                    l.Headers = "";

					StringBuilder headers = new StringBuilder();
					if (r != null)
						for (int i = 0; i < r.Headers.Count; i++)
							headers.AppendLine(r.Headers.GetKey(i) + ": " + r.Headers[i]);
					l.Headers = headers.ToString();
					//l.RequestType = r.RequestType;
					if (r != null) l.Url = r.Url.AbsoluteUri;
					//l.UrlReferrer = r.UrlReferrer == null ? "" : r.UrlReferrer.AbsoluteUri;
					if (_httpContext != null) l.UserName = _httpContext.User.Identity.Name;
					//l.UserAgent = r.UserAgent;
					//l.UserHostAddress = r.UserHostAddress;
					//l.UserHostName = r.UserHostName;
					l.SqlLog = "";

					if (_httpContext != null)
					{
						List<IDataContext> loggedDC = new List<IDataContext>();
						foreach (var item in _httpContext.Items.Values.OfType<IDataContext>())
						{
							if (loggedDC.Contains(item)) continue;
							loggedDC.Add(item);
							if (item.Log != null)
								l.SqlLog += "\r\n\r\n>>>>> " + item.GetType().FullName + " <<<<<\r\n" + item.Log.ToString();
						}
					}


					errorInfo.AppendLine("[Headers]").AppendLine(l.Headers ?? "");
                    errorInfo.AppendLine("[RequestType]").AppendLine(l.RequestType);
                    errorInfo.AppendLine("[Url]").AppendLine(l.Url);
					errorInfo.AppendLine("[UrlReferrer]").AppendLine(l.UrlReferrer);
					errorInfo.AppendLine("[UserName]").AppendLine(l.UserName);
                    errorInfo.AppendLine("[UserAgent]").AppendLine(l.UserAgent);
                    errorInfo.AppendLine("[UserHostAddress]").AppendLine(l.UserHostAddress);
					errorInfo.AppendLine("[SqlLog]").AppendLine(l.SqlLog);

					dc.IErrorLog.InsertOnSubmit(l);
                    dc.SubmitChanges();
					return l.ErrorLogID;
                }
            }
            catch(Exception errorException)
            {
				WriteFile(errorInfo, errorException);
            }

			return 0;
        }

		static void WriteFile(StringBuilder errorInfo, Exception errorException)
		{
			try
			{
				string path = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "App_Data" + Path.DirectorySeparatorChar + "Errors";
				Directory.CreateDirectory(path);
				if (errorException != null)
					errorInfo.AppendLine("[ErrorLog Error]").AppendLine(errorException.Message).AppendLine(errorException.StackTrace);
				File.WriteAllText(Path.Combine(path, "error_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt"), errorInfo.ToString());
			}
			catch { }
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