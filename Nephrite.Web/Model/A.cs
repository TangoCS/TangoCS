using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Nephrite.Meta;
using Nephrite.Meta.Database;

namespace Nephrite.Web
{
	public class A
	{
		public static IDataContext Model
		{
			get
			{
				return A.Items["Base.Model"] as IDataContext;
			}
			set
			{
				A.Items["Base.Model"] = value;
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

		public static IDictionary Items
		{
			get
			{
				if (HttpContext.Current != null) return HttpContext.Current.Items;

				Hashtable ht = (Hashtable)AppDomain.CurrentDomain.GetData("ContextItems");
				if (ht == null)
				{
					ht = new Hashtable();
					AppDomain.CurrentDomain.SetData("ContextItems", ht);
				}
				return ht;

			}
		}
	}
}