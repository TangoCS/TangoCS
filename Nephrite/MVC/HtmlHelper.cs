using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Configuration;
using Nephrite.Meta;
using Nephrite.Identity;
using Nephrite.Multilanguage;
using Nephrite.AccessControl;
using Nephrite.Http;
using Nephrite.Html.Controls;
using Nephrite.Html;

namespace Nephrite.MVC
{
	public class HtmlHelper
	{
		public IAccessControl AccessControl { get; set; }
		public IUrlHelper UrlHelper { get; private set; }

		public HtmlHelper(IUrlHelper urlHelper, IAccessControl accessControl)
		{
			UrlHelper = urlHelper;
			AccessControl = accessControl;
		}

		public void Repeater<T>(IEnumerable<T> items, Action<T> render)
		{
			HtmlRepeaters.Repeater(items, render);
		}
		
		public void Repeater<T>(IEnumerable<T> items,
			string className,
			string classNameAlt,
			Action<T, string> render)
		{
			HtmlRepeaters.Repeater(items, className, classNameAlt, render);
		}

		public void GroupingRepeater<T>(IEnumerable<T> items,
			string className,
			string classNameAlt,
			Func<T, object> GroupBy1Property,
			Func<T, object> GroupBy2Property,
			bool Group1Asc,
			bool Group2Asc,
			Action<T, string> renderItems,
			Action<T, string> renderGroup1,
			Action<T, string> renderGroup2) where T : IWithTitle
		{
			HtmlRepeaters.GroupingRepeater(items, className, classNameAlt, GroupBy1Property, GroupBy2Property, Group1Asc, Group2Asc, renderItems, renderGroup1, renderGroup2);
		}

		public ActionLink ActionLink(string title = null, Action<ATagAttributes> customATagAttributes = null)
		{
			return new ActionLink(UrlHelper, AccessControl).UseRoute("default").Link(title, customATagAttributes);
		}
		public ActionLink ActionOpenDialogLink(DialogOptions options, string title = null, Action<ATagAttributes> customATagAttributes = null)
		{
			return new ActionLink(UrlHelper, AccessControl).UseRoute("api").OpenDialogLink(options, title, customATagAttributes);
		}

		public ActionLink ActionImage(string title = null, string image = null,
			Action<ATagAttributes> customAImgTagAttributes = null,
			Action<ImgTagAttributes> customImgTagAttributes = null)
		{
			return new ActionLink(UrlHelper, AccessControl).UseRoute("default").Image(title, image, customAImgTagAttributes, customImgTagAttributes);
		}
		public ActionLink ActionImageLink(string title = null, string image = null,
			Action<ATagAttributes> customATagAttributes = null,
			Action<ATagAttributes> customAImgTagAttributes = null,
			Action<ImgTagAttributes> customImgTagAttributes = null)
		{
			return new ActionLink(UrlHelper, AccessControl).UseRoute("default").ImageLink(title, image, customATagAttributes, customAImgTagAttributes, customImgTagAttributes);
		}

		public ActionLink RouteLink(string routeName, string title = null, Action<ATagAttributes> customATagAttributes = null)
		{
			return new ActionLink(UrlHelper, AccessControl).UseRoute(routeName).Link(title, customATagAttributes);
		}
		public ActionLink RouteImage(string routeName, string title = null, string image = null,
			Action<ATagAttributes> customAImgTagAttributes = null,
			Action<ImgTagAttributes> customImgTagAttributes = null)
		{
			return new ActionLink(UrlHelper, AccessControl).UseRoute(routeName).Image(title, image, customAImgTagAttributes, customImgTagAttributes);
		}
		public ActionLink RouteImageLink(string routeName, string title = null, string image = null,
			Action<ATagAttributes> customATagAttributes = null,
			Action<ATagAttributes> customAImgTagAttributes = null,
			Action<ImgTagAttributes> customImgTagAttributes = null)
		{
			return new ActionLink(UrlHelper, AccessControl).UseRoute(routeName).ImageLink(title, image, customATagAttributes, customAImgTagAttributes, customImgTagAttributes);
		}

