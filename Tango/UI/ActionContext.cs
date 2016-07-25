using Tango.Localization;
using System;
using System.Collections.Generic;
using System.Net;

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
			TextResource = RequestServices.GetService(typeof(ITextResource)) as ITextResource;
			Routes = RequestServices.GetService(typeof(RoutesCollection)) as RoutesCollection;
		}

		public IServiceProvider RequestServices { get; protected set; }
		public ITextResource TextResource { get; protected set; }
		public RoutesCollection Routes { get; protected set; }

		public string Service { get; set; }
		public string Action { get; set; }
		public string RequestMethod { get; set; }
		public string Event { get; set; }
		public string EventReceiver { get; set; }

		public DynamicDictionary AllArgs { get; set; }
		public DynamicDictionary ActionArgs { get; set; }
		public DynamicDictionary EventArgs { get; set; }
		public IDictionary<string, string> PersistentArgs { get; }

		public dynamic FormBag { get { return FormData; } }
		public DynamicDictionary FormData { get; set; }

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
