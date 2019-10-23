using Tango.Localization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Globalization;
using System.Linq;

namespace Tango.UI
{
	public abstract class ActionContext
	{
		public ActionContext(IServiceProvider requestServices)
		{
			AllArgs = new DynamicDictionary(StringComparer.OrdinalIgnoreCase);
			RouteArgs = new DynamicDictionary(StringComparer.OrdinalIgnoreCase);
			FormData = new DynamicDictionary(StringComparer.OrdinalIgnoreCase);
			EventReceivers = new List<IViewElement>();
			PersistentArgs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			ReturnUrl = new Dictionary<int, string>();
			ReturnTarget = new Dictionary<int, ActionTarget>();

			RequestServices = requestServices;
			Resources = RequestServices.GetService(typeof(IResourceManager)) as IResourceManager;
			Routes = RequestServices.GetService(typeof(RoutesCollection)) as RoutesCollection;
		}

		public IServiceProvider RequestServices { get; protected set; }
		public abstract IServiceScope CreateServiceScope();

		public IResourceManager Resources { get; protected set; }
		public RoutesCollection Routes { get; protected set; }
		public IDictionary<string, string> PersistentArgs { get; }
		public ICollection<IViewElement> EventReceivers { get; private set; }

		// request
		public Guid? RequestID { get; set; }
		public string RequestMethod { get; set; }
		public bool IsFirstLoad { get; set; }

		// target
		public string RootReceiver { get; set; }
		public string Service { get; set; }
		public string Action { get; set; }
		public string Event { get; set; }
		public string EventReceiver { get; set; }
		public string Sender { get; set; }
		public Dictionary<int, string> ReturnUrl { get; set; }
		public Dictionary<int, ActionTarget> ReturnTarget { get; set; }
		//public string SourceUrl { get; set; }

		// parameters
		public DynamicDictionary AllArgs { get; set; }
		public DynamicDictionary RouteArgs { get; set; }
		public DynamicDictionary FormData { get; set; }		

		// container
		public string ContainerType { get; set; }
		public string ContainerPrefix { get; set; }
		public bool AddContainer { get; set; }

		protected abstract ActionTarget ParseReturnUrl(string returnUrl);

		protected void ParseRouteParms(IReadOnlyDictionary<string, string> d)
		{
			ParseParms(d, (k, v) => RouteArgs.Add(k, v));
		}

		protected void ParseQueryParms(IReadOnlyDictionary<string, string> d)
		{
			ParseParms(d, (k, v) => AllArgs.Add(k, v));
		}

		protected void ProcessFormData()
		{
			ParseParms(FormData, (k, v) => FormData.ForEach((kv) => {
				if (!AllArgs.ContainsKey(kv.Key))
					AllArgs.Add(kv.Key, kv.Value);
				else
					AllArgs[kv.Key] = kv.Value;
			}));
		}

		void ParseParms<TValue>(IEnumerable<KeyValuePair<string, TValue>> d, Action<string, string> addArg)
		{
			foreach (var arg in d)
			{
				var key = arg.Key;
				var value = arg.Value.ToString();

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
				{
					IsFirstLoad = value.ToLower() == "true";
					AddContainer = AddContainer || IsFirstLoad;
				}
				else if (key == Constants.Sender)
					Sender = $"#{value.ToLower()}";
				else if (key == Constants.EventName)
				{
					Event = value.ToString().ToLower();
				}
				else if (key == Constants.EventReceiverName)
					EventReceiver = value.ToString().ToLower();
				else if (key == Constants.ReturnUrl)
					ReturnUrl[1] = value.ToString().ToLower();
				else if (key.StartsWith(Constants.ReturnUrl))
				{
					var parts = key.Split('_');
					if (parts.Length > 0)
						ReturnUrl[parts[1].ToInt32(0)] = value.ToString().ToLower();
				}
				//else if (key == "sourceurl")
				//	SourceUrl = value.ToString().ToLower();
				else if (key == Constants.ContainerType)
					ContainerType = value.ToString().ToLower();
				else if (key == Constants.ContainerPrefix)
					ContainerPrefix = value.ToString().ToLower();
				else if (key == Constants.ContainerNew)
					AddContainer = AddContainer || value == "1";
				else
				{
					addArg(key, value);
				}
			}
		}

		public ActionContext ReturnTargetContext(int code) => 
			ReturnTarget.ContainsKey(code) ? TargetContext(ReturnTarget[code]) : null;

