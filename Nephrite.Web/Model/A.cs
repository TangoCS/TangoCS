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

				Hashtable ht = AppDomain.CurrentDomain.GetData("ContextItems") as Hashtable;
				if (ht == null)
				{
					ht = new Hashtable();
					AppDomain.CurrentDomain.SetData("ContextItems", ht);
				}
				return ht;

			}
		}

		static DBType? _dbType = null;
		public static DBType DBType
		{
			get
			{
				if (_dbType == null) _dbType = (DBType?)Enum.Parse(typeof(DBType), System.Configuration.ConfigurationManager.AppSettings["DBType"].ToUpper());
				if (_dbType == null) throw new Exception("DBType parameter not found in your app's config");
				return _dbType.Value;
			}
			set
			{
				_dbType = value;
			}
		}
	}

	public enum DBType
	{
		MSSQL, DB2, ORACLE, POSTGRESQL
	}
}