		public string InternalLink(string onClick, string linkText)
		{
			return String.Format("<a href='#' onclick='{0}'>{1}</a>", onClick, linkText);
		}

		public string InternalImage(string onClick, string linkText, string image)
		{
			return String.Format("<a href='#' onclick='{0}'><img src='{3}{2}' alt='{1}' title='{1}' class='middle' /></a>", onClick, linkText, image, IconSet.RootPath);
		}

		public string InternalImageLink(string onClick, string linkText, string image)
		{
			return String.Format("<a href='#' onclick='{0}'><img src='{3}{2}' alt='{1}' title='{1}' class='middle' /></a>&nbsp;<a href='#' onclick='{0}'>{1}</a>", onClick, linkText, image, IconSet.RootPath);
		}

		public ActionLink ActionImageConfirm(string linkText, string image, string confirmString)
		{
			return ActionImage(linkText, image, a => a.OnClick = String.Format("javascript:return confirm('{0}')", confirmString));
		}

		public ActionLink ActionImageLinkConfirm(string linkText, string image, string confirmString)
		{
			return ActionImageLink(linkText, image, a => a.OnClick = String.Format("javascript:return confirm('{0}')", confirmString));
		}

		//public string ActionUrl<T>(Expression<Action<T>> action) where T : BaseController, new()
		//{
		//	return ActionUrl<T>(action, false);
		//}

		//public string ActionUrl<T>(Expression<Action<T>> action, bool addReturnUrl) where T : BaseController, new()
		//{
			
		//	MethodCallExpression body = action.Body as MethodCallExpression;

		//	if (body == null)
		//	{
		//		throw new InvalidOperationException("Expression must be a method call");
		//	}

		//	if (body.Object != action.Parameters[0])
		//	{
		//		throw new InvalidOperationException("Method call must target lambda argument");
		//	}

		//	string actionName = body.Method.Name;
		//	Type t = typeof(T);
		//	string mode = "";
		//	if (t.IsGenericType)
		//	{
		//		mode = t.Name + ";" + t.GetGenericArguments()[0].Name;
		//	}
		//	else
		//		mode = t.Name;

		//	if (mode.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
		//	{
		//		mode = mode.Remove(mode.Length - 10, 10);
		//	}

		//	if (ConfigurationManager.AppSettings["DisableSPM"] == null)
		//	{
		//		string checkaction = actionName;
		//		object[] ca = body.Method.GetCustomAttributes(typeof(SecurableObjectKeyAttribute), true);
		//		if (ca != null && ca.Length == 1)
		//		{
		//			var san = ca[0] as SecurableObjectKeyAttribute;
		//			checkaction = san.Name;
		//		}
		//		//MetaOperation mo = Base.Meta.GetOperation(mode, checkaction);
		//		if (!String.IsNullOrEmpty(checkaction) && !ActionAccessControl.Instance.Check(mode + "." + checkaction)) return "#";
		//		//if (!AppSPM.AccessRightManager.Check(mode, checkaction)) return "#";
		//	}

		//	HtmlParms p = new HtmlParms();
		//	p.Add("action", actionName.ToLower());
		//	ParameterInfo[] parameters = body.Method.GetParameters();
		//	for (int i = 0; i < parameters.Length; i++)
		//	{
		//		object obj;
		//		Expression expression = body.Arguments[i];
		//		ConstantExpression expression2 = expression as ConstantExpression;
		//		if (expression2 != null)
		//		{
		//			obj = expression2.Value;
		//		}
		//		else
		//		{
		//			Expression<Func<object>> expression3 = Expression.Lambda<Func<object>>(Expression.Convert(expression, typeof(object)), new ParameterExpression[0]);
		//			try
		//			{
		//				obj = expression3.Compile()();
		//			}
		//			catch
		//			{
		//				obj = "";
		//			}
		//		}

