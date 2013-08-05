using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using Nephrite.Meta;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;
using System.Collections;
using Nephrite.Web.Controls;


namespace Nephrite.Web
{
	public class AppWeb
	{
		public static Page Page
		{
			get { return HttpContext.Current.Items["page"] as Page; }
			set { HttpContext.Current.Items["page"] = value; }
		}
		public static IMasterControl MasterControl
		{
			get { return HttpContext.Current.Items["mastercontrol"] as IMasterControl; }
			set { HttpContext.Current.Items["mastercontrol"] = value; }
		}

		public static string Title 
		{ 
			get { return HttpContext.Current.Items["title"] as string; }
			set { HttpContext.Current.Items["title"] = value; }
		}
		public static string Keywords
		{
			get { return HttpContext.Current.Items["keywords"] as string; }
			set { HttpContext.Current.Items["keywords"] = value; }
		}
		public static string Description
		{
			get { return HttpContext.Current.Items["description"] as string; }
			set { HttpContext.Current.Items["description"] = value; }
		}

		public static IDictionary<string, object> RouteDataTokens
		{
			get { return HttpContext.Current.Items["RouteDataTokens"] as IDictionary<string, object>; }
			set { HttpContext.Current.Items["RouteDataTokens"] = value; }
		}
		public static IDictionary<string, object> RouteDataValues
		{
			get { return HttpContext.Current.Items["RouteDataValues"] as IDictionary<string, object>; }
			set { HttpContext.Current.Items["RouteDataValues"] = value; }
		}
		public static NodeData NodeData
		{
			get { return HttpContext.Current.Items["NodeData"] as NodeData; }
			set { HttpContext.Current.Items["NodeData"] = value; }
		}
		public static NodeData HomeNodeData { get; set; }
		public static bool IsRouting
		{
			get { return Convert.ToBoolean(HttpContext.Current.Items["IsRouting"]); }
			set { HttpContext.Current.Items["IsRouting"] = value; }
		}
	}

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

	

	public class DataContextLogWriter : StringWriter
	{
		public override void Write(char[] buffer, int index, int count)
		{
			base.Write(buffer, index, count);
			if ((new string(buffer)).StartsWith("-- Context:"))
			{
				char[] buff = ("-- Execute start: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + System.Environment.NewLine).ToCharArray();
				base.Write(buff, 0, buff.Length);
			}
		}
	}
}
