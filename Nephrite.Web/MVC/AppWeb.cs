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
using System.Configuration;


namespace Nephrite.Web
{
	public class AppWeb
	{
		//public static Page Page
		//{
		//	get { return HttpContext.Current.Items["page"] as Page; }
		//	set { HttpContext.Current.Items["page"] = value; }
		//}
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

		//public static IDictionary<string, object> RouteDataTokens
		//{
		//	get { return HttpContext.Current.Items["RouteDataTokens"] as IDictionary<string, object>; }
		//	set { HttpContext.Current.Items["RouteDataTokens"] = value; }
		//}
		//public static IDictionary<string, object> RouteDataValues
		//{
		//	get { return HttpContext.Current.Items["RouteDataValues"] as IDictionary<string, object>; }
		//	set { HttpContext.Current.Items["RouteDataValues"] = value; }
		//}
		public static NodeData NodeData
		{
			get { return HttpContext.Current.Items["NodeData"] as NodeData; }
			set { HttpContext.Current.Items["NodeData"] = value; }
		}
		//public static NodeData HomeNodeData { get; set; }
		public static bool IsRouting
		{
			get { return Convert.ToBoolean(HttpContext.Current.Items["IsRouting"]); }
			set { HttpContext.Current.Items["IsRouting"] = value; }
		}

		public static string AppNamespace
		{
			get
			{
				return ConfigurationManager.AppSettings["ModelNamespace"];
			}
		}
	}

	public class NodeData
	{
		//public Guid NodeGUID { get; set; }
		public string FURL { get; set; }
		public string MasterPage { get; set; }
		//public string Parameters { get; set; }
		public string Description { get; set; }
		public string Keywords { get; set; }
		public string PageTitle { get; set; }
		public string Title { get; set; }
		public string Site { get; set; }

		public Dictionary<string, Action> ContentPlaceHolderRenderers { get; set; }

		public NodeData()
		{
			ContentPlaceHolderRenderers = new Dictionary<string, Action>();
		}
	}

	
}
