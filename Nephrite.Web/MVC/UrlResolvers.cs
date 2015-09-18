using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using Nephrite.AccessControl;
using Nephrite.Html;

namespace Nephrite.Web
{
	public class RouteUrlResolver : IUrlResolver
	{
		protected string _routeName;

		public RouteUrlResolver UseRoute(string routeName)
		{
			_routeName = routeName;
			return this;
		}

		public virtual string Resolve(IDictionary<string, object> parameters)
		{
			return GeneratePathAndQueryFromRoute(_routeName, parameters) ?? "";
		}

		public string GeneratePathAndQueryFromRoute(string routeName, IDictionary<string, object> values)
		{
			if (routeName != null)
			{
				var route = RouteTable.Routes[routeName];
				if (route == null) return null;
				return CreateUrl((route as Route).Url, values);
			}
			else
			{
				var d = new RouteValueDictionary(values);
				var r = HttpContext.Current.Request.RequestContext;
				VirtualPathData virtualPathData = RouteTable.Routes.GetVirtualPath(r, d);
				if (virtualPathData == null) return null;
				return CreateUrl((virtualPathData.Route as Route).Url, values);
			}
		}

		public string CreateUrl(string url, IDictionary<string, object> parms)
		{
			string[] parts = url.Split(new char[] { '/' });
			StringBuilder res = new StringBuilder();

			foreach (string part in parts)
			{
				if (part.StartsWith("{"))
				{
					string key = part.Substring(1, part.Length - 2);
					if (parms.ContainsKey(key))
					{
						res.Append("/").Append(parms[key]);
						parms.Remove(key);
					}
				}
				else
					res.Append("/").Append(part);
			}

			if (parms.Count > 0) res.Append("?");
			bool first = true;
			foreach (var parm in parms)
			{
				if (!first) res.Append("&");
				res.Append(parm.Key);
				if (parm.Value != null) res.Append("=").Append(parm.Value);
				first = false;
			}

			return res.ToString();
		}
	}

	public class SecurableUrlResolver : RouteUrlResolver
	{
		IAccessControl _accessControl;
		string _securableObjectKey;
		object _predicateContext;
		bool _checkPredicateIfContextIsEmpty = false;

		public SecurableUrlResolver(IAccessControl accessControl)
		{
			_accessControl = accessControl;
        }

		public SecurableUrlResolver WithKey(string securableObjectKey)
		{
			_securableObjectKey = securableObjectKey;
			return this;
		}

		public SecurableUrlResolver WithPredicate(object predicateContext, bool checkPredicateIfContextIsEmpty = false)
		{
			_predicateContext = predicateContext;
			_checkPredicateIfContextIsEmpty = checkPredicateIfContextIsEmpty;
			return this;
		}

		public override string Resolve(IDictionary<string, object> parameters)
		{
			bool access = true;
			if (!_securableObjectKey.IsEmpty())
				if (_predicateContext != null || _checkPredicateIfContextIsEmpty)
					access = _accessControl.CheckWithPredicate(_securableObjectKey, _predicateContext).Value;
				else
					access = _accessControl.Check(_securableObjectKey);

			if (access)
				return base.Resolve(parameters);
			else
				return "";
		}
	}
}
