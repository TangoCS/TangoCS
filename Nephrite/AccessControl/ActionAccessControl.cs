using System;
using System.Collections.Generic;
using System.Linq;
using Nephrite.Http;
using Nephrite.Identity;

namespace Nephrite.AccessControl
{
	public class ActionAccessControl<TKey>
	{
		static Func<IHttpContext> HttpContext = null;
		public static void Init(Func<IHttpContext> httpContext)
		{
			HttpContext = httpContext;
		}

		public static CacheableAccessControl<TKey> Instance
		{
			get
			{
				return HttpContext().Items["ActionAccessControl"] as CacheableAccessControl<TKey>;
			}
		}
	}

	public class ActionAccessControl : ActionAccessControl<int> { }

	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class SecurableObjectKeyAttribute : Attribute
	{
		readonly string _name;


		public SecurableObjectKeyAttribute(string name)
		{
			_name = name;
		}

		public string Name
		{
			get { return _name; }
		}
	}
}