		//		string pname = parameters[i].Name.ToLower();
		//		if (pname == "id")
		//			pname = "oid";
		//		if (obj == null)
		//			obj = "";
		//		p.Add(pname, obj.ToString());
		//	}

		//	if (AppWeb.IsRouting)
		//	{
		//		p.Add("mode", mode);
		//		p.Add("bgroup", UrlHelper.Current().GetString("bgroup"));
		//		if (p.ContainsKey("oid"))
		//		{
		//			return QueryHelpers.CreateUrl("{mode}/{action}/{oid}", p);
		//		}
		//		else
		//		{
		//			return QueryHelpers.CreateUrl("{mode}/{action}", p);
		//		}
		//	}
		//	else
		//	{
		//		List<string> remove = new List<string>();
		//		remove.AddRange(p.Keys.ToArray());
		//		remove.Add("anchorlocation");
		//		remove.Add("bgroup");
		//		remove.Add("returnurl");
		//		remove.Add("page");

		//		string result = "";
		//		if (Query.GetString("mode").ToLower() == mode.ToLower())
		//		{
		//			result = UrlHelper.Current().RemoveParameter(remove.ToArray()).ToString();
		//			result = result.Substring(result.IndexOf('?'));
		//			if (ConfigurationManager.AppSettings["ActionPage"] != null)
		//				result = ConfigurationManager.AppSettings["ActionPage"] + result;
		//		}
		//		else
		//		{
		//			if (ConfigurationManager.AppSettings["ActionPage"] != null)
		//				result += ConfigurationManager.AppSettings["ActionPage"];

		//			result += "?mode=" + mode;
		//		}

		//		foreach (var parm in p)
		//			result += "&" + parm.Key + "=" + parm.Value;

		//		if (Query.GetString("bgroup") != String.Empty)
		//		{
		//			result = result + "&bgroup=" + Query.GetString("bgroup");
		//		}
		//		if (Query.GetString("lang") != String.Empty)
		//		{
		//			result = result + "&lang=" + Query.GetString("lang");
		//		}
		//		if (addReturnUrl)
		//		{
		//			result = result + "&returnurl=" + Query.CreateReturnUrl();
		//		}
		//		return DefaultPath + result;
		//	}

		//}

		//public string ActionLink<T>(Expression<Action<T>> action, string text) where T : BaseController, new()
		//{
		//	string url = ActionUrl<T>(action);
		//	if (url == "#") return text;
		//	if (String.IsNullOrEmpty(text))
		//		text = TextResource.Get("Common.Label.EmptyLink");
		//	if (String.IsNullOrEmpty(text))
		//		return String.Format(@"<a href='{0}'>{1}</a>", url, url.Length > 15 ? url.Substring(0, 15) + "…" : url);
		//	else
		//		return String.Format(@"<a href='{0}'>{1}</a>", url, text);
		//}
		//public string ActionImageLink<T>(Expression<Action<T>> action, string text, string image) where T : BaseController, new()
		//{
		//	string url = ActionUrl<T>(action);
		//	if (url == "#") return text;
		//	return String.Format(@"<a href='{0}'><img src=""" + Settings.ImagesPath + @"{2}"" alt=""{1}"" title=""{1}"" border=""0"" style=""border:0; vertical-align:middle;""/></a> <a href='{0}'>{1}</a>", url, text, image);
		//}

		//public string ActionLink<T>(Expression<Action<T>> action, string text, string image) where T : BaseController, new()
		//{
		//	string url = ActionUrl<T>(action);
		//	if (url == "#") return text;
		//	return String.Format(@"<a href=""{0}"" title=""{2}""><img src=""" + Settings.ImagesPath + @"{1}"" alt=""{2}"" title=""{2}"" border=""0"" style=""border:0; vertical-align:middle;""/></a> <a href=""{0}"">{2}</a>", url, image, text);
		//}

