using System;
using System.Web.UI;
using System.Linq.Expressions;
using System.Web;
using System.IO;
using System.Xml.Linq;
using System.Reflection;
using System.Configuration;
using Nephrite.Identity;
using Nephrite.Web.Controls;

using Nephrite.Meta;
using Nephrite.Web.Office;
using Nephrite.Meta.Forms;

namespace Nephrite.Web
{
    public abstract class BaseController
    {
		public static void RedirectTo<TController>(Expression<Action<TController>> action) where TController : BaseController, new()
		{
		    RedirectTo<TController>(action, null);
		}

		public static void RedirectTo<TController>(Expression<Action<TController>> action, string anchor) where TController : BaseController, new()
		{
		    string url = HtmlHelperBase.Instance.ActionUrl<TController>(action);
		    if (anchor != null)
		        url += "&anchorlocation=" + anchor;

			/// HttpContext.Current.Request.Url.AbsoluteUri может не соответствовать реальному URL, если сервер сидит за прокси, которая меняет порт
			string newUrl = "";// HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "");
			if (!url.StartsWith("/"))
				url = "/" + url;
            newUrl = newUrl + url;
			HttpContext.Current.Response.Redirect(newUrl);
		}

		public void Update(object obj)
		{
			A.Model.SubmitChanges();
		}
    }
}
