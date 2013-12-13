using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web
{
    public class UrlStack
    {
        const string HttpContextKey = "UrlStack";
        
        List<string> urls = new List<string>();


        public UrlStack(string url)
        {
            string returnUrl = url.GetQueryParameter("returnurl");
            while (returnUrl != String.Empty)
            {
                string u = HttpUtility.UrlDecode(returnUrl);
                urls.Add(u);
				u = "?" + u;
                returnUrl = u.GetQueryParameter("returnurl");
            }
        }

        public static string CreateReturnUrl(string url)
        {
            return HttpUtility.UrlEncode(url.Replace("/", "").Replace("?", ""));
        }

        public static string CreateReturnUrl()
        {
            return CreateReturnUrl(HttpContext.Current.Request.Url.Query);
        }

        public static UrlStack Instance
        {
            get 
            {
                var s = HttpContext.Current.Items[HttpContextKey];
                if (s is UrlStack)
                    return (UrlStack)s;
                HttpContext.Current.Items[HttpContextKey] = new UrlStack(HttpContext.Current.Request.Url.Query);
                return (UrlStack)HttpContext.Current.Items[HttpContextKey];
            }
        }

        public string GetReturnUrl(int level)
        {
            if (level < urls.Count)
                return HttpContext.Current.Request.Url.AbsolutePath + "?" + urls[level];
            else
                return String.Empty;
        }

        public int ReturnUrlCount
        {
            get { return urls.Count; }
        }

        public static string GetReturnUrl()
        {
            return Instance.GetReturnUrl(0);
        }
    }
}