		//public string ActionImage<T>(Expression<Action<T>> action, string text, string image) where T : BaseController, new()
		//{
		//	string url = ActionUrl<T>(action);
		//	if (url == "#") return String.Empty;
		//	return String.Format(@"<a href=""{0}"" title=""{2}""><img src=""" + Settings.ImagesPath + @"{1}"" alt=""{2}"" title=""{2}"" border=""0"" style=""border:0; vertical-align:middle;""/></a>", url, image, text);
		//}

		//public string ImageUrl(string url, string text, string image)
		//{
		//	return String.Format(@"<a href=""{0}"" title=""{2}""><img src=""" + Settings.ImagesPath + @"{1}"" alt=""{2}"" title=""{2}"" border=""0"" style=""border:0; vertical-align:middle;""/></a>", url, image, text);
		//}
		//public string ActionUrl(string url, string text)
		//{
		//	return String.Format(@"<a href=""{0}"" title=""{1}"">{1}</a>", url, text);
		//}

		//public string ActionImageConfirm<T>(Expression<Action<T>> action, string text, string image, string confirmString) where T : BaseController, new()
		//{
		//	string url = ActionUrl<T>(action);
		//	if (url == "#") return String.Empty;
		//	return String.Format(@"<a href=""{0}"" onclick=""javascript:return confirm('{3}')""><img src=""" + Settings.ImagesPath + @"{1}"" alt=""{2}"" title=""{2}"" border=""0"" style=""border:0; vertical-align:middle;""/></a>", url, image, text, confirmString);
		//}
		//public string ActionLinkConfirm<T>(Expression<Action<T>> action, string text, string image, string confirmString) where T : BaseController, new()
		//{
		//	string url = ActionUrl<T>(action);
		//	if (url == "#") return String.Empty;
		//	return String.Format(@"<a href=""{0}"" onclick=""javascript:return confirm('{3}')""><img src=""" + Settings.ImagesPath + @"{1}"" alt=""{2}"" title=""{2}"" border=""0"" style=""border:0; vertical-align:middle;""/></a> <a href=""{0}"" onclick=""javascript:return confirm('{3}')"">{2}</a>", url, image, text, confirmString);
		//}

		//public string InternalLink(string onClick, string text, bool accessRights)
		//{
		//	if (accessRights)
		//		return String.Format(@"<a href='#' onclick=""{0}"">{1}</a>", onClick, text);
		//	else
		//		return text;
		//}

		//public string InternalLink(string onClick, string text, string image, bool accessRights)
		//{
		//	if (accessRights)
		//		return String.Format(@"<a href='#' onclick=""{0}""><img src='" + Settings.ImagesPath + "{2}' alt='{1}' title='{1}' class='middle' /></a>", onClick, text, image);
		//	else
		//		return String.Empty;
		//}

		//public string InternalActionLink(string onClick, string text, string image, bool accessRights)
		//{
		//	if (accessRights)
		//		return String.Format(@"<a href='#' onclick=""{0}""><img src='" + Settings.ImagesPath + "{2}' alt='{1}' title='{1}' class='middle' /></a>", onClick, text, image) +
		//			String.Format(@" <a href='#' onclick=""{0}"">{1}</a>", onClick, text);
		//	else
		//		return String.Empty;
		//}

		//public string Image(string src, string alt)
		//{
		//	// при необходимости сделать метод DBImage
		//	//if (src.ToLower().Contains("data.ashx"))
		//	//	return String.Format("<img src='{0}' class='middle' alt='{1}' title='{1}' />", DataHandler.GetDataUrl(src.GetQueryParameter("oid").ToInt32(0)), alt);
		//	//if (src.IndexOf('/', 1) > 0 && !src.StartsWith(".."))
		//	//	return String.Format("<img src='{0}' class='middle' alt='{1}' title='{1}' />", DataHandler.GetDataUrl(src), alt);
		//	return String.Format("<img src='{0}' class='middle' alt='{1}' title='{1}' />", Settings.ImagesPath + src, alt);
		//}

