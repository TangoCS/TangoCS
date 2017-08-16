using Tango.Localization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Globalization;

namespace Tango.UI
{
	public abstract class ActionContext
	{
		public ActionContext(IServiceProvider requestServices)
		{
			AllArgs = new DynamicDictionary(StringComparer.OrdinalIgnoreCase);
			ActionArgs = new DynamicDictionary(StringComparer.OrdinalIgnoreCase);
			EventArgs = new DynamicDictionary(StringComparer.OrdinalIgnoreCase);
			FormData = new DynamicDictionary(StringComparer.OrdinalIgnoreCase);
			EventReceivers = new Dictionary<string, InteractionFlowElement>(StringComparer.OrdinalIgnoreCase);
			PersistentArgs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			RequestServices = requestServices;
			Resources = RequestServices.GetService(typeof(IResourceManager)) as IResourceManager;
			Routes = RequestServices.GetService(typeof(RoutesCollection)) as RoutesCollection;
		}

		public IServiceProvider RequestServices { get; protected set; }
		public IResourceManager Resources { get; protected set; }
		public RoutesCollection Routes { get; protected set; }

		public string RequestMethod { get; set; }
		public string RootReceiver { get; set; }
		public string Service { get; set; }
		public string Action { get; set; }
		public string Event { get; set; }
		public string EventReceiver { get; set; }

		public DynamicDictionary AllArgs { get; set; }
		public DynamicDictionary ActionArgs { get; set; }
		public DynamicDictionary EventArgs { get; set; }

		public dynamic FormBag { get { return FormData; } }
		public DynamicDictionary FormData { get; set; }

		public IDictionary<string, string> PersistentArgs { get; }
		public IDictionary<string, InteractionFlowElement> EventReceivers { get; private set; }
	}

	public class PostedFileInfo
	{
		public string FileName { get; set; }
		public byte[] FileBytes { get; set; }
	}

	public class RoutesCollection : Dictionary<string, string> { }

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
			int res = 0;
			bool b = ctx.AllArgs.TryGetValue(name, out s);
			if (b) b = int.TryParse(s.ToString(), out res);
			if (b) return res;
			return defaultValue;
		}
		public static int? GetIntArg(this ActionContext ctx, string name)
		{
			object s = null;
			int res = 0;
			bool b = ctx.AllArgs.TryGetValue(name, out s);
			if (b) b = int.TryParse(s.ToString(), out res);
			if (b) return res;
			return null;
		}
		public static Guid GetGuidArg(this ActionContext ctx, string name, Guid defaultValue)
		{
			object s = null;
			Guid res = Guid.Empty;
			bool b = ctx.AllArgs.TryGetValue(name, out s);
			if (b) b = Guid.TryParse(s.ToString(), out res);
			if (b) return res;
			return defaultValue;
		}
		public static Guid? GetGuidArg(this ActionContext ctx, string name)
		{
			object s = null;
			Guid res = Guid.Empty;
			bool b = ctx.AllArgs.TryGetValue(name, out s);
			if (b) b = Guid.TryParse(s.ToString(), out res);
			if (b) return res;
			return null;
		}

		public static DateTime GetDateTimeArg(this ActionContext ctx, string name, DateTime defaultValue)
		{
			return ctx.GetDateTimeArg(name) ?? defaultValue;
		}

		public static DateTime GetDateTimeArg(this ActionContext ctx, string name, string format, DateTime defaultValue)
		{
			return ctx.GetDateTimeArg(name, format) ?? defaultValue;
		}

		public static DateTime? GetDateTimeArg(this ActionContext ctx, string name, string format = "yyyyMMdd")
		{
			object s = null;
			bool b = ctx.AllArgs.TryGetValue(name, out s);
			if (b)
			{
				DateTime dt;
				b = DateTime.TryParseExact(s.ToString(), format, null, DateTimeStyles.None, out dt);
				if (b)
					return dt;
				else
					return null;
			}
			return null;
		}


		public static T GetArg<T>(this ActionContext ctx, string name)
		{
			return ctx.AllArgs.Parse<T>(name);
		}
	}
}
