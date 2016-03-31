using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Routing;
using System.Web.UI;
using Nephrite.Web.View;
using Nephrite.MVC;

namespace Nephrite.Web.FormsEngine
{
	public class WebFormsRenderer
	{
		public static string DefaultViewName { get; set; }

		public bool IsStringResult
		{
			get
			{
				return false;
			}
		}

		Control _container;

		public void SetContainer(Control container)
		{
			_container = container;
		}

		public void RenderMessage(string message)
		{
			RenderMessage("Внимание!", message);
		}

		public void RenderMessage(string title, string message)
		{
			string path = Settings.BaseControlsPath + "Message/show.ascx";
			ViewControl control = (ViewControl)_container.Page.LoadControl(path);

			control.SetViewData(new MessageViewData { Title = title, Text = message });

			_container.Controls.Add(control);
		}

		public void RenderView(string folder, string viewName, object viewData)
		{
			string path = Settings.ControlsPath + "/" + folder + "/" + viewName + ".ascx";
			//HttpContext.Current.Items["FormView"] = folder + "." + viewName;
			//HttpContext.Current.Items["ObjectType"] = folder;
			//HttpContext.Current.Items["helpdata"] = "mode=c_help&view=view&form=" + folder + "." + viewName;

			ViewControl ctl = null;
			try
			{

				if (!File.Exists(String.Format("{1}{0}{2}{0}{3}{0}{4}.ascx", Path.DirectorySeparatorChar, 
					AppDomain.CurrentDomain.BaseDirectory, Settings.ControlsPath, 
					folder, viewName)))
				{
					if (viewName == "delete")
						ctl = _container.Page.LoadControl(typeof(StandardDelete), null) as ViewControl;
					else if (viewName == "undelete")
						ctl = _container.Page.LoadControl(typeof(StandardUndelete), null) as ViewControl;
					else 
					{
						RenderMessage("Представление " + VirtualPathUtility.ToAppRelative(path) + " не найдено");
						return;
					}
				}
				else
				{
					ctl = _container.Page.LoadControl(path) as ViewControl;
				}
				if (viewData != null)
					ctl.SetViewData(viewData);
				var t = ctl.GetType();
				ctl.RenderMargin = !(viewData is IQueryable);
				_container.Controls.Add(ctl);
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
				_container.Controls.Add(lc);
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
							_container.Controls.Add(lc2);
						}
					}
				}

			}
		}

		public void RenderHtml(string title, string html)
		{
			HttpContext.Current.Items["title"] = title;
			LiteralControl l = new LiteralControl(html);
			_container.Controls.Add(l);
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
			rc.MapPageRoute(Guid.NewGuid().ToString(), routeUrl, masterPage, builder);
		}

		public static void MapPageRoute(this RouteCollection rc, string routeName, string routeUrl, string masterPage, Action<RouteBuilder> builder)
		{
			RouteBuilder rb = new RouteBuilder();
			if (builder != null) builder(rb);

			rc.MapPageRoute(routeName, routeUrl, "~/default.aspx", false,
				new RouteValueDictionary(),
				rb.Constraints,
				new RouteValueDictionary { { "node", new NodeData { MasterPage = masterPage, ContentPlaceHolderRenderers = rb.Renderers } } });
		}
	}

	public class RouteBuilder
	{
		public Dictionary<string, Action<Control>> Renderers { get; set; }
		public RouteValueDictionary Constraints { get; set; }

		public RouteBuilder()
		{
			Renderers = new Dictionary<string, Action<Control>>();
			Constraints = new RouteValueDictionary();
		}

		public void SetRenderer(string containerName, Action<Control> renderer)
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