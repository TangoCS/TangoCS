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

		public Guid? RequestID { get; set; }
		public string RequestMethod { get; set; }
		public string RootReceiver { get; set; }
		public string Service { get; set; }
		public string Action { get; set; }
		public string Event { get; set; }
		public string EventReceiver { get; set; }
		public string Sender { get; set; }
		public bool IsFirstLoad { get; set; }

		public DynamicDictionary AllArgs { get; set; }
		public DynamicDictionary ActionArgs { get; set; }
		public DynamicDictionary EventArgs { get; set; }

		public dynamic FormBag { get { return FormData; } }
		public DynamicDictionary FormData { get; set; }

		public IDictionary<string, string> PersistentArgs { get; }
		public IDictionary<string, InteractionFlowElement> EventReceivers { get; private set; }

		protected void ParseQueryParms(IReadOnlyDictionary<string, string> d)
		{
			Action<string, string> addArg = (k, v) => ActionArgs.Add(k, v);

			foreach (var arg in d)
			{
				var key = arg.Key;
				var value = arg.Value;

				if (key == Constants.ServiceName)
				{
					Service = value.ToString();
					if (Service != null && Service.Contains("."))
						throw new ArgumentException("Incorrect character '.' in Service parameter");
				}
				else if (key == Constants.ActionName)
				{
					Action = value.ToString();
					if (Action != null && Action.Contains("."))
						throw new ArgumentException("Incorrect character '.' in Action parameter");
				}
				else if (key == Constants.RootReceiverName)
					RootReceiver = value.ToString();
				else if (key == Constants.FirstLoad)
					IsFirstLoad = value.ToLower() == "true";
				else if (key == Constants.Sender)
					Sender = value.ToLower();
				else if (key == Constants.EventName)
				{
					addArg = (k, v) => {
						EventArgs.Add(k, v);
						if (!FormData.ContainsKey(k)) FormData.Add(k, v);
					};
					Event = value.ToString().ToLower();
				}
				else if (key == Constants.EventReceiverName)
				{
					EventReceiver = value.ToString().ToLower();
				}
				else
				{
					addArg(key, value);
					AllArgs.Add(key, value);
				}
			}
		}
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
			bool b = ctx.AllArgs.TryGetValue(name, out object s);
			if (b) return WebUtility.UrlDecode(s.ToString());
			return null;
		}
		public static int GetIntArg(this ActionContext ctx, string name, int defaultValue)
		{
			int res = 0;
			bool b = ctx.AllArgs.TryGetValue(name, out object s);
			if (b) b = int.TryParse(s.ToString(), out res);
			if (b) return res;
			return defaultValue;
		}
		public static int? GetIntArg(this ActionContext ctx, string name)
		{
			int res = 0;
			bool b = ctx.AllArgs.TryGetValue(name, out object s);
			if (b) b = int.TryParse(s.ToString(), out res);
			if (b) return res;
			return null;
		}
		public static Guid GetGuidArg(this ActionContext ctx, string name, Guid defaultValue)
		{
			Guid res = Guid.Empty;
			bool b = ctx.AllArgs.TryGetValue(name, out object s);
			if (b) b = Guid.TryParse(s.ToString(), out res);
			if (b) return res;
			return defaultValue;
		}
		public static Guid? GetGuidArg(this ActionContext ctx, string name)
		{
			Guid res = Guid.Empty;
			bool b = ctx.AllArgs.TryGetValue(name, out object s);
			if (b) b = Guid.TryParse(s.ToString(), out res);
			if (b) return res;
			return null;
		}
		public static bool GetBoolArg(this ActionContext ctx, string name, bool defaultValue)
		{
			if (ctx.AllArgs.TryGetValue(name, out object s))
				return s.ToString().In("true", "1");
			return defaultValue;
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
				b = DateTime.TryParseExact(s.ToString(), format, null, DateTimeStyles.None, out DateTime dt);
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
