using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Tango.Html;
using Tango.Localization;

namespace Tango.UI.Controls
{
	public class ActionLink
	{
		protected IUrlResolver _resolver;
		protected IUrlResolver _hashPartResolver = new RouteUrlResolver("", "/");

		Dictionary<string, string> _args = new Dictionary<string, string>();
		Dictionary<string, string> _eventArgs = new Dictionary<string, string>();

		string _url = null;
		string _title;
		string _imageSrc;
		string _description;
		bool _enabled = false;
		bool _condition = true;

		public string Title => _title;
		public string Image => _imageSrc;
		public string Description => _description;

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
					if (!_args.ContainsKey(arg.Key))
						_args.Add(arg.Key, arg.Value);
				}

				var r = _resolver.Resolve(_args);
				if (r.Resolved)
				{
					if (_eventArgs.Count > 0)
					{
						var hr = _hashPartResolver.Resolve(_eventArgs);
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

		public ActionLink WithArgs(IDictionary<string, object> args)
		{
			foreach (var p in args)
				if (p.Value != null)
					_args[p.Key] = WebUtility.UrlEncode(p.Value.ToString());
				else
					_args.Remove(p.Key);
			return this;
		}

		public ActionLink WithEventArgs(IDictionary<string, object> args)
		{
			foreach (var p in args)
				if (p.Value != null)
					_eventArgs[p.Key] = WebUtility.UrlEncode(p.Value.ToString());
				else
					_eventArgs.Remove(p.Key);
			return this;
		}

		public ActionLink WithArg<T>(string key, T value)
		{
			if (value != null)
				_args[key] = WebUtility.UrlEncode(value.ToString());
			else
				_args.Remove(key);
			return this;
		}

		public ActionLink WithEventArg(string key, string value)
		{
			if (value != null)
				_eventArgs[key] = WebUtility.UrlEncode(value);
			else
				_eventArgs.Remove(key);
			return this;
		}

		public ActionLink WithTitle(string title)
		{
			_title = title;
			return this;
		}

		public ActionLink WithImage(string imageSrc)
		{
			_imageSrc = imageSrc;
			return this;
		}

		public ActionLink WithDescription(string description)
		{
			_description = description;
			return this;
		}

		public ActionLink WithCondition(bool cond)
		{
			_condition = _condition && cond;
			return this;
		}
	}
}