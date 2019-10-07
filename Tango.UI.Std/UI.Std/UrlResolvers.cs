using System.Collections.Generic;
using Tango.AccessControl;
using Tango.Html;

namespace Tango.UI.Std
{
	public class SecurableUrlResolver : RouteUrlResolver
	{
		IAccessControl _accessControl;
		string _securableObjectKey;
		object _predicateContext;
		bool _checkPredicateIfContextIsEmpty = false;

		public SecurableUrlResolver(IAccessControl accessControl, string routeTemplate) : base(routeTemplate)
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

		public override UrlResolverResult Resolve(IReadOnlyDictionary<string, string> parameters, DynamicDictionary globalParameters)
		{
			bool access = true;
			if (!_securableObjectKey.IsEmpty())
				if (_predicateContext != null || _checkPredicateIfContextIsEmpty)
					access = _accessControl.CheckWithPredicate(_securableObjectKey, _predicateContext).Value;
				else
					access = _accessControl.Check(_securableObjectKey);

			if (access)
				return base.Resolve(parameters, globalParameters);
			else
				return new UrlResolverResult { Resolved = false };
		}
	}
}
