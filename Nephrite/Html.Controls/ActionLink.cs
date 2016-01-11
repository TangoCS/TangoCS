using System;
using System.Collections.Generic;
using System.Text;
using Nephrite.Multilanguage;
using Nephrite.Templating;

namespace Nephrite.Html.Controls
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
					_args[p.Key] = p.Value.ToString();
				else
					_args.Remove(p.Key);
			return _this;
		}

		public T WithEventArgs(IDictionary<string, object> args)
		{
			foreach (var p in args)
				if (p.Value != null)
					_eventArgs[p.Key] = p.Value.ToString();
				else
					_eventArgs.Remove(p.Key);
			return _this;
		}

		public T WithArg(string key, string value)
		{
			if (value != null)
				_args[key] = value;
			else
				_args.Remove(key);
			return _this;
		}

		public T WithEventArg(string key, string value)
		{
			if (value != null)
				_eventArgs[key] = value;
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





	//public abstract class AbstractActionLink<T>
	//{
	//	protected IUrlResolver _urlResolver;

	//	protected string _title;
	//	protected string _imageSrc;
	//	protected T _this;

	//	IDictionary<string, string> _args = new Dictionary<string, string>();
	//	IDictionary<string, string> _eventArgs = new Dictionary<string, string>();

	//	string _url = null;
	//	public string Url
	//	{
	//		get
	//		{
	//			if (_urlResolver == null) return "";
	//			if (_url == null)
	//			{
	//				StringBuilder sb = _urlResolver.Resolve(_args);
	//				if (_eventArgs.Count > 0)
	//				{
	//					sb.Append("#").Append(_urlResolver.Resolve(_eventArgs, true));
	//				}
 //                   _url = sb.ToString();
	//			}

	//			return _url;
	//		}
	//	}

	//	protected IHtmlWriter _writer;

	//	public T Title(string title)
	//	{
	//		if (_title.IsEmpty()) _title = title;
	//		return _this;
	//	}

	//	public T Image(string image)
	//	{
	//		if (_imageSrc.IsEmpty()) _imageSrc = image;
	//		return _this;
	//	}

	//	public T UseUrlResolver(IUrlResolver urlResolver)
	//	{
	//		_urlResolver = urlResolver;
	//		return _this;
	//	}


	//	public T WithArgs(IDictionary<string, object> args)
	//	{
	//		foreach (var p in args)
	//			if (p.Value != null)
	//				_args[p.Key] = p.Value.ToString();
	//			else
	//				_args.Remove(p.Key);
	//		return _this;
	//	}
	//	public T WithEventArgs(IDictionary<string, object> args)
	//	{
	//		foreach (var p in args)
	//			if (p.Value != null)
	//				_eventArgs[p.Key] = p.Value.ToString();
	//			else
	//				_eventArgs.Remove(p.Key);
	//		return _this;
	//	}

	//	public T WithArg(string key, string value)
	//	{
	//		if (value != null)
	//			_args[key] = value;
	//		else
	//			_args.Remove(key);
	//		return _this;
	//	}
	//	public T WithEventArg(string key, string value)
	//	{
	//		if (value != null)
	//			_eventArgs[key] = value;
	//		else
	//			_eventArgs.Remove(key);
	//		return _this;
	//	}

	//	public T SetWriter(IHtmlWriter writer)
	//	{
	//		_writer = writer;
	//		return _this;
	//	}

	//	public abstract void Render();

	//	public override string ToString()
	//	{
	//		var w = new HtmlWriter();
	//		SetWriter(w);
	//		Render();
	//		return w.ToString();
	//	}

	//	[Obsolete]
	//	public string GetTitle()
	//	{
	//		return _title;
	//	}
	//	[Obsolete]
	//	public string GetImage()
	//	{
	//		return _imageSrc;
	//	}

	//	public static implicit operator string(AbstractActionLink<T> l)
	//	{
	//		if (l == null) return null;
	//		return l.ToString();
	//	}
	//}

	//public class ActionLink : AbstractActionLink<ActionLink>
	//{
	//	Action<ATagAttributes> _attrs = null;

	//	public ActionLink()
	//	{
	//		_this = this;		
	//	}

	//	public ActionLink Attr(Action<ATagAttributes> aTagAttributes)
	//	{
	//		_attrs = aTagAttributes;
	//		return this;
	//	}

	//	public override void Render()
	//	{
	//		if (_writer != null)
	//			_writer.A(a => { a.Href(Url); if(_attrs != null) _attrs(a); }, () => {
	//				if (!_imageSrc.IsEmpty()) _writer.Img(a => a.Src(IconSet.RootPath + _imageSrc).Alt(_title).Class("linkicon"));
	//				_writer.Write(_title);
	//			});
	//	}
	//}

	//public class ActionImage : ActionLink
	//{
	//	public override void Render()
	//	{
	//		if (_writer != null)
	//			_writer.A(a => a.Href(Url), () => _writer.Img(a => a.Src(IconSet.RootPath + _imageSrc)));
	//	}
	//}
}