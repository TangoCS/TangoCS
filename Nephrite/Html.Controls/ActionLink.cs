using System;
using Nephrite.AccessControl;
using Nephrite.MVC;

namespace Nephrite.Html.Controls
{
	public class ActionLink
	{
		public IAccessControl AccessControl { get; protected set; }
		public IUrlHelper UrlHelper { get; protected set; }

		public string Title { get; protected set; }
		public string ImageSrc { get; protected set; }

		public string RouteName { get; protected set; }
		//public string ControllerName { get; private set; }
		//public string ActionName { get; private set; }

		public string SecurableObjectKey { get; protected set; }
		public object PredicateContext { get; protected set; }
		public bool CheckPredicateIfContextIsEmpty { get; protected set; }
		HtmlParms _parameters = new HtmlParms();

		public ActionLink(IUrlHelper urlHelper, IAccessControl accessControl)
		{
			AccessControl = accessControl;
			UrlHelper = urlHelper;
			CheckPredicateIfContextIsEmpty = false;
		}

		string _url = null;
		public string Url
		{
			get
			{
				if (_url != null) return _url;

				bool access = true;
				if (!SecurableObjectKey.IsEmpty())
					if (PredicateContext != null || CheckPredicateIfContextIsEmpty)
						access = AccessControl.CheckWithPredicate(SecurableObjectKey, PredicateContext).Value;
					else
						access = AccessControl.Check(SecurableObjectKey);

				if (access)
					_url = UrlHelper.GeneratePathAndQueryFromRoute(RouteName, _parameters) ?? "";
				else
					_url = "";
				return _url;
			}
		}


		public ActionLink UseRoute(string routeName)
		{
			RouteName = routeName;
			return this;
		}

		public ActionLink UseDefaults(string title, string image)
		{
			if (Title.IsEmpty()) Title = title;
			if (ImageSrc.IsEmpty()) ImageSrc = image;
			return this;
		}

		public ActionLink To(string controllerName, string actionName)
		{
			if (SecurableObjectKey.IsEmpty()) SecurableObjectKey = controllerName + "." + actionName;
			//ControllerName = controllerName;
			//ActionName = actionName;

			//_parameters.Add("bgroup", Url.GetString("bgroup"));
			_parameters.Add(MvcOptions.ControllerName, controllerName);
			_parameters.Add(MvcOptions.ActionName, actionName);
			return this;
		}

		public ActionLink Check(Func<ActionLink, string> securableObjectKey, object predicateContext = null, bool checkPredicateIfContextIsEmpty = false)
		{
			SecurableObjectKey = securableObjectKey(this);
			PredicateContext = predicateContext;
			CheckPredicateIfContextIsEmpty = checkPredicateIfContextIsEmpty;
			return this;
		}

		public ActionLink Check(object predicateContext, bool checkPredicateIfContextIsEmpty = false)
		{
			PredicateContext = predicateContext;
			CheckPredicateIfContextIsEmpty = checkPredicateIfContextIsEmpty;
			return this;
		}

		public ActionLink With(HtmlParms parametersValues)
		{
			foreach (var p in parametersValues)
				_parameters.Add(p.Key, p.Value);
			return this;
		}
		public ActionLink With(string key, string value)
		{
			_parameters.Add(key, value);
			return this;
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

		public ActionSimpleLink(IUrlHelper urlHelper, IAccessControl accessControl) : base(urlHelper, accessControl) { 	}

		public ActionSimpleLink Link(string title = null, Action<ATagAttributes> customATagAttributes = null)
		{
			if (!title.IsEmpty()) Title = title;
			_aTagAttributes = a => { a.Href = Url; if (customATagAttributes != null) customATagAttributes(a); };
			return this;
		}

		public override string ToString()
		{
			HtmlWriter w = new HtmlWriter();
			w.A(_aTagAttributes, Title);
			return w.ToString();
		}
	}

	public class ActionImageLink : ActionLink
	{
		Action<ATagAttributes> _aTagAttributes = null;
		Action<ATagAttributes> _aImgTagAttributes = null;
		Action<ImgTagAttributes> _imgTagAttributes = null;

		public ActionImageLink(IUrlHelper urlHelper, IAccessControl accessControl) : base(urlHelper, accessControl) { }

		public ActionImageLink ImageLink(string title = null, string image = null,
			Action<ATagAttributes> customATagAttributes = null,
			Action<ATagAttributes> customAImgTagAttributes = null,
			Action<ImgTagAttributes> customImgTagAttributes = null)
		{
			if (!title.IsEmpty()) Title = title;
			if (!image.IsEmpty()) ImageSrc = image;

			_aTagAttributes = a => { a.Href = Url; if (customATagAttributes != null) customATagAttributes(a); };
			_aImgTagAttributes = a =>
			{
				a.Href = Url;
				if (customATagAttributes != null && customAImgTagAttributes == null) customATagAttributes(a);
				if (customAImgTagAttributes != null) customAImgTagAttributes(a);
			};
			_imgTagAttributes = a => { a.Src = IconSet.RootPath + ImageSrc; a.Alt = Title; if (customImgTagAttributes != null) customImgTagAttributes(a); };

			return this;
		}

		public override string ToString()
		{
			HtmlWriter w = new HtmlWriter();
			w.A(_aImgTagAttributes, () => w.Img(_imgTagAttributes));
			w.Write("&nbsp;");
			w.A(_aTagAttributes, Title);
			return w.ToString();
		}
	}

	public class ActionImage : ActionLink
	{
		Action<ATagAttributes> _aImgTagAttributes = null;
		Action<ImgTagAttributes> _imgTagAttributes = null;

		public ActionImage(IUrlHelper urlHelper, IAccessControl accessControl) : base(urlHelper, accessControl) { }

		public ActionLink Image(string title = null, string image = null,
			Action<ATagAttributes> customAImgTagAttributes = null,
			Action<ImgTagAttributes> customImgTagAttributes = null)
		{
			if (!title.IsEmpty()) Title = title;
			if (!image.IsEmpty()) ImageSrc = image;

			_aImgTagAttributes = a => { a.Href = Url; if (customAImgTagAttributes != null) customAImgTagAttributes(a); };
			_imgTagAttributes = a => { a.Src = IconSet.RootPath + ImageSrc; if (customImgTagAttributes != null) customImgTagAttributes(a); };

			return this;
		}

		public override string ToString()
		{
			HtmlWriter w = new HtmlWriter();
			w.A(_aImgTagAttributes, () => w.Img(_imgTagAttributes));
			return w.ToString();
		}
	}
}