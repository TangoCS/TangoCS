using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using System.Reflection;


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

		/// <summary>
		/// Удалить параметры из URL
		/// </summary>
		/// <param name="str">Строка с исходным URL</param>
		/// <param name="parametername">Имена параметров, которые надо удалить</param>
		/// <returns>Строка с URL без перечисленных параметров</returns>
		public static string RemoveQueryParameter(this string str, params string[] parametername)
		{
			string query = str;
			if (query.IndexOf('?') < 0)
				return query;
			string path = query.Substring(0, query.IndexOf('?'));
			query = query.Substring(query.IndexOf('?'));

			for (int i = 0; i < parametername.Length; i++)
				query = Regex.Replace(query, "[?&]" + parametername[i] + "=(?<1>[^&]*)", "", RegexOptions.IgnoreCase);
			if (query.Length == 0)
				return path;
			if (query[0] != '?')
				return path + "?" + query.Substring(1);
			return path + query;
		}

		public static string AddQueryParameter(this string str, string param)
		{
			return str + (str.IndexOf('?') >= 0 ? "&" : "?") + param;
		}

		public static string AddQueryParameter(this string str, string param, string value)
		{
			return str + (str.IndexOf('?') >= 0 ? "&" : "?") + param + "=" + value;
		}
		public static string AddQueryParameter(this string str, string param, int value)
		{
			return str + (str.IndexOf('?') >= 0 ? "&" : "?") + param + "=" + value.ToString();
		}
		public static string AddQueryParameter(this string str, string param, Guid value)
		{
			return str + (str.IndexOf('?') >= 0 ? "&" : "?") + param + "=" + value.ToString();
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

		public static string GetQueryParameter(this string querystring, string parameterName)
		{
			if (querystring.IndexOf('?') < 0)
				return String.Empty;

			querystring = querystring.Substring(querystring.IndexOf('?'));

			Match m = Regex.Match(querystring, "[?&]" + parameterName + "=(?<1>[^&]*)", RegexOptions.IgnoreCase);
			if (querystring[0] != '?')
				return "?" + querystring.Substring(1);
			return m.Groups[1].Value;
		}

		public static string GetString(string parametername)
		{
			return HttpContext.Current!=null ? HttpContext.Current.Request.Url.PathAndQuery.GetQueryParameter(parametername):"";
		}

		public static string CreateReturnUrl()
		{
			return Url.CreateReturnUrl();
		}

		public static string CreateReturnUrl(string urlquery)
		{
			return Url.CreateReturnUrl(urlquery);
		}

		public static string GetReturnUrl()
		{
			//return HttpContext.Current.Request.Url.AbsolutePath + "?" + GetString("returnurl").Replace("-e*", "=").Replace("-a*", "&").Replace("-n*", "#").Replace("-t*", "*");
			if (Url.Current.ReturnUrl == null)
				return Url.Current;
			return Url.Current.ReturnUrl;// UrlStack.GetReturnUrl();
		}

		public static void Redirect()
		{
			HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Url.PathAndQuery);
		}

		public static void RedirectBack()
		{
			if (Url.Current.ReturnUrl != null)
				Url.Current.ReturnUrl.Go();
			Url.Current.Go();
			//HttpContext.Current.Response.Redirect(UrlStack.GetReturnUrl());
		}
	}
}
