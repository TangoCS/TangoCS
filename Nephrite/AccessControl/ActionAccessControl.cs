using System;
using System.Collections.Generic;
using System.Linq;
using Nephrite.Identity;

namespace Nephrite.AccessControl
{
	public class ActionAccessControl
	{
		static CacheableAccessControl<int> _instanceHolder;
		static object LockObject = new object();

		public static void Init(
			Func<IAppContext> appContext,
			Func<ICacheableAccessControlDataContext> dataContext,
			Func<IIdentityManager<int>> identityManager,
			CacheableAccessControlOptions options = null
			)
		{
			if (_instanceHolder == null)
			{
				lock (LockObject)
				{
					if (_instanceHolder == null)
					{
						if (options == null) options = new CacheableAccessControlOptions { Enabled = () => true };
						if (options.ClassName.IsEmpty()) options.ClassName = "Action";
						_instanceHolder = new CacheableAccessControl<int>(appContext, dataContext, identityManager, options);
						return;
					}
				}
			}

			throw new ApplicationException("ActionAccessControl.Init() method should be called only once.");
		}

		public static CacheableAccessControl<int> Instance
		{
			get
			{
				if (_instanceHolder == null)
				{
					throw new ApplicationException("ActionAccessControl instance hasn't been initialized.");
				}

				return _instanceHolder;
			}
		}
	}

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