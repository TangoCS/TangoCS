using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Tango.Html;
using Tango.Localization;

namespace Tango.UI
{
	public enum ActionRequestType
	{
		Text,
		ArrayBuffer
	}

	public interface IActionTarget
	{
		string Service { get; set; }
		string Action { get; set; }

		string Event { get; set; }
		string EventReceiver { get; set; }

		Dictionary<string, string> Args { get; }
		Dictionary<string, string> Data { get; }

		ActionRequestType RequestType { get; set; }
		string RequestMethod { get; set; }

		UrlResolverResult Resolve(ActionContext context, IUrlResolver resolver);
	}

	public class ActionTarget : IActionTarget
	{
		public string Service { get; set; }
		public string Action { get; set; }

		public string Event { get; set; }
		public string EventReceiver { get; set; }

		public string RouteTemplateName { get; set; }
		public Dictionary<string, string> Args { get; } = new Dictionary<string, string>();
		
		public Dictionary<string, string> Data { get; } = new Dictionary<string, string>();

		public ActionRequestType RequestType { get; set; } = ActionRequestType.Text;
		public string RequestMethod { get; set; } = "GET";

		public UrlResolverResult Resolve(ActionContext context, IUrlResolver resolver = null)
		{
			var urlArgs = new Dictionary<string, string>(Args);
			var routeInfo = context.Routes[RouteTemplateName ?? context.DefaultRouteTemplateName];
			var templateValue = RouteTemplateName == null && Service == null ? "" : routeInfo.Template;

			if (routeInfo.Parameters.Contains(Constants.ServiceName))
				urlArgs.AddIfNotExists(Constants.ServiceName, Service);
			if (routeInfo.Parameters.Contains(Constants.ActionName))
				urlArgs.AddIfNotExists(Constants.ActionName, Action);
			if (routeInfo.Parameters.Contains(Constants.Lang))
				urlArgs.AddIfNotExists(Constants.Lang, context.Lang);

			if ((RouteTemplateName ?? context.DefaultRouteTemplateName) == context.CurrentRoute.Name)
			{
				foreach (var arg in context.RouteArgs)
					urlArgs.AddIfNotExists(arg.Key, arg.Value?.ToString());
			}
			
			return (resolver ?? new RouteUrlResolver()).Resolve(templateValue, urlArgs);
		}
	}

	public class ActionLink : ActionTarget
	{
		protected string _url = null;
		protected IUrlResolver _resolver;
		
		string _title;
		Func<IResourceManager, string> _titleFunc = null;

		bool _enabled = false;
		bool _condition = true;

		public string Title
		{
			get
			{
				if (_titleFunc != null)
				{
					_title = _titleFunc(Context.Resources);
					_titleFunc = null;
				}
				return _title;
			}
		}
		public string ImageSrc { get; private set; }
        public string ImageTip { get; private set; }
        public string ImageColor { get; private set; }
        public string Description { get; private set; }

		public bool ChangeUrl { get; private set; } = true;
		public bool IsTargetBlank { get; private set; } = false;
		public bool HideDisabled { get; private set; } = false;

		public List<string> References { get; } = new List<string>();
		public (string Type, string Prefix, Dictionary<string, string> Parms) Container { get; private set; }

		public string CallbackUrl { get; private set; }

		public ActionContext Context { get; private set; }
		public IResourceManager Resources => Context.Resources;

		public ActionLink(ActionContext context)
		{
			Context = context;
		}

		public string Url {
			get {
				Resolve();
				return _url;
			}
		}

		public bool Enabled {
			get {
				Resolve();
				return _enabled;
			}			
		}

		void Resolve()
		{
			if (_resolver == null)
			{
				_enabled = _condition;
				return;
			}
			if (_url == null)
			{
				if (!_condition)
				{
					_enabled = false;
					return;
				}

				var r = Resolve(Context, _resolver);
				if (r.Resolved)
				{
					if (r.Result.Length == 0)
						r.Result.Append('/');
					else if (r.Result[0] != '/')
						r.Result.Insert(0, '/');
					_url = r.Result.ToString();
				}

				_enabled = r.Resolved;
			}
		}

		public override string ToString()
		{
			return Url;
		}

		public static implicit operator string(ActionLink l)
		{
			if (l == null) return null;
			return l.Url;
		}

		public ActionLink UseResolver(IUrlResolver resolver)
		{
			_resolver = resolver;
			return this;
		}

		public ActionLink WithTitle<T>(T title)
		{
			_title = title?.ToString();
			_titleFunc = null;
			return this;
		}

		public ActionLink WithTitle(Func<IResourceManager, string> title)
		{
			_titleFunc = title;
			return this;
		}

		public ActionLink WithImage(string imageSrc) 
			=> WithImage(imageSrc, null);

        public ActionLink WithImage(string imageSrc, string tip) 
			=> WithColorImage(imageSrc, null, tip);

        public ActionLink WithColorImage(string imageSrc, string color) 
			=> WithColorImage(imageSrc, color, null);

        public ActionLink WithColorImage(string imageSrc, string imageColor, string imageTip)
        {
            ImageSrc = imageSrc;
			ImageColor = imageColor;
			ImageTip = imageTip;
            return this;
        }

        public ActionLink WithDescription(string description)
		{
			Description = description;
			return this;
		}

		public ActionLink WithCondition(bool cond)
		{
			_condition = _condition && cond;
			return this;
		}

		public ActionLink WithHideDisabled(bool value)
		{
			HideDisabled = value;
			return this;
		}

		public ActionLink WithRequestMethod(string value)
		{
			RequestMethod = value;
			return this;
		}

