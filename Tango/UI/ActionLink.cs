using System;
using System.Collections.Generic;
using System.Net;
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

		Dictionary<string, string> Args { get; }
		Dictionary<string, string> HashArgs { get; }

		ActionRequestType RequestType { get; set; }
	}

	public class ActionTarget : IActionTarget
	{
		public string Service { get; set; }
		public string Action { get; set; }

		public Dictionary<string, string> Args { get; } = new Dictionary<string, string>();
		public Dictionary<string, string> HashArgs { get; } = new Dictionary<string, string>();

		public ActionRequestType RequestType { get; set; } = ActionRequestType.Text;
	}

	public class ActionLink : ActionTarget
	{
		protected IUrlResolver _resolver;
		protected IUrlResolver _hashPartResolver = new RouteUrlResolver("", "/");

		Dictionary<int, ActionTarget> _callbacks = new Dictionary<int, ActionTarget>();

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
		public IReadOnlyDictionary<int, ActionTarget> Callbacks => _callbacks;

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
				_enabled = false;
				return;
			}
			if (_url == null)
			{
				if (!_condition)
				{
					_enabled = false;
					return;
				}

				foreach (var arg in Context.PersistentArgs)
				{
					if (!Args.ContainsKey(arg.Key))
						Args.Add(arg.Key, arg.Value);
				}

				var urlArgs = new Dictionary<string, string>(Args) {
					{ Constants.ServiceName, Service },
					{ Constants.ActionName, Action }
				};
				var r = _resolver.Resolve(urlArgs);
				if (r.Resolved)
				{
					if (HashArgs.Count > 0)
					{
						var hr = _hashPartResolver.Resolve(HashArgs);
						if (hr.Resolved)
							r.Result.Append("#").Append(hr.Result);
					}
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
		public ActionLink UseHashPartResolver(IUrlResolver resolver)
		{
			_hashPartResolver = resolver;
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

		public ActionLink Callback(int retValue, ActionTarget action)
		{
			_callbacks[retValue] = action;
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
		public static T To<T>(this T target, string serviceName, string actionName)
			where T: IActionTarget
		{
			target.Service = serviceName;
			target.Action = actionName;
			return target;
		}

		public static T WithArgs<T>(this T target, IDictionary<string, object> args)
			where T : IActionTarget
		{
			foreach (var p in args)
				if (p.Value != null)
					target.Args[p.Key] = p.Value.ToString();
				else
					target.Args.Remove(p.Key);
			return target;
		}

		public static T WithEventArgs<T>(this T target, IDictionary<string, object> args)
			where T : IActionTarget
		{
			foreach (var p in args)
				if (p.Value != null)
					target.HashArgs[p.Key] = p.Value.ToString();
				else
					target.HashArgs.Remove(p.Key);
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

		public static T WithEventArg<T, TValue>(this T target, string key, TValue value)
			where T : IActionTarget
		{
			if (value != null)
				target.HashArgs[key] = value.ToString();
			else
				target.HashArgs.Remove(key);
			return target;
		}


		public static T RemoveArg<T>(this T target, string key)
			where T : IActionTarget
		{
			target.Args.Remove(key);
			target.HashArgs.Remove(key);
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
	}
}