using Dapper;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.IO;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using Tango.Data;

namespace Tango.Logger
{
	public class ErrorLogger : IErrorLogger
    {
		IDbConnection _conn;
		IExceptionFilter _exceptionFilter;
		IHostingEnvironment _env;
		IRequestEnvironment _req;
		IRequestLoggerProvider _reqLoggerProvider;
		string _userName;

		public ErrorLogger(
			IDbConnection conn, 
			IIdentity identity, 
			IHostingEnvironment env, 
			IRequestEnvironment req,
			IRequestLoggerProvider reqLoggerProvider,
			IExceptionFilter exceptionFilter)
		{
			_conn = conn;
			_exceptionFilter = exceptionFilter;
			_env = env;
			_req = req;
			_reqLoggerProvider = reqLoggerProvider;
			_userName = identity?.Name;
		}

		public int Log(Exception exception)
        {
			if (_exceptionFilter != null && !_exceptionFilter.Filter(exception)) return 0;

			var errortext = new StringBuilder();
			var e = exception;
			while (e != null)
			{
				errortext.AppendLine(e.Message).AppendLine(e.StackTrace ?? String.Empty);
				e = e.InnerException;
			}

			var headers = new StringBuilder();
			if (_req.Headers != null)
				foreach (var h in _req.Headers.Keys)
					headers.AppendLine(h + ": " + _req.Headers[h].Join("; "));

			var logs = new StringBuilder();
			if (_reqLoggerProvider != null)
			{				
				foreach (var log in _reqLoggerProvider.GetLoggers())
				{
					logs.AppendLine(log.Key);
					logs.AppendLine(log.Value?.ToString());
				}
			}

			var l = new ErrorLog {
				ErrorDate = DateTime.Now,
				ErrorText = errortext.ToString(),
				RequestType = _req.Method,
				Url = $"{_req.Scheme}://{_req.Host}{_req.Path}{_req.QueryString}",
				UrlReferrer = _req.Referrer,
				UserName = _userName,
				UserAgent = _req.UserAgent,
				UserHostAddress = _req.IP?.ToString(),
				UserHostName = _req.Host,
				Headers = headers.ToString(),
				SqlLog = logs.ToString()
			};

			try
            {
				return _conn.QuerySingle<int>(@"
insert into errorlog(errordate, errortext, url, urlreferrer, userhostname, userhostaddress, useragent, requesttype, headers, sqllog, username) 
values (@ErrorDate, @ErrorText, @Url, @UrlReferrer, @UserHostName, @UserHostAddress, @UserAgent, @RequestType, @Headers, @SqlLog, @UserName)
returning errorlogid", l);

			}
            catch(Exception errorException)
            {
				var errorInfo = new StringBuilder();
				errorInfo.AppendLine(DateTime.Now.ToString());
				errorInfo.AppendLine(l.ErrorText);
				errorInfo.AppendLine("[Headers]").AppendLine(l.Headers ?? "");
				errorInfo.AppendLine("[RequestType]").AppendLine(l.RequestType);
				errorInfo.AppendLine("[Url]").AppendLine(l.Url);
				errorInfo.AppendLine("[UrlReferrer]").AppendLine(l.UrlReferrer);
				errorInfo.AppendLine("[UserName]").AppendLine(l.UserName);
				errorInfo.AppendLine("[UserAgent]").AppendLine(l.UserAgent);
				errorInfo.AppendLine("[UserHostAddress]").AppendLine(l.UserHostAddress);
				errorInfo.AppendLine("[SqlLog]").AppendLine(l.SqlLog);

				try
				{
					string path = _env.WebRootPath + Path.DirectorySeparatorChar + "errors";
					Directory.CreateDirectory(path);
					if (errorException != null)
						errorInfo.AppendLine("[ErrorLog Error]").AppendLine(errorException.Message).AppendLine(errorException.StackTrace);
					File.WriteAllText(Path.Combine(path, "error_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt"), errorInfo.ToString());
				}
				catch
				{
					Console.Write(errorInfo.ToString());
				}
				return 0;
			}			
        }
    }

	[Table("ErrorLog")]
	public partial class ErrorLog : IEntity, IWithKey<ErrorLog, int>
	{
		public ErrorLog()
		{

		}


		public virtual Expression<Func<ErrorLog, bool>> KeySelector(int id)
		{
			return o => o.ErrorLogID == id;
		}
		public virtual int ID { get { return ErrorLogID; } }
		[Key]
		[Column]
		public virtual int ErrorLogID { get; set; }
		[Column]
		public virtual DateTime ErrorDate { get; set; }
		[Column]
		public virtual string ErrorText { get; set; }
		[Column]
		public virtual string Url { get; set; }
		[Column]
		public virtual string UrlReferrer { get; set; }
		[Column]
		public virtual string UserHostName { get; set; }
		[Column]
		public virtual string UserHostAddress { get; set; }
		[Column]
		public virtual string UserAgent { get; set; }
		[Column]
		public virtual string RequestType { get; set; }
		[Column]
		public virtual string Headers { get; set; }
		[Column]
		public virtual string SqlLog { get; set; }
		[Column]
		public virtual string UserName { get; set; }
		[Column]
		public virtual byte[] Hash { get; set; }
		[Column]
		public virtual int? SimilarErrorID { get; set; }
	}
}