		//public string InternalImageLink(string onClick, string text, string image)
		//{
		//	//if (image.ToLower().Contains("data.ashx"))
		//	//	return String.Format(@"<a href=""#"" onclick=""javascript:{0}""><img src=""{2}"" alt=""{1}"" title=""{1}"" border=""0"" style=""border:0; vertical-align:middle;""/></a>", onClick, text, DataHandler.GetDataUrl(image.GetQueryParameter("oid").ToInt32(0)));
		//	//if (image.IndexOf('/', 1) > 0 && !image.StartsWith(".."))
		//	//	return String.Format(@"<a href=""#"" onclick=""javascript:{0}""><img src=""{2}"" alt=""{1}"" title=""{1}"" border=""0"" style=""border:0; vertical-align:middle;""/></a>", onClick, text, DataHandler.GetDataUrl(image));
		//	return String.Format(@"<a href=""#"" onclick=""javascript:{0}""><img src=""" + Settings.ImagesPath + @"{2}"" alt=""{1}"" title=""{1}"" border=""0"" style=""border:0; vertical-align:middle;""/></a>", onClick, text, image);
		//}
    }


	public static class HtmlRepeaters
	{
		public static void Repeater<T>(IEnumerable<T> items, Action<T> render)
		{
			if (items == null)
				return;
			foreach (var item in items)
				render(item);
		}

		public static void Repeater<T>(IEnumerable<T> items,
			string className,
			string classNameAlt,
			Action<T, string> render)
		{
			if (items == null)
				return;

			int i = 0;

			foreach (var item in items)
				render(item, (i++ % 2 == 0) ? className : classNameAlt);
		}

		public static void GroupingRepeater<T>(IEnumerable<T> items,
			string className,
			string classNameAlt,
			Func<T, object> GroupBy1Property,
			Func<T, object> GroupBy2Property,
			bool Group1Asc,
			bool Group2Asc,
			Action<T, string> renderItems,
			Action<T, string> renderGroup1,
			Action<T, string> renderGroup2) where T : IWithTitle
		{
			if (items == null)
				return;

			int i = 0;

			Dictionary<string, Dictionary<string, List<T>>> list = new Dictionary<string, Dictionary<string, List<T>>>();

			foreach (var item in items)
			{
				string val1s = "";
				string val2s = "";
				if (GroupBy1Property != null)
				{
					object val1 = GroupBy1Property(item);

					if (val1 is IWithTitle)
					{
						val1s = ((IWithTitle)val1).Title;
					}
					else
					{
						if (val1 != null)
							val1s = val1.ToString();
					}
				}
				if (GroupBy2Property != null)
				{
					object val2 = GroupBy2Property(item);

					if (val2 is IWithTitle)
					{
						val2s = ((IWithTitle)val2).Title;
					}
					else
					{
						if (val2 != null)
							val2s = val2.ToString();
					}
				}
				if (!list.ContainsKey(val1s))
					list.Add(val1s, new Dictionary<string, List<T>>());

				if (!list[val1s].ContainsKey(val2s))
					list[val1s].Add(val2s, new List<T>());

				list[val1s][val2s].Add(item);
			}

			foreach (string k1 in Group1Asc ? list.Keys.OrderBy(k => k) : list.Keys.OrderByDescending(k => k))
			{
				bool rg1 = false;
				if (k1 != "")
					rg1 = true;
				foreach (string k2 in Group2Asc ? list[k1].Keys.OrderBy(k => k) : list[k1].Keys.OrderByDescending(k => k))
				{
					bool rg2 = false;
					if (k2 != "")
						rg2 = true;
					foreach (var item in list[k1][k2])
					{
						if (rg1)
						{
							renderGroup1(item, k1);
							rg1 = false;
						}
						if (rg2)
						{
							renderGroup2(item, k2);
							rg2 = false;
						}
						renderItems(item, (i++ % 2 == 0) ? className : classNameAlt);
					}
				}
			}
		}
	}


}
