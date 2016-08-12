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

		public string Title => _title;
		public string Image => _imageSrc;
		public string Description => _description;

		public ActionContext Context { get; private set; }
		public ITextResource TextResource => Context.TextResource;

		public ActionLink(ActionContext context)
		{
			Context = context;
		}

		public string Url {
			get {
				if (_resolver == null) return null;
				if (_url == null)
				{
					foreach (var arg in Context.PersistentArgs)
					{
						if (!_args.ContainsKey(arg.Key))
							_args.Add(arg.Key, arg.Value);
					}

					StringBuilder sb = _resolver.Resolve(_args);
					if (_eventArgs.Count > 0)
					{
						sb.Append("#").Append(_hashPartResolver.Resolve(_eventArgs));
					}
					_url = sb.ToString();
				}
				return _url;
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

		public ActionLink WithArg(string key, string value)
		{
			if (value != null)
				_args[key] = WebUtility.UrlEncode(value);
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
	}
}