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
	public class RouteUrlResolver : AbstractUrlResolver
	{
		protected string _routeName;

		public virtual RouteUrlResolver UseRoute(string routeName)
		{
			_routeName = routeName;
			return this;
		}

		public override StringBuilder Resolve(IDictionary<string, object> parameters, bool isHashPart = false)
		{
			return GeneratePathAndQueryFromRoute(_routeName, parameters, isHashPart) ?? new StringBuilder();
		}

		public StringBuilder GeneratePathAndQueryFromRoute(string routeName, IDictionary<string, object> values, bool isHashPart)
		{
			if (routeName != null)
			{
				var route = RouteTable.Routes[routeName];
				if (route == null) return null;
				return CreateUrl((route as Route).Url, values, isHashPart);
			}
			else
			{
				var d = new RouteValueDictionary(values);
				var r = HttpContext.Current.Request.RequestContext;
				VirtualPathData virtualPathData = RouteTable.Routes.GetVirtualPath(r, d);
				if (virtualPathData == null) return null;
				return CreateUrl((virtualPathData.Route as Route).Url, values, isHashPart);
			}
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

		public override StringBuilder Resolve(IDictionary<string, object> parameters, bool isHashPart = false)
		{
			bool access = true;
			if (!_securableObjectKey.IsEmpty())
				if (_predicateContext != null || _checkPredicateIfContextIsEmpty)
					access = _accessControl.CheckWithPredicate(_securableObjectKey, _predicateContext).Value;
				else
					access = _accessControl.Check(_securableObjectKey);

			if (access)
				return base.Resolve(parameters, isHashPart);
			else
				return new StringBuilder();
		}
	}
}
