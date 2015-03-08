using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using System.Reflection;
using Nephrite.Http;

namespace Nephrite.Web
{
	public static class Query
	{
		public static string RemoveParameter(params string[] parametername)
		{
			string query = "?" + HttpContext.Current.Request.QueryString.ToString();

			//string query = HttpContext.Current.Request.Url.AbsoluteUri;
			//if (query.IndexOf('?') < 0)
			//	return query;
			//string path = query.Substring(0, query.IndexOf('?'));
			//query = query.Substring(query.IndexOf('?'));

			for (int i = 0; i < parametername.Length; i++)
				query = Regex.Replace(query, "[?&]" + parametername[i] + "=(?<1>[^&]*)", "", RegexOptions.IgnoreCase);

			if (String.IsNullOrEmpty(query)) query = "?";
			if (query[0] != '?')
				return "?" + query.Substring(1);
			return query;
		}


		public static bool SortAsc
		{
			get
			{
				string sort = HttpContext.Current.Request.QueryString["sort"] ?? String.Empty;
				return !sort.ToLower().EndsWith("_desc");
			}
		}

		public static string Search
		{
			get { return (HttpContext.Current.Request.QueryString["search"] ?? String.Empty).ToLower(); }
		}

		public static int PageIndex
		{
			get
			{
				int page;
				if (Int32.TryParse(HttpContext.Current.Request.QueryString["page"], out page))
					return page > 0 ? page : 1;
				return 1;
			}
		}

		public static int GetInt(string parameterName, int defaultValue)
		{
			return HttpContext.Current.Request.QueryString[parameterName].ToInt32(defaultValue);
		}

		public static int? GetInt(string parameterName)
		{
			return HttpContext.Current.Request.QueryString[parameterName].ToInt32();
		}

		public static Guid GetGuid(string parameterName)
		{
			return HttpContext.Current.Request.QueryString[parameterName].ToGuid();
		}

		public static string SortColumn
		{
			get
			{
				string sort = HttpContext.Current.Request.QueryString["sort"] ?? String.Empty;
				return sort.ToLower().Replace("_desc", "");
			}
		}

		public static string GetString(string parametername)
		{
			return HttpContext.Current!=null ? HttpContext.Current.Request.Url.PathAndQuery.GetQueryParameter(parametername):"";
		}

		public static string CreateReturnUrl()
		{
			return UrlHelper.Current().CreateReturnUrl();
		}

		public static string CreateReturnUrl(string urlquery)
		{			
			return (new Url(urlquery)).CreateReturnUrl();
		}

		public static string GetReturnUrl()
		{
			var url = UrlHelper.Current();
			//return HttpContext.Current.Request.Url.AbsolutePath + "?" + GetString("returnurl").Replace("-e*", "=").Replace("-a*", "&").Replace("-n*", "#").Replace("-t*", "*");
			if (url.ReturnUrl == null)
				return url;
			return url.ReturnUrl;// UrlStack.GetReturnUrl();
		}

		public static void Redirect()
		{
			HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Url.PathAndQuery);
		}

		public static void RedirectBack()
		{
			var ret = UrlHelper.Current().ReturnUrl;
			if (ret != null)
				HttpContext.Current.Response.Redirect(ret);

			HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Url.PathAndQuery);
			//HttpContext.Current.Response.Redirect(UrlStack.GetReturnUrl());
		}
	}
}
