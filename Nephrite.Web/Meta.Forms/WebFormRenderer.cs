using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Routing;
using System.Web.UI;
using Nephrite.Web;
using Nephrite.Web.SettingsManager;
using Nephrite.Web.SPM;

namespace Nephrite.Meta.Forms
{
	public static class WebFormRenderer
	{
		[DefaultSettingValue("ViewList")]
		public static string DefaultViewName { get; set; }

		public static void RenderMessage(string message)
		{
			Control container = HttpContext.Current.Items["ViewContainer"] as Control;
			string path = Settings.BaseControlsPath + "Message/show.ascx";
			ViewControl control = (ViewControl)container.Page.LoadControl(path);

			control.SetViewData(new MessageViewData { Title = "Внимание!", Text = message });

			container.Controls.Add(control);
		}

		public static void RenderMessage(string title, string message)
		{
			Control container = HttpContext.Current.Items["ViewContainer"] as Control;
			string path = Settings.BaseControlsPath + "Message/show.ascx";
			ViewControl control = (ViewControl)container.Page.LoadControl(path);

			control.SetViewData(new MessageViewData { Title = title, Text = message });

			container.Controls.Add(control);
		}

		public static void RenderView()
		{
			RenderView(Url.Current.GetString("mode"), Url.Current.GetString("action"), null);
		}

		public static void RenderView(object viewData)
		{
			RenderView(Url.Current.GetString("mode"), Url.Current.GetString("action"), viewData);
		}
		
		public static void RenderView(string viewName, object viewData)
		{
			RenderView(Url.Current.GetString("mode"), viewName, viewData);
		}

		public static void RenderView(string folder, string viewName, object viewData)
		{
			Control container = HttpContext.Current.Items["ViewContainer"] as Control;
			string path = Settings.ControlsPath + "/" + folder + "/" + viewName + ".ascx";
			HttpContext.Current.Items["FormView"] = folder + "." + viewName;
			HttpContext.Current.Items["ObjectType"] = folder;
			HttpContext.Current.Items["helpdata"] = "mode=c_help&view=view&form=" + folder + "." + viewName;

			Control ctl = null;
			try
			{
				ctl = container.Page.LoadControl(path);
				if (viewData != null)
					((ViewControl)ctl).SetViewData(viewData);
				var t = ctl.GetType();
				((ViewControl)ctl).RenderMargin = !(viewData is IQueryable); //t.BaseType.BaseType.GetGenericArguments().Length == 1 && t.BaseType.BaseType.GetGenericArguments()[0].GetInterfaces().Contains(typeof(IMMObject));
				container.Controls.Add(ctl);
			}
			catch (Exception e)
			{
				int line = 0;
				int col = 0;
				if (e is HttpCompileException)
				{
					var hce = (HttpCompileException)e;
					if (hce.Results.Errors.HasErrors)
					{
						line = hce.Results.Errors[0].Line;
						col = hce.Results.Errors[0].Column;
					}
				}
				string text1 = "";
				string text2 = "";
				if (ctl != null)
					text1 = "<div>Класс представления: " + ctl.GetType().FullName + ", " + folder + ", FormView=" + viewName + "</div>";

				while (e != null)
				{
					text2 += "<b>" + HttpUtility.HtmlEncode(e.Message) + "</b>\r\n" + e.StackTrace + "\r\n\r\n";
					e = e.InnerException;
				}
				LiteralControl lc = new LiteralControl(text1 + "<pre>" + text2 + "</pre>");
				container.Controls.Add(lc);
				if (line > 0)
				{
					text2 = "";

					using (Stream s = VirtualPathProvider.OpenFile(path))
					{
						using (StreamReader sr = new StreamReader(s))
						{
							string[] lines = sr.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.None);
							for (int i = 0; i < lines.Length; i++)
							{
								if (i + 1 == line)
									text2 += "<span style=\"color:Red; font-size:13px; font-weight:bold\">" + HttpUtility.HtmlEncode(lines[i]).Replace(" ", "&nbsp;").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;") + "</span>";
								else
									text2 += HttpUtility.HtmlEncode(lines[i]).Replace(" ", "&nbsp;").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
								text2 += "<br />";
							}
							LiteralControl lc2 = new LiteralControl("<br /><br />" + text2);
							container.Controls.Add(lc2);
						}
					}
				}

			}
		}

		public static void RenderOperation()
		{
			var rd = HttpContext.Current.Request.RequestContext.RouteData;
			if (!rd.Values.ContainsKey("mode"))
			{
				RenderMessage("Не найден параметр mode");
				return;
			}

			string mode = rd.Values["mode"].ToString();
			string action = rd.Values.ContainsKey("action") ? rd.Values["action"].ToString() : DefaultViewName;

			if (ConfigurationManager.AppSettings["DisableSPM"] == null && action.ToLower() != "logoff")
			{
				if (!CheckPermissions(mode + "." + action)) return;
			}

			MetaOperation op = null;
			if (mode.EndsWith("Pck"))
			{
				var pck = A.Meta.GetPackage(mode.Replace("Pck", ""));
				if (pck == null)
				{
					RenderMessage(String.Format(Nephrite.Web.Resources.Common.ControllerMethodNotFound, action, mode));
					return;
				}
				op = pck.GetOperation(action);
			}
			else
				op = A.Meta.GetOperation(mode, action);

			if (op == null)
			{
				RenderMessage(String.Format(Nephrite.Web.Resources.Common.ControllerMethodNotFound, action, mode));
				return;
			}

			op.Invoke();
		}

		public static bool CheckPermissions(string securableObjectID)
		{
			if (!ActionSPMContext.Current.Check(securableObjectID, 1))
			{
				if (AppSettings.Get("loginurl").IsEmpty())
				{
					RenderMessage("Недостаточно полномочий для доступа к информации");
					return false;
				}
				else
					HttpContext.Current.Response.Redirect(AppSettings.Get("loginurl").AddQueryParameter("returnurl", Query.CreateReturnUrl()));
			}
			return true;
		}
	}

	public class MessageViewData
	{
		public string Title { get; set; }
		public string Text { get; set; }
	}

	public static class RouteCollectionExtension
	{
		public static void MapPageRoute(this RouteCollection rc, string routeUrl, string masterPage, Action<RouteBuilder> builder)
		{
			RouteBuilder rb = new RouteBuilder();
			if (builder != null) builder(rb);

			rc.MapPageRoute(Guid.NewGuid().ToString(), routeUrl, "~/default.aspx", false, 
				new RouteValueDictionary(), 
				rb.Constraints, 
				new RouteValueDictionary {{"node", new NodeData { MasterPage = masterPage, FURL = routeUrl, ContentPlaceHolderRenderers = rb.Renderers }}});
		}
	}

	public class RouteBuilder
	{
		public Dictionary<string, Action> Renderers { get; set; }
		public RouteValueDictionary Constraints { get; set; }

		public RouteBuilder()
		{
			Renderers = new Dictionary<string,Action>();
			Constraints = new RouteValueDictionary();
		}

		public void SetRenderer(string containerName, Action renderer)
		{
			Renderers.Add(containerName, renderer);
		}

		public void GuidMatch(string parmName)
		{
			Constraints.Add(parmName, @"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$");
		}

		public void NumericMatch(string parmName)
		{
			Constraints.Add(parmName, @"\d+");
		}

		public void Match(string parmName, object constraint)
		{
			Constraints.Add(parmName, constraint);
		}

	}
}