		public ActionContext TargetContext(ActionTarget target)
		{
			var ctx = MemberwiseClone() as ActionContext;

			ctx.IsFirstLoad = false;
			ctx.Service = target.Service;
			ctx.Action = target.Action;
			ctx.Event = target.Event;
			ctx.EventReceiver = target.EventReceiver;
			ctx.RequestMethod = "GET";

			ctx.AddContainer = true;

			ctx.ReturnTarget = new Dictionary<int, ActionTarget>();
			ctx.ReturnUrl = new Dictionary<int, string>();

			ctx.AllArgs.Clear();
			ctx.FormData.Clear();

			foreach (var p in target.Args)
			{
				if (p.Key == Constants.ReturnUrl)
				{
					ctx.ReturnUrl[1] = p.Value;
					ctx.ReturnTarget[1] = ParseReturnUrl(p.Value);
					if (ctx.ReturnTarget[1].Args.TryGetValue(Constants.ReturnUrl, out string value))
						ctx.AllArgs.Add(p.Key, value);
				}
				else if (p.Key == Constants.EventName)
					ctx.Event = p.Value.ToString().ToLower();
				else if (p.Key == Constants.EventReceiverName)
					ctx.EventReceiver = p.Value.ToString().ToLower();
				else if (p.Key == Constants.ContainerNew)
					ctx.AddContainer = p.Value == "1";
				else
				{
					ctx.AllArgs.Add(p.Key, p.Value);
				}
			}

			ctx.Sender = null;
			ctx.ContainerType = null;
			ctx.ContainerPrefix = null;
			

			return ctx;
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
		public static T GetService<T>(this ActionContext ctx)
			where T:class
		{
			return ctx.RequestServices.GetService(typeof(T)) as T;
		}

		public static string GetArg(this ActionContext ctx, string name)
		{
			if (name == null) return null;
			bool b = ctx.AllArgs.TryGetValue(name, out object s);
			if (b) return WebUtility.UrlDecode(s.ToString());
			return null;
		}
		public static int GetIntArg(this ActionContext ctx, string name, int defaultValue)
		{
			if (name == null) return defaultValue;
			int res = 0;
			bool b = ctx.AllArgs.TryGetValue(name, out object s);
			if (b) b = int.TryParse(s.ToString(), out res);
			if (b) return res;
			return defaultValue;
		}
		public static int? GetIntArg(this ActionContext ctx, string name)
		{
			if (name == null) return null;
			int res = 0;
			bool b = ctx.AllArgs.TryGetValue(name, out object s);
			if (b) b = int.TryParse(s.ToString(), out res);
			if (b) return res;
			return null;
		}
		public static long GetLongArg(this ActionContext ctx, string name, int defaultValue)
		{
			if (name == null) return defaultValue;
			long res = 0;
			bool b = ctx.AllArgs.TryGetValue(name, out object s);
			if (b) b = long.TryParse(s.ToString(), out res);
			if (b) return res;
			return defaultValue;
		}
		public static long? GetLongArg(this ActionContext ctx, string name)
		{
			if (name == null) return null;
			long res = 0;
			bool b = ctx.AllArgs.TryGetValue(name, out object s);
			if (b) b = long.TryParse(s.ToString(), out res);
			if (b) return res;
			return null;
		}
		public static Guid GetGuidArg(this ActionContext ctx, string name, Guid defaultValue)
		{
			if (name == null) return defaultValue;
			Guid res = Guid.Empty;
			bool b = ctx.AllArgs.TryGetValue(name, out object s);
			if (b) b = Guid.TryParse(s.ToString(), out res);
			if (b) return res;
			return defaultValue;
		}
		public static Guid? GetGuidArg(this ActionContext ctx, string name)
		{
			if (name == null) return null;
			Guid res = Guid.Empty;
			bool b = ctx.AllArgs.TryGetValue(name, out object s);
			if (b) b = Guid.TryParse(s.ToString(), out res);
			if (b) return res;
			return null;
		}

		public static bool GetBoolArg(this ActionContext ctx, string name, bool defaultValue)
		{
			if (name == null) return defaultValue;
			if (ctx.AllArgs.TryGetValue(name, out object s))
				return s.ToString().ToLower().In("true", "1");
			return defaultValue;
		}
		public static bool? GetBoolArg(this ActionContext ctx, string name)
		{
			if (name == null) return null;
			if (ctx.AllArgs.TryGetValue(name, out object s))
				return s.ToString().ToLower().In("true", "1");
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

		public static DateTime? GetDateTimeArg(this ActionContext ctx, string name, string format = null)
		{
			if (name != null && ctx.AllArgs.TryGetValue(name, out object s))
			{
				var formats = new List<string> { ctx.GetArg($"__format_{name}"), "dd.MM.yyyy", "yyyy-MM-dd", "yyyyMMdd" }
					.Where(x => x != null).ToArray();

                if (DateTime.TryParseExact(s.ToString(), formats, null, DateTimeStyles.None, out DateTime dt))
					return dt;
			}
			return null;
		}


		public static T GetArg<T>(this ActionContext ctx, string name, T defaultValue = default)
		{
			return ctx.AllArgs.Parse<T>(name, defaultValue);
		}

		public static List<T> GetListArg<T>(this ActionContext ctx, string name)
		{
			return ctx.AllArgs.ParseList<T>(name);
		}
	}
}
