using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Nephrite.UI
{
	public abstract class ActionContext
	{
		public ActionContext()
		{
			AllArgs = new DynamicDictionary(StringComparer.OrdinalIgnoreCase);
			ActionArgs = new DynamicDictionary(StringComparer.OrdinalIgnoreCase);
			EventArgs = new DynamicDictionary(StringComparer.OrdinalIgnoreCase);
			FormData = new DynamicDictionary(StringComparer.OrdinalIgnoreCase);
			EventReceivers = new Dictionary<string, InteractionFlowElement>(StringComparer.OrdinalIgnoreCase);
			//Files = new List<FileInfo>();
		}

		public IServiceProvider RequestServices { get; protected set; }

		public class ResponseInfo
		{
			StringWriter w = new StringWriter();

			public string CsrfToken { get; set; }
			public string ContentType { get; set; }
			//public int StatusCode { get; set; } = 200;
			//public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();

			//public void Redirect(string location, bool permanent = false)
			//{
			//	if (permanent)
			//		StatusCode = 301;
			//	else
			//		StatusCode = 302;
			//	Headers["Location"] = location;
			//}

			public void Write(string text)
			{
				w.Write(text);
			}

			public string GetContent()
			{
				return w.ToString();
			}
		}

		public class FileInfo
		{
			public string FileName { get; set; }
			public byte[] FileBytes { get; set; }
		}

		public string Service { get; set; }
		public string Action { get; set; }
		public string RequestMethod { get; set; }
		public string Event { get; set; }
		public string EventReceiver { get; set; }
		//public List<FileInfo> Files { get; set; }

		public DynamicDictionary AllArgs { get; set; }
		public DynamicDictionary ActionArgs { get; set; }
		public DynamicDictionary EventArgs { get; set; }

		public dynamic FormBag { get { return FormData; } }
		public DynamicDictionary FormData { get; set; }

		public IDictionary<string, InteractionFlowElement> EventReceivers { get; private set; }

		public ResponseInfo Response { get; set; } = new ResponseInfo();
	}


	public static class ActionContextExtensions
	{
		public static string GetArg(this ActionContext ctx, string name)
		{
			object s = null;
			bool b = ctx.AllArgs.TryGetValue(name, out s);
			if (b) return WebUtility.UrlDecode(s.ToString());
			return null;
		}
		public static int GetIntArg(this ActionContext ctx, string name, int defaultValue)
		{
			object s = null;
			bool b = ctx.AllArgs.TryGetValue(name, out s);
			if (b) return Convert.ToInt32(s);
			return defaultValue;
		}
		public static int? GetIntArg(this ActionContext ctx, string name)
		{
			object s = null;
			bool b = ctx.AllArgs.TryGetValue(name, out s);
			if (b) return Convert.ToInt32(s);
			return null;
		}
		public static Guid GetGuidArg(this ActionContext ctx, string name, Guid defaultValue)
		{
			object s = null;
			bool b = ctx.AllArgs.TryGetValue(name, out s);
			if (b) return Guid.Parse(s.ToString());
			return defaultValue;
		}
		public static Guid? GetGuidArg(this ActionContext ctx, string name)
		{
			object s = null;
			bool b = ctx.AllArgs.TryGetValue(name, out s);
			if (b) return Guid.Parse(s.ToString());
			return null;
		}
		public static T GetArg<T>(this ActionContext ctx, string name)
		{
			return ctx.AllArgs.Parse<T>(name);
		}
	}
}
