using System;
using System.Collections.Generic;
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

		ActionRequestType RequestType { get; set; }
		string RequestMethod { get; set; }
	}

	public class ActionTarget : IActionTarget
	{
		public string Service { get; set; }
		public string Action { get; set; }

		public string Event { get; set; }
		public string EventReceiver { get; set; }

		public Dictionary<string, string> Args { get; } = new Dictionary<string, string>();

		public ActionRequestType RequestType { get; set; } = ActionRequestType.Text;
		public string RequestMethod { get; set; } = "GET";
	}

	public class ActionLink : ActionTarget
	{
		protected IUrlResolver _resolver;


		string _url = null;
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
		public string Image { get; private set; }
		public string Description { get; private set; }

		public bool ChangeUrl { get; private set; } = true;

		public Dictionary<string, string> References { get; } = new Dictionary<string, string>();
		public (string Prefix, string Type) Container { get; private set; }

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
				_enabled = true;
				return;
			}
			if (_url == null)
			{
				if (!_condition)
				{
					_enabled = false;
					return;
				}

				//foreach (var arg in Context.PersistentArgs)
				//{
				//	if (!Args.ContainsKey(arg.Key))
				//		Args.Add(arg.Key, arg.Value);
				//}

				var urlArgs = new Dictionary<string, string>(Args) {
					{ Constants.ServiceName, Service },
					{ Constants.ActionName, Action }
				};
				var r = _resolver.Resolve(urlArgs, Context.AllArgs);
				if (r.Resolved)
				{
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
		{
			Image = imageSrc;
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

		public ActionLink InContainer(string prefix, string type)
		{
			Container = (prefix, type);
			return this;
		}

		public ActionLink WithCallbackUrl(string url)
		{
			CallbackUrl = url;
			return this;
		}

		public ActionLink WithRef(string key, string value)
		{
			if (value != null)
				References[key] = value;
			else
				References.Remove(key);
			return this;
		}

		public ActionLink KeepTheSameUrl()
		{
			ChangeUrl = false;
			return this;
		}

	}

	public static class ActionTargetExtensions
	{
		public static T RunEvent<T>(this T target, Action<ApiResponse> action)
			where T : IActionTarget
		{
			target.RequestMethod = "GET";
			return target.SetEvent(action);
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

		public static T To<T>(this T target, string serviceName, string actionName)
			where T : IActionTarget
		{
			target.Service = serviceName;
			target.Action = actionName;
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

		public static T WithArg<T, TValue>(this T target, string key, TValue value)
			where T : IActionTarget
		{
			if (value != null)
				target.Args[key] = value.ToString();
			else
				target.Args.Remove(key);
			return target;
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