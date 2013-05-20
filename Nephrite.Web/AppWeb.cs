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
using Nephrite.Web.Layout;
using Nephrite.Web.Model;
using System.Globalization;
using System.Reflection;
using Nephrite.Web.Controls;
using System.Xml.Linq;
using NHibernate.Cfg.Loquacious;
using NHibernate.Cfg;
using NHibernate.Dialect;
using System.Collections;
using Nephrite.Web.Multilanguage;

namespace Nephrite.Web
{
	public class AppWeb
	{
		//[ThreadStatic]
		//static HCoreDataContext context = null;

		/*public static HCoreDataContext DataContext
		{
			get
			{
				if (HttpContext.Current != null)
				{
					if (HttpContext.Current.Items["HCoreDataContext"] == null)
					{
						HCoreDataContext dc = new HCoreDataContext(AppWeb.DBConfig);
						//dc.CommandTimeout = 300;
						HttpContext.Current.Items["HCoreDataContext"] = dc;
						//dc.Log = new DataContextLogWriter();
					}
					return (HCoreDataContext)HttpContext.Current.Items["HCoreDataContext"];
				}
				else
				{
					if (context == null)
					{
						context = new HCoreDataContext(AppWeb.DBConfig);
						//context.CommandTimeout = 300;
						//context.Log = new DataContextLogWriter();
					}
					return context;
				}
			}
		}
		 */

		static ISystemLayout _layout = null;

		public static ISystemLayout Layout
		{
			get
			{
				if (_layout == null)
				{
					SystemLayout sl = new SystemLayout();
					sl.Main = new MainWSS2007_FixHeight();
					sl.List = new ListWSS2007();
					sl.ListRowDrag = new ListRowDragWSS2007();
					sl.Message = new MessageWSS2007();
					sl.Paging = new PagingWSS2007();
					sl.Form = new FormWSS2007();
					sl.Toolbar = new ToolbarWSS2007();
					sl.ButtonBar = new ButtonsBarWSS2007();
					sl.Modal = new WinModal();
					sl.ToolbarDropdownCompact = new PopupMenuWSS2007Compact();
					sl.ToolbarDropdownLarge = new PopupMenuWSS2007Large();
					sl.ButtonDropdown = new PopupMenuWSS2007Compact();
					sl.SimpleTags = new SimpleTags();
					sl.Labels = new LabelsBootstrap();
					sl.AutoMargin = new AutoMarginWSS2007();
					sl.Button = new ButtonWSS2007();
					sl.ToolbarButton = new ToolbarButtonWSS2007();
					
					_layout = sl;
				}
				return _layout;
			}
		}
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

		static List<IC_Language> _langs;
		public static List<IC_Language> Languages
		{
			get
			{
				if (_langs == null)
					_langs = ((IDC_Multilanguage)Base.Model).C_Language.OrderByDescending(o => o.IsDefault).ToList();
				return _langs;
			}
		}

		public static IC_Language CurrentLanguage
		{
			get
			{
				string lang = Query.GetString("lang");
				if (HttpContext.Current.Request.Cookies["lcid"] != null)
					lang = HttpContext.Current.Request.Cookies["lcid"].Value == "1033" ? "en" : "ru";
				if (HttpContext.Current.Items["Lang"] != null)
					lang = (string)HttpContext.Current.Items["Lang"];
				var l = Languages.SingleOrDefault(o => o.LanguageCode == lang);
				if (l == null)
					l = Languages.Single(o => o.IsDefault);
				return l;
			}
		}

		public static CultureInfo CurrentCulture
		{
			get
			{
				switch (CurrentLanguage.LanguageCode.ToLower())
				{
					case "en":
						return CultureInfo.GetCultureInfo("en-US");
					case "ru":
						return CultureInfo.GetCultureInfo("ru-RU");
					default:
						return CultureInfo.InvariantCulture;
				}
			}
		}

		public static Action<IDbIntegrationConfigurationProperties> DBConfig
		{
			get
			{
				return c =>
				{
					c.Dialect<MsSql2008Dialect>();
					c.ConnectionString = ConnectionManager.ConnectionString;
					c.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
					c.IsolationLevel = System.Data.IsolationLevel.ReadUncommitted;
					//c.LogFormattedSql = true;
					c.SchemaAction = SchemaAutoAction.Validate;
				};
			}
		}
		public static Assembly Assembly { get; set; }
	}

	public class Base
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
