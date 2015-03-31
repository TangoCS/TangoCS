using System;
using System.Collections.Generic;
using System.Linq;

namespace Nephrite.Web
{
	public class Cache
	{
		public static void Refresh(string group)
		{
		}

		public static void RefreshAll()
		{

		}
	}

	[global::System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public sealed class CacheAttribute : Attribute
	{
		readonly string _group;


		public CacheAttribute(string group)
		{
			_group = group;
		}

		public string Group
		{
			get { return _group; }
		}
	}
}