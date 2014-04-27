using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Nephrite.Meta;
using Nephrite.Meta.Database;
using Nephrite.Web.Hibernate;

namespace Nephrite.Web
{
	public class A
	{
		[ThreadStatic]
		static IDataContext model = null;
		public static IDataContext Model
		{
			get
			{
				if (HttpContext.Current != null)
				{
					return HttpContext.Current.Items["Base.Model"] as IDataContext;
				}
				else
				{
					return model;
				}
			}
			set
			{
				if (HttpContext.Current != null)
				{
					HttpContext.Current.Items["Base.Model"] = value;
				}
				else
				{
					model = value;
				}
			}
		}

		[ThreadStatic]
		static IDBScript script = null;

		public static IDBScript DBScript
		{
			get { return script; }
			set { script = value; }
		}

		public static MetaSolution Meta { get; set; }

		static XElement _metaXml = null;
		public static XElement MetaXml
		{
			get
			{
				if (_metaXml == null)
					return DBScript.GetMeta();
				return _metaXml;
			}
		}

		static MetaSolution _meta = null;
		public static MetaSolution DynamicMeta
		{
			get
			{
				if (_meta == null) _meta = MetaSolution.Load();
				return _meta;
			}
		}
	}
}