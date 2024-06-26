﻿using System.Collections.Generic;
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

		public override UrlResolverResult Resolve(string template, IReadOnlyDictionary<string, string> parameters)
		{
			bool access = true;
			if (!_securableObjectKey.IsEmpty())
				if (_predicateContext != null || _checkPredicateIfContextIsEmpty)
					access = _accessControl.CheckWithPredicate(_securableObjectKey, _predicateContext).Value;
				else
					access = _accessControl.Check(_securableObjectKey);

			if (access)
				return base.Resolve(template, parameters);
			else
				return new UrlResolverResult { Resolved = false };
		}
	}
}
