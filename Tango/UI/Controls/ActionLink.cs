using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Tango.Html;
using Tango.Localization;

namespace Tango.UI.Controls
{
	public abstract class ActionUrl<T>
	{
		protected T _this;
		protected IUrlResolver _resolver;

		IDictionary<string, string> _args = new Dictionary<string, string>();
		IDictionary<string, string> _eventArgs = new Dictionary<string, string>();

		string _url = null;

		public ActionContext Context { get; private set; }

		public ActionUrl(ActionContext context)
		{
			Context = context;
		}

		public override string ToString()
		{
			if (_resolver == null) return "";
			if (_url == null)
			{
				StringBuilder sb = _resolver.Resolve(_args);
				if (_eventArgs.Count > 0)
				{
					sb.Append("#").Append(_resolver.Resolve(_eventArgs, true));
				}
				_url = sb.ToString();
			}

			return _url;
		}

		public static implicit operator string(ActionUrl<T> l)
		{
			if (l == null) return null;
			return l.ToString();
		}

		public T UseResolver(IUrlResolver resolver)
		{
			_resolver = resolver;
			return _this;
		}

		public T WithArgs(IDictionary<string, object> args)
		{
			foreach (var p in args)
				if (p.Value != null)
					_args[p.Key] = WebUtility.UrlEncode(p.Value.ToString());
				else
					_args.Remove(p.Key);
			return _this;
		}

		public T WithEventArgs(IDictionary<string, object> args)
		{
			foreach (var p in args)
				if (p.Value != null)
					_eventArgs[p.Key] = WebUtility.UrlEncode(p.Value.ToString());
				else
					_eventArgs.Remove(p.Key);
			return _this;
		}

		public T WithArg(string key, string value)
		{
			if (value != null)
				_args[key] = WebUtility.UrlEncode(value);
			else
				_args.Remove(key);
			return _this;
		}

		public T WithEventArg(string key, string value)
		{
			if (value != null)
				_eventArgs[key] = WebUtility.UrlEncode(value);
			else
				_eventArgs.Remove(key);
			return _this;
		}
	}

	public class ActionUrl : ActionUrl<ActionUrl>
	{
		
		public ActionUrl(ActionContext context) : base(context)
		{
			_this = this;
		}
	}

	public class ActionLink : ActionUrl<ActionLink>
	{
		string _title;
		string _imageSrc;

		public string GetTitle() => _title;
		public string GetImageSrc() => _imageSrc;
		public ITextResource TextResource { get; private set; }

		public ActionLink(ActionContext context, ITextResource textResource) : base(context)
		{
			TextResource = textResource;
			_this = this;
		}

		public ActionLink Title(string title)
		{
			_title = title;
			return this;
		}

		public ActionLink Image(string imageSrc)
		{
			_imageSrc = imageSrc;
			return this;
		}
	}
}