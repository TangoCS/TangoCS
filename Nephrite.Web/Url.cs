using System.Collections.Specialized;
using System.Web.UI;
using System.Web;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Nephrite.Web
{
	public class Url
	{
		string _q = "";
		NameValueCollection _c = null;
		bool _isCurrent = true;

		protected Url(string pathandquery, NameValueCollection query)
		{
			_q = pathandquery;
			_c = query;
		}

		public static Url Current
		{
			get
			{
				return new Url(HttpContext.Current == null ? "" : HttpContext.Current.Request.Url.PathAndQuery, 
								HttpContext.Current == null ? new NameValueCollection() : HttpContext.Current.Request.QueryString);
			}
		}

		public string Mode { get { return GetString("mode"); }}
		public string Action { get { return GetString("action"); }}
		public int? ID { get { return GetInt("oid"); }}

		public Url RemoveParameter(params string[] parametername)
		{
			foreach (string s in parametername)
			{
				if (_isCurrent && AppWeb.NodeData != null && AppWeb.NodeData.FURL.IndexOf("{" + s + "}") != -1)
				{
					// потом учесть вариант, когда несколько параметров имеют одинаковые значения
					string value = Convert.ToString(AppWeb.RouteDataValues[s]);
					if (_q.EndsWith("/" + value)) 
						_q = _q.Replace("/" + value, "");
					else
					{
						_q = _q.Replace("/" + value + "/", "/");
						_q = _q.Replace("/" + value + "?", "?");
					}
				}
				else
				{
					if (_q.IndexOf('?') > 0)
					{
						_q = Regex.Replace(_q, "[?]" + s + "=(?<1>[^&]*)", "?", RegexOptions.IgnoreCase);
						_q = Regex.Replace(_q, "[&]" + s + "=(?<1>[^&]*)", "", RegexOptions.IgnoreCase);
					}
				}
			}

			if (String.IsNullOrEmpty(_q)) _q = "/";
			return this;
		}

		public Url SetParameter(HtmlParms parms)
		{
			foreach (var parm in parms)
				RemoveParameter(parm.Key);
			foreach (var parm in parms)
				if (!parm.Value.IsEmpty()) _q = _q.AddQueryParameter(parm.Key, parm.Value);
			return this;
		}

		public Url SetParameter(string parm, string value)
		{
			RemoveParameter(parm);
			if (!value.IsEmpty()) _q = _q.AddQueryParameter(parm, value);
			return this;
		}

		public Url ReturnUrl
		{
			get
			{
				string r = _c["returnurl"];
				if (r.IsEmpty())
					return null;
				Uri u = new Uri(HttpContext.Current.Request.Url, r);
				string hash = u.ToString().IndexOf('#') > 0 ? u.ToString().Substring(u.ToString().IndexOf('#')) : "";
				return new Url(u.PathAndQuery + hash, HttpUtility.ParseQueryString(u.Query));
			}
			set
			{
				SetParameter("returnurl", HttpUtility.UrlEncode(value._q));
			}
		}

		public int GetInt(string parametername, int defaultValue)
		{
			if (_isCurrent && AppWeb.NodeData != null && AppWeb.NodeData.FURL.IndexOf("{" + parametername + "}") != -1)
				return Convert.ToString(AppWeb.RouteDataValues[parametername]).ToInt32(defaultValue);
			else
				return _c[parametername].ToInt32(defaultValue);
		}

		public int? GetInt(string parametername)
		{
			if (_isCurrent && AppWeb.NodeData != null && AppWeb.NodeData.FURL.IndexOf("{" + parametername + "}") != -1)
				return Convert.ToInt32(AppWeb.RouteDataValues[parametername]);
			else
				return _c[parametername].ToInt32();
		}

		public Guid GetGuid(string parametername)
		{
			if (_isCurrent && AppWeb.NodeData != null && AppWeb.NodeData.FURL.IndexOf("{" + parametername + "}") != -1)
			{
				object o = AppWeb.RouteDataValues[parametername];
				if (o == null)
					return Guid.Empty;
				else
					return o.ToString().ToGuid();
			}
			else
				return _c[parametername].ToGuid();
		}

		public string GetString(string parametername)
		{
			if (_isCurrent && AppWeb.NodeData != null && AppWeb.NodeData.FURL.IndexOf("{" + parametername + "}") != -1)
				return Convert.ToString(AppWeb.RouteDataValues[parametername]);
			else
				return _q.GetQueryParameter(parametername);
		}

		public static implicit operator string(Url m)
		{
			if (m == null)
				return null;
			return m.ToString();
		}

		public void Go()
		{
			HttpContext.Current.Response.Redirect(_q);
		}

		public static string Create(string mode, string action = "", HtmlParms parms = null, string site = null)
		{
			if (site == null && AppWeb.NodeData != null)
				site = AppWeb.NodeData.Site;
			string oid = parms != null && parms.ContainsKey("oid") ? ("/" + parms["oid"]) : "";
			if (parms != null && parms.ContainsKey("oid"))
				parms.Remove("oid");
			string p = parms == null ? "" : parms.ToString();

			if (AppWeb.IsRouting)
				return String.Format("/{0}{1}{2}{3}{4}",
					site.IsEmpty() ? "" : (site + "/"),
					mode,
					action.IsEmpty() ? "" : ("/" + action),
					oid,
					p.IsEmpty() ? "" : ("?" + p)
				);
			else
				return String.Format("/{0}?mode={1}&action={2}{3}{4}",
					site, mode, action, oid.IsEmpty() ? "" : ("&oid=" + oid), p.IsEmpty() ? "" : ("&" + p)
				);
			
		}

		public static string Create(HtmlParms parms = null, string site = null)
		{
			if (AppWeb.NodeData != null)
				return Create(AppWeb.NodeData.FURL, parms, site);
			else
				return "";
		}

		public static string Create(string route, HtmlParms parms = null, string site = null)
		{
			if (site == null && AppWeb.NodeData != null)
				site = AppWeb.NodeData.Site;
			string s = "/" + route;
			if (!site.IsEmpty())
				s = "/" + site + s;
			HtmlParms p = new HtmlParms();
			foreach (var parm in parms)
			{
				if (route.IndexOf("{" + parm.Key + "}") != -1)
					s = s.Replace("{" + parm.Key + "}", parm.Value);
				else
					p.Add(parm.Key, parm.Value);
			}
			string ps = p.ToString();
			return s + (ps.IsEmpty() ? "" : ("?" + ps));
		}

		public static Url CreateUrl(string route, HtmlParms parms = null, string site = null)
		{
			string s = Url.Create(route, parms, site);
			return new Url(s, HttpUtility.ParseQueryString(s)); 
		}

		public override string ToString()
		{
			return _q;
		}

		public static int MaxReturnUrlLength = 1800;

		public static string CreateReturnUrl()
		{
			return CreateReturnUrl(Url.Current.ToString());
			//return HttpUtility.UrlEncode(Url.Current);
		}

		public static string CreateReturnUrl(string url)
		{
			var returnurl = HttpUtility.UrlEncode(url);
			if (returnurl.Length > MaxReturnUrlLength)
			{
				Stack<string> urlStack = new Stack<string>();

				while (url.GetQueryParameter("returnurl") != "")
				{
					urlStack.Push(url.RemoveQueryParameter("returnurl"));
					url = HttpUtility.UrlDecode(url.GetQueryParameter("returnurl"));
				}
				url = urlStack.Pop();
				while (urlStack.Count > 0)
					url = urlStack.Pop().AddQueryParameter("returnurl", HttpUtility.UrlEncode(url));
				returnurl = HttpUtility.UrlEncode(url);
			}
			return returnurl;
		}
	}

	public class HtmlParms : Dictionary<string, string>
	{
		public HtmlParms() : base() { }
		public HtmlParms(IDictionary<string, string> dictionary) : base(dictionary) { }

		public override string ToString()
		{
			return this.Select(o => o.Value.IsEmpty() ? "" : (o.Key + "=" + o.Value)).Join("&");
		}

		public HtmlParms(string key, string value) : base() 
		{
			Add(key, value);
		}
	}

	public class NodeData
	{
		public Guid NodeGUID { get; set; }
		public string FURL { get; set; }
		public string MasterPage { get; set; }
		public string Parameters { get; set; }
		public string Description { get; set; }
		public string Keywords { get; set; }
		public string PageTitle { get; set; }
		public string Title { get; set; }
		public string Site { get; set; }
	}
	
}