		public ActionLink InContainer(string type, string prefix, Dictionary<string, string> parms)
		{
			Container = (type, prefix, parms);
			return this;
		}

		public ActionLink InContainer(Type type, string prefix, Dictionary<string, string> parms)
		{
			Container = (type.Name.Replace("Container", ""), prefix, parms);
			return this;
		}

		public ActionLink InDefaultContainer()
		{
			Container = ("default", null, null);
			return this;
		}

		public ActionLink WithCallbackUrl(string url)
		{
			CallbackUrl = url;
			return this;
		}

		public ActionLink WithRef(string id)
		{
			if (!References.Contains(id))
				References.Add(id);
			return this;
		}

		public ActionLink WithData(DataCollection coll)
		{
			foreach (var kv in coll)
				Data[kv.Key] = kv.Value;
			return this;
		}

		public ActionLink KeepTheSameUrl()
		{
			ChangeUrl = false;
			return this;
		}

		public ActionLink TargetBlank()
		{
			IsTargetBlank = true;
			return this;
		}

		public ActionLink WithTemplate(string name)
		{
			RouteTemplateName = name;
			return this;
		}

	}

	public static class ActionTargetExtensions
	{
		public static T RunEvent<T>(this T target, Action<ApiResponse> eventMethod)
			where T : IActionTarget
		{
			target.RequestMethod = "GET";
			return target.SetEvent(eventMethod);
		}

		public static T RunEvent<T>(this T target, string eventName, string eventReceiver = null)
			where T : IActionTarget
		{
			target.RequestMethod = "GET";
			target.Event = eventName;
			target.EventReceiver = eventReceiver;
			return target;
		}

		public static T PostEvent<T>(this T target, Action<ApiResponse> action)
			where T : IActionTarget
		{
			target.RequestMethod = "POST";
			return target.SetEvent(action);
		}

		public static T PostEvent<T>(this T target, Func<ActionResult> action)
			where T : IActionTarget
		{
			target.RequestMethod = "POST";
			return target.SetEvent(action);
		}

		public static T PostEvent<T>(this T target, string eventName, string eventReceiver = null)
			where T : IActionTarget
		{
			target.RequestMethod = "POST";
			target.Event = eventName;
			target.EventReceiver = eventReceiver;
			return target;
		}

		static T SetEvent<T>(this T target, Action<ApiResponse> action)
			where T: IActionTarget
		{
			var el = action.Target as ViewElement;
			if (el == null) throw new InvalidCastException("Invalid class type for action.Target; must be of type ViewElement");

			if (!el.ClientID.IsEmpty()) target.EventReceiver = el.ClientID;
			target.Event = action.Method.Name.ToLower();

			return target;
		}

		static T SetEvent<T>(this T target, Func<ActionResult> action)
			where T : IActionTarget
		{
			var el = action.Target as ViewElement;
			if (el == null) throw new InvalidCastException("Invalid class type for action.Target; must be of type ViewElement");

			if (!el.ClientID.IsEmpty()) target.EventReceiver = el.ClientID;
			target.Event = action.Method.Name.ToLower();

			return target;
		}

		public static T RunAction<T>(this T target, string serviceName, string actionName, string returnurl = null)
			where T : IActionTarget
		{
			target.Service = serviceName;
			target.Action = actionName;
			if (!returnurl.IsEmpty())
				target.WithArg("returnurl", returnurl);
			return target;
		}

		public static IActionTarget RunAction<T>(this IActionTarget target, string actionName, string returnurl = null)
			where T : Controller
		{
			target.Service = typeof(T).Name.Replace("Controller", "");
			target.Action = actionName;
			if (!returnurl.IsEmpty())
				target.WithArg("returnurl", returnurl);
			return target;
		}

		public static IActionTarget WithReturnUrlToCurrent(this IActionTarget target, ActionContext context)
		{
			target.WithArg(Constants.ReturnUrl, context.BaseUrl().Url);
			return target;
		}

		public static T WithArgs<T, TValue>(this T target, IEnumerable<KeyValuePair<string, TValue>> args)
			where T : IActionTarget
		{
			foreach (var p in args)
				if (p.Value != null)
					target.Args[p.Key] = p.Value.ToString();
				else
					target.Args.Remove(p.Key);
			return target;
		}

		public static T WithData<T, TValue>(this T target, string key, TValue value)
			where T : IActionTarget
		{
			if (value != null)
				target.Data[key] = value.ToString();
			else
				target.Data.Remove(key);
			return target;
		}
		public static T WithArg<T, TValue>(this T target, string key, TValue value)
			where T : IActionTarget
		{
			if (value != null)
				target.Args[key] = value.ToString();
			else
				target.Args.Remove(key);
			return target;
		}

		public static T WithDateArg<T>(this T target, string key, DateTime value)
			where T : IActionTarget
		{
			return target.WithArg(key, value.ToString("yyyyMMdd"));
		}

		public static T WithDateTimeArg<T>(this T target, string key, DateTime value)
			where T : IActionTarget
		{
			return target.WithArg(key, value.ToString("yyyyMMddHHmm"));
		}

		public static T RemoveArg<T>(this T target, string key)
			where T : IActionTarget
		{
			target.Args.Remove(key);
			return target;
		}

		public static T WithRequestType<T>(this T target, ActionRequestType requestType)
			where T : IActionTarget
		{
			target.RequestType = requestType;
			return target;
		}

		public static T Set<T>(this T target, Action<T> attrs)
			where T : IActionTarget
		{
			attrs?.Invoke(target);
			return target;
		}

		public static ActionLink CallbackToCurrent(this ActionLink link)
		{
			link.WithCallbackUrl(link.Context.BaseUrl().Url);
			return link;
		}
	}
}