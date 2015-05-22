using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Collections.Specialized;
using Nephrite.Meta;
using Nephrite.Identity;
using Nephrite.AccessControl;
using Nephrite.Layout;
using Nephrite.Http;
using Nephrite.MVC;
using System.Text;

namespace Nephrite.Html.Controls
{
	public class ActionLink
	{
		public IAccessControl AccessControl { get; private set; }
		public IUrlHelper UrlHelper { get; private set; }

		public string Title { get; private set; }
		public string ImageSrc { get; private set; }

		public string RouteName { get; private set; }
		public string ControllerName { get; private set; }
		public string ActionName { get; private set; }

		public string SecurableObjectKey { get; private set; }
		public object PredicateContext { get; private set; }
		public bool CheckPredicateIfContextIsEmpty { get; private set; }
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
					if (PredicateContext != null)
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
			ControllerName = controllerName;
			ActionName = actionName;

			//_parameters.Add("bgroup", Url.GetString("bgroup"));
			_parameters.Add(MvcOptions.ControllerName, ControllerName);
			_parameters.Add(MvcOptions.ActionName, ActionName);
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

		Action<ATagAttributes> _aTagAttributes = null;
		Action<ATagAttributes> _aImgTagAttributes = null;
		Action<ImgTagAttributes> _imgTagAttributes = null;
		ActionLinkRenderType _type = ActionLinkRenderType.Link;

		public ActionLink Link(string title = null, Action<ATagAttributes> customATagAttributes = null)
		{
			if (!title.IsEmpty()) Title = title;

			_aTagAttributes = a => { a.Href = Url; if (customATagAttributes != null) customATagAttributes(a); };
			_type = ActionLinkRenderType.Link;

			return this;		
		}

		public ActionLink Image(string title = null, string image = null, 
			Action<ATagAttributes> customAImgTagAttributes = null,
			Action<ImgTagAttributes> customImgTagAttributes = null)
		{
			if (!title.IsEmpty()) Title = title;
			if (!image.IsEmpty()) ImageSrc = image;

			_aImgTagAttributes = a => { a.Href = Url; if (customAImgTagAttributes != null) customAImgTagAttributes(a); };
			_imgTagAttributes = a => { a.Src = IconSet.RootPath + ImageSrc; if (customImgTagAttributes != null) customImgTagAttributes(a); };

			_type = ActionLinkRenderType.Image;
			return this;			
		}

		public ActionLink ImageLink(string title = null, string image = null, 
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

			_type = ActionLinkRenderType.ImageLink;
			return this;		
		}

		public static implicit operator string(ActionLink l)
		{
			if (l == null) return null;
			return l.ToString();
		}

		public override string ToString()
		{
			HtmlWriter w = new HtmlWriter();
			switch (_type)
			{
				case ActionLinkRenderType.Link:
					w.A(_aTagAttributes, Title);
					return w.ToString();
				case ActionLinkRenderType.Image:					
					w.A(_aImgTagAttributes, () => w.Img(_imgTagAttributes));
					return w.ToString();
				case ActionLinkRenderType.ImageLink:
					w.A(_aImgTagAttributes, () => w.Img(_imgTagAttributes));
					w.Write("&nbsp;");
					w.A(_aTagAttributes, Title);
					return w.ToString();
				default:
					return "";
			}
		}
	}

	public enum ActionLinkRenderType
	{
		Link, Image, ImageLink
	}
}