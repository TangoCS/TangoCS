﻿using Tango.Localization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Globalization;
using System.Security.Principal;
using Newtonsoft.Json;
using Tango.Html;

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
			Routes = RequestServices.GetService(typeof(NamedRouteCollection)) as NamedRouteCollection;
			DefaultRouteResolver = RequestServices.GetService(typeof(IDefaultRouteResolver)) as IDefaultRouteResolver;
		}

		public IServiceProvider RequestServices { get; protected set; }
		private IDefaultRouteResolver DefaultRouteResolver { get; }
		public abstract IServiceScope CreateServiceScope();

		public IResourceManager Resources { get; protected set; }
		public IDictionary<string, string> PersistentArgs { get; }
		public ICollection<IViewElement> EventReceivers { get; private set; }

		// routes
		public NamedRouteCollection Routes { get; protected set; }
		public RouteInfo CurrentRoute { get; set; }
		public string DefaultRouteTemplateName => DefaultRouteResolver?.Resolve(this) ?? "default";

		// request
		public Guid? RequestID { get; set; }
		public string RequestMethod { get; set; }
		public bool IsFirstLoad { get; set; }
		public bool IsLocalRequest { get; set; }

		// target
		public string RootReceiver { get; set; }
		public string Service { get; set; }
		public string Action { get; set; }
		public string Lang { get; set; }
		public string Event { get; set; }
		public string EventReceiver { get; set; }
		public AbsoluteID Sender { get; set; }
		public Dictionary<int, string> ReturnUrl { get; set; }
		public Dictionary<int, ActionTarget> ReturnTarget { get; set; }
		public string ReturnState { get; set; }
		//public string SourceUrl { get; set; }

		// parameters
		public DynamicDictionary AllArgs { get; set; }
		public DynamicDictionary RouteArgs { get; set; }
		public DynamicDictionary FormData { get; set; }

		// container
		public string ContainerType { get; set; }
		public string ContainerPrefix { get; set; }
		public bool AddContainer { get; set; }

		// response
		public string ResponseType { get; set; }

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
				var key = arg.Key.ToLower();
				var value = arg.Value?.ToString();

				if (key == Constants.ServiceName)
				{
					Service = value;
					if (Service != null && Service.Contains("."))
						throw new ArgumentException("Incorrect character '.' in Service parameter");
				}
				else if (key == Constants.ActionName)
				{
					Action = value;
					if (Action != null && Action.Contains("."))
						throw new ArgumentException("Incorrect character '.' in Action parameter");
				}
				else if (key == Constants.RootReceiverName)
					RootReceiver = value;
				else if (key == Constants.FirstLoad)
				{
					IsFirstLoad = value.ToLower() == "true";
					AddContainer = AddContainer || IsFirstLoad;
				}
				else if (key == Constants.Sender)
					Sender = value.ToLower();
				else if (key == Constants.EventName)
				{
					Event = value.ToLower();
				}
				else if (key == Constants.EventReceiverName)
					EventReceiver = value.ToLower();
				else if (key == Constants.ReturnUrl)
					ReturnUrl[1] = value.ToLower();
				else if (key == Constants.ReturnState)
					ReturnState = value;
				else if (key == Constants.ResponseType)
					ResponseType = value;
				else if (key.StartsWith(Constants.ReturnUrl))
				{
					var parts = key.Split('_');
					if (parts.Length > 0)
						ReturnUrl[parts[1].ToInt32(0)] = value.ToLower();
				}
				//else if (key == "sourceurl")
				//	SourceUrl = value.ToString().ToLower();
				else if (key == Constants.ContainerType)
					ContainerType = value.ToLower();
				else if (key == Constants.ContainerPrefix)
					ContainerPrefix = value.ToLower();
				else if (key == Constants.ContainerNew)
					AddContainer = AddContainer || value == "1";
				else if (key == Constants.Lang)
					Lang = value.ToLower();
				else
				{
					addArg(key, value);
				}
			}
		}

		public ActionContext ReturnTargetContext(int code) =>
			ReturnTarget.ContainsKey(code) ? TargetContext(ReturnTarget[code]) : null;

		public void SwitchToReturnTarget(ActionTarget target)
		{
			IsFirstLoad = false;
			Service = target.Service;
			Action = target.Action;
			Event = target.Event;
			EventReceiver = target.EventReceiver;

			AddContainer = true;

			ReturnTarget = new Dictionary<int, ActionTarget>();
			ReturnUrl = new Dictionary<int, string>();

			AllArgs.Clear();
			FormData.Clear();

			foreach (var p in target.Args)
			{
				if (p.Key == Constants.ReturnUrl)
				{
					ReturnUrl[1] = p.Value;
					ReturnTarget[1] = ParseReturnUrl(p.Value);
					if (ReturnTarget[1].Args.TryGetValue(Constants.ReturnUrl, out string value))
						AllArgs.Add(p.Key, value);
				}
				else if (p.Key == Constants.EventName)
					Event = p.Value.ToString().ToLower();
				else if (p.Key == Constants.EventReceiverName)
					EventReceiver = p.Value.ToString().ToLower();
				else if (p.Key == Constants.ContainerNew)
					AddContainer = p.Value == "1";
				else if (p.Key.StartsWith("~"))
				{
					AllArgs.Add(p.Key.Substring(1), p.Value);
					FormData.Add(p.Key.Substring(1), p.Value);
				}
				else
				{
					AllArgs.Add(p.Key, p.Value);
				}
			}

			Sender = null;
			ContainerType = null;
			ContainerPrefix = null;
		}

		public ActionContext TargetContext(ActionTarget target)
		{
			var ctx = MemberwiseClone() as ActionContext;

			ctx.SwitchToReturnTarget(target);

			//ctx.IsFirstLoad = false;
			//ctx.Service = target.Service;
			//ctx.Action = target.Action;
			//ctx.Event = target.Event;
			//ctx.EventReceiver = target.EventReceiver;
			//ctx.RequestMethod = "GET";

			//ctx.AddContainer = true;

			//ctx.ReturnTarget = new Dictionary<int, ActionTarget>();
			//ctx.ReturnUrl = new Dictionary<int, string>();

			//ctx.AllArgs.Clear();
			//ctx.FormData.Clear();

			//foreach (var p in target.Args)
			//{
			//	if (p.Key == Constants.ReturnUrl)
			//	{
			//		ctx.ReturnUrl[1] = p.Value;
			//		ctx.ReturnTarget[1] = ParseReturnUrl(p.Value);
			//		if (ctx.ReturnTarget[1].Args.TryGetValue(Constants.ReturnUrl, out string value))
			//			ctx.AllArgs.Add(p.Key, value);
			//	}
			//	else if (p.Key == Constants.EventName)
			//		ctx.Event = p.Value.ToString().ToLower();
			//	else if (p.Key == Constants.EventReceiverName)
			//		ctx.EventReceiver = p.Value.ToString().ToLower();
			//	else if (p.Key == Constants.ContainerNew)
			//		ctx.AddContainer = p.Value == "1";
			//	else if (p.Key.StartsWith("~"))
			//	{
			//		ctx.AllArgs.Add(p.Key.Substring(1), p.Value);
			//		ctx.FormData.Add(p.Key.Substring(1), p.Value);
			//	}
			//	else
			//	{
			//		ctx.AllArgs.Add(p.Key, p.Value);
			//	}
			//}

			//ctx.Sender = null;
			//ctx.ContainerType = null;
			//ctx.ContainerPrefix = null;
			

			return ctx;
		}
	}

	public class PostedFileInfo
	{
		public string FileName { get; set; }
		public byte[] FileBytes { get; set; }
	}

	public class NamedRouteCollection : Dictionary<string, string> { }

	public class RouteInfo
	{
		public string Name { get; set; }
		public string Template { get; set; }
	}

	public interface IDefaultRouteResolver
	{
		string Resolve(ActionContext context);
	}

	public static class ActionContextExtensions
	{
		public static T GetService<T>(this ActionContext ctx)
			where T:class
		{
			return ctx.RequestServices.GetService(typeof(T)) as T;
		}

		public static string GetArg(this ActionContext ctx, string name, bool decode = true)
		{
			if (name == null) return null;
			bool b = ctx.AllArgs.TryGetValue(name, out object s);
			if (b) return decode ? WebUtility.UrlDecode(s?.ToString()) : s?.ToString();
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
		public static long GetLongArg(this ActionContext ctx, string name, long defaultValue)
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

		public static decimal GetDecimalArg(this ActionContext ctx, string name, decimal defaultValue)
		{
			if (name == null) return defaultValue;
			decimal res = 0;
			bool b = ctx.AllArgs.TryGetValue(name, out object s);
			if (b)
			{
				var str = s.ToString();
				str = str.Replace(" ", "").Replace(",", str.Contains(".") ? "" : ".");
				b = decimal.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out res);
			}
			if (b) return res;
			return defaultValue;
		}
		public static decimal? GetDecimalArg(this ActionContext ctx, string name)
		{
			if (name == null) return null;
			decimal res = 0;
			bool b = ctx.AllArgs.TryGetValue(name, out object s);
			if (b)
			{
				var str = s.ToString();

                //первый Replace - замена обычно пробела, второй - замена &nbsp
				str = str.Replace(" ", "").Replace(" ", "").Replace(",", str.Contains(".") ? "" : ".");
				b = decimal.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out res);
			}
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
				return s.ToString().ToBoolean(defaultValue);
			return defaultValue;
		}
		public static bool? GetBoolArg(this ActionContext ctx, string name)
		{
			if (name == null) return null;
			if (ctx.AllArgs.TryGetValue(name, out object s))
				return s.ToString().ToBoolean(false);
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
				return s.ToString().ToDateTime(ctx.GetArg($"__format_{name}"), format);
			}
			return null;
		}

        public static byte[] GetBytesArg(this ActionContext ctx, string name)
        {
            if (name == null) return null;
            bool b = ctx.AllArgs.TryGetValue(name, out object s);
            if (b) return Convert.FromBase64String(s.ToString());
            return null;
        }

		/// <summary>
		/// Для типов int, long, decimal, string, bool, DateTime, Guid нужно использовать специализированные методы!
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ctx"></param>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static T GetArg<T>(this ActionContext ctx, string name, T defaultValue = default)
		{
			return ctx.AllArgs.Parse<T>(name, defaultValue);
		}

		public static List<T> GetListArg<T>(this ActionContext ctx, string name)
		{
			return ctx.AllArgs.ParseList<T>(name);
		}

		public static T GetJsonArg<T>(this ActionContext ctx, string name, Func<T> defaultValue = null)
		{
			var s = ctx.GetArg(name);
			s = WebUtility.HtmlDecode(s);
			T res = default;
			if (!s.IsEmpty()) res = JsonConvert.DeserializeObject<T>(s);
			if (res == null && defaultValue != null) res = defaultValue();
			return res;
		}

		public static ActionResult RunAction(this ActionContext ctx, ITypeActivatorCache cache = null, string key = null)
		{
			cache = cache ?? ctx.RequestServices.GetService(typeof(ITypeActivatorCache)) as ITypeActivatorCache;
			key = key ?? (ctx.Service + "." + ctx.Action);
			var view = cache.Get(key);
			if (view?.Args != null)
				foreach (var kv in view.Args)
					ctx.AllArgs.Add(kv.Key, kv.Value);
			return view?.Invoker?.Invoke(ctx, view.Type) ?? new HttpResult { StatusCode = HttpStatusCode.NotFound };
		}
	}

	public static class ActionContextIdentityExtension
	{
		public static bool IsCurrentUserAuthenticated(this ActionContext context)
		{
			return context.RequestServices.GetService(typeof(IIdentity)) is IIdentity identity && identity.IsAuthenticated;
		}
	}

}
