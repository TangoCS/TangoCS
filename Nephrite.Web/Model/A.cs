using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Nephrite.Meta;

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

		public static MetaSolution Meta { get; set; }

		static XElement _metaXml = null;
		public static XElement MetaXml
		{
			get
			{
				if (_metaXml == null) _metaXml = Model.ExecuteQuery<XElement>("EXEC [dbo].[usp_model]").FirstOrDefault();
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