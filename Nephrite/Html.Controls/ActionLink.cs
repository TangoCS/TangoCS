using System;
using System.Collections.Generic;
using System.Text;

namespace Nephrite.Html.Controls
{
	public class ActionLink
	{
		protected IUrlResolver _urlResolver;
		protected IUrlResolver _eventUrlResolver;

		protected string _title;
		protected string _imageSrc;

		IDictionary<string, object> _args = new Dictionary<string, object>();
		IDictionary<string, object> _eventArgs = new Dictionary<string, object>();

		string _url = null;
		public string Url
		{
			get
			{
				if (_url == null)
				{
					StringBuilder sb = _urlResolver == null ? new StringBuilder() : _urlResolver.Resolve(_args);
					if (_eventArgs.Count > 0 && _eventUrlResolver != null)
					{
						sb.Append("#");
						sb.Append(_eventUrlResolver.Resolve(_eventArgs, true));
					}
                    _url = sb.ToString();
				}

				return _url;
			}
		}

		public ActionLink Title(string title)
		{
			if (_title.IsEmpty()) _title = title;
			return this;
		}

		public ActionLink Image(string image)
		{
			if (_imageSrc.IsEmpty()) _imageSrc = image;
			return this;
		}

		public ActionLink UseUrlResolver(IUrlResolver urlResolver)
		{
			_urlResolver = urlResolver;
			return this;
		}
		public ActionLink UseEventUrlResolver(IUrlResolver eventUrlResolver)
		{
			_eventUrlResolver = eventUrlResolver;
			return this;
		}

		public ActionLink WithArgs(IDictionary<string, object> args)
		{
			foreach (var p in args)
				_args.Add(p.Key, p.Value);
			return this;
		}
		public ActionLink WithEventArgs(IDictionary<string, object> args)
		{
			foreach (var p in args)
				_eventArgs.Add(p.Key, p.Value);
			return this;
		}

		public ActionLink WithArg(string key, string value)
		{
			_args.Add(key, value);
			return this;
		}
		public ActionLink WithEventArg(string key, string value)
		{
			_eventArgs[key] = value;
			return this;
		}

		[Obsolete]
		public string GetTitle()
		{
			return _title;
		}
		[Obsolete]
		public string GetImage()
		{
			return _imageSrc;
		}

		public static implicit operator string(ActionLink l)
		{
			if (l == null) return null;
			return l.ToString();
		}
	}

	public class ActionSimpleLink : ActionLink
	{
		Action<ATagAttributes> _aTagAttributes = null;

		public ActionSimpleLink Attr(Action<ATagAttributes> customATagAttributes)
		{
			_aTagAttributes = customATagAttributes;
			return this;
		}

		public override string ToString()
		{
			HtmlWriter w = new HtmlWriter();
			w.A(a => { a.Href(Url); if (_aTagAttributes != null) _aTagAttributes(a); }, _title);
			return w.ToString();
		}
	}

	public class ActionImageLink : ActionLink
	{
		Action<ATagAttributes> _aTagAttributes = null;
		Action<ATagAttributes> _aImgTagAttributes = null;
		Action<ImgTagAttributes> _imgTagAttributes = null;

		public ActionImageLink Attr(Action<ATagAttributes> customATagAttributes = null,
			Action<ATagAttributes> customAImgTagAttributes = null,
			Action<ImgTagAttributes> customImgTagAttributes = null)
		{
			_aTagAttributes = customATagAttributes;
			_aImgTagAttributes = customAImgTagAttributes;
			_imgTagAttributes = customImgTagAttributes;
			return this;
		}

		public override string ToString()
		{
			Action<ATagAttributes> a1Attr = a => { a.Href(Url); if (_aImgTagAttributes != null) _aImgTagAttributes(a); };
			Action<ImgTagAttributes> imgAttr = a => { a.Src(IconSet.RootPath + _imageSrc).Alt(_title); if (_imgTagAttributes != null) _imgTagAttributes(a); };
			Action<ATagAttributes> a2Attr = a => { a.Href(Url); if (_aTagAttributes != null) _aTagAttributes(a); };

			HtmlWriter w = new HtmlWriter();
			w.A(a1Attr, () => w.Img(imgAttr));
			w.Write("&nbsp;");
			w.A(a2Attr, _title);
			return w.ToString();
		}
	}

	public class ActionImage : ActionLink
	{
		Action<ATagAttributes> _aImgTagAttributes = null;
		Action<ImgTagAttributes> _imgTagAttributes = null;

		public ActionLink Attr(
			Action<ATagAttributes> customAImgTagAttributes = null,
			Action<ImgTagAttributes> customImgTagAttributes = null)
		{
			_aImgTagAttributes = customAImgTagAttributes;
			_imgTagAttributes = customImgTagAttributes;

			return this;
		}

		public override string ToString()
		{
			HtmlWriter w = new HtmlWriter();
			w.A(a => { a.Href(Url); if (_aImgTagAttributes != null) _aImgTagAttributes(a); }, 
				() => w.Img(a => { a.Src(IconSet.RootPath + _imageSrc); if (_imgTagAttributes != null) _imgTagAttributes(a); }));
			return w.ToString();
		}
	}
}