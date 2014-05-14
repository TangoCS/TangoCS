using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using System.Collections.Specialized;
using System.IO;
using System.Data.Linq;
using System.Text.RegularExpressions;
//using Nephrite.Web.ErrorLogging;
using Nephrite.Web.Model;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using NHibernate.Cfg.Loquacious;
using NHibernate.Linq;

namespace Nephrite.Web
{
    public static class ErrorLogger
    {
		[Obsolete]
		public static int Log(Exception exception, DataContext dataContext)
		{
			return Log(exception);
		}
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

				HttpRequest r = null;
				if (HttpContext.Current != null)
				{
					r = HttpContext.Current.Request;
					// Проверить, не входит ли URL в список запрещенных к логированию
					string[] blockedUrlPatterns = App.AppSettings.Get("ErrorLoggingBlockedUrlPatterns").Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (var pattern in blockedUrlPatterns)
						if (Regex.IsMatch(r.Url.AbsoluteUri, pattern))
							return 0;
				}
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
				using (HErrorLogDataContext dc = new HErrorLogDataContext(AppWeb.DBConfig))
                {
					//dc.CommandTimeout = 300;
                    ErrorLog l = new ErrorLog();
                    l.ErrorDate = DateTime.Now;
                    l.ErrorText = errortext;
                    l.Headers = "";
					if (r != null)
					{
						for (int i = 0; i < r.Headers.Count; i++)
							l.Headers += r.Headers.GetKey(i) + ": " + r.Headers[i] + Environment.NewLine;
						errorInfo += "[Headers]" + Environment.NewLine + l.Headers ?? "";
						l.RequestType = r.RequestType;
						errorInfo += "[RequestType]" + Environment.NewLine + l.RequestType + Environment.NewLine;
						l.Url = r.Url.AbsoluteUri;
						errorInfo += "[Url]" + Environment.NewLine + l.Url + Environment.NewLine;
						l.UrlReferrer = r.UrlReferrer == null ? "" : r.UrlReferrer.AbsoluteUri;
						errorInfo += "[UrlReferrer]" + Environment.NewLine + l.UrlReferrer + Environment.NewLine;

						l.UserName = HttpContext.Current == null ? "System" : HttpContext.Current.User.Identity.Name;
						errorInfo += "[UserName]" + Environment.NewLine + l.UserName + Environment.NewLine;

						l.UserAgent = r.UserAgent;
						errorInfo += "[UserAgent]" + Environment.NewLine + l.UserAgent + Environment.NewLine;
						l.UserHostAddress = r.UserHostAddress;
						errorInfo += "[UserHostAddress]" + Environment.NewLine + l.UserHostAddress + Environment.NewLine;
						l.UserHostName = r.UserHostName;
					}
					l.SqlLog = "";//dataContext.Log.ToString();
					if (HttpContext.Current != null)
					{
						List<DataContext> loggedDC = new List<DataContext>();
						foreach (var item in HttpContext.Current.Items.Values.OfType<DataContext>())
						{
							if (loggedDC.Contains(item))
								continue;
							loggedDC.Add(item);
							if (item.Log != null)
								l.SqlLog += "\r\n\r\n>>>>> " + item.GetType().FullName + " <<<<<\r\n" + item.Log.ToString();
						}
						foreach (var item in HttpContext.Current.Items.Values.OfType<HCoreDataContext>())
						{
							if (item.Log != null)
								l.SqlLog += "\r\n\r\n>>>>> " + item.GetType().FullName + " <<<<<\r\n" + item.Log.ToString();
						}
					}
					errorInfo += "[SqlLog]" + Environment.NewLine + l.SqlLog;
					dc.ErrorLogs.InsertOnSubmit(l);
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
}

namespace Nephrite.Web.Model
{
	public partial class ErrorLog
	{
		public virtual int ErrorLogID { get; set; }
		public virtual System.DateTime ErrorDate { get; set; }
		public virtual string ErrorText { get; set; }
		public virtual string Url { get; set; }
		public virtual string UrlReferrer { get; set; }
		public virtual string UserHostName { get; set; }
		public virtual string UserHostAddress { get; set; }
		public virtual string UserAgent { get; set; }
		public virtual string RequestType { get; set; }
		public virtual string Headers { get; set; }
		public virtual string SqlLog { get; set; }
		public virtual string UserName { get; set; }
		public virtual byte[] Hash { get; set; }
		public virtual System.Nullable<int> SimilarErrorID { get; set; }
	}

	public class ErrorLogMap : ClassMapping<ErrorLog>
	{
		public ErrorLogMap()
		{
			Table("ErrorLog");
			Lazy(true);
			Id(x => x.ErrorLogID, map => map.Generator(Generators.Identity));
			Property(x => x.ErrorDate, map => map.NotNullable(true));
			Property(x => x.ErrorText, map => map.NotNullable(true));
			Property(x => x.Url);
			Property(x => x.UrlReferrer);
			Property(x => x.UserHostName);
			Property(x => x.UserHostAddress);
			Property(x => x.UserAgent);
			Property(x => x.RequestType);
			Property(x => x.Headers);
			Property(x => x.SqlLog);
			Property(x => x.UserName);
			Property(x => x.Hash);
			Property(x => x.SimilarErrorID);
		}
	}

	public class HErrorLogDataContext : HDataContext
	{
		public HErrorLogDataContext(Action<IDbIntegrationConfigurationProperties> dbConfig)
			: base(dbConfig)
		{
		}

		public override IEnumerable<Type> GetEntitiesTypes()
		{
			List<Type> l = new List<Type>();
			l.Add(typeof(ErrorLogMap));
			return l;
		}

		public HTable<ErrorLog> ErrorLogs
		{
			get
			{
				return new HTable<ErrorLog>(this, Session.Query<ErrorLog>());
			}
		}
	}
}