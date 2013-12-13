using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Nephrite.Web.SPM;
using Nephrite.Web.Controls;
using Nephrite.Web.Controllers;

namespace Nephrite.Web
{
	public abstract class MasterControl : ViewControl
	{
		public virtual List<ContentPlaceHolderInfo> ContentPlaceHolders()
		{
			return new List<ContentPlaceHolderInfo>();
		}
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);		

			List<ContentPlaceHolderInfo> contents = ContentPlaceHolders();
			if (contents.Count == 0) return;

			NodeData node = AppWeb.NodeData;
			if (node == null) return;

			AppWeb.Title = node.PageTitle;
			AppWeb.Description = node.Description;
			AppWeb.Keywords = node.Keywords;
			AppWeb.MasterControl = this;

			// проверим права на адрес (в будущем нужно переделать)
			if (!ActionSPMContext.Current.Check(node.FURL + ".View", 1, true))
			{
				LiteralControl l = new LiteralControl("У вас нет прав просматривать данную страницу");
				contents[0].Control.Controls.Add(l);
				return;
			}

			string[] parms = node.Parameters != null ? node.Parameters.Split(new char[] { ';' }) : new string[] { "" };
			int i = 0;
			foreach (ContentPlaceHolderInfo content in contents)
			{
				if (i >= parms.Length) continue; 
				string[] partParms = parms[i].Split(new char[] { ',' });
				if (partParms[0] == "M")
				{
					BaseController.Run(content.Control, partParms[1], partParms[2], true, true);
				}
				else if (partParms[0] == "V")
				{
					System.Type controllerType = ControllerFactory.GetControllerType(partParms[1]);
					MMObjectController controller = (MMObjectController)Activator.CreateInstance(controllerType);
					controller.WebPart = content.Control;
					controller.RenderMMView(partParms[2]);
				}
				else if (partParms[0] == "P")
				{
					PackageFormViewController controller = new PackageFormViewController(partParms[1]);
					controller.WebPart = content.Control;
					controller.RenderMMView(partParms[2]);
				}
				else if (partParms[0] == "XA")
				{
					if (AppWeb.RouteDataValues.ContainsKey("mode"))
					{
						if (AppWeb.RouteDataValues.ContainsKey("action"))
						{
							BaseController.Run(content.Control,
							AppWeb.RouteDataValues["mode"].ToString(),
							AppWeb.RouteDataValues["action"].ToString(), true, true);
						}
						else
						{
							if (AppWeb.RouteDataValues.ContainsKey("oid"))
							{
								BaseController.Run(content.Control,
								AppWeb.RouteDataValues["mode"].ToString(),
								"View", true, true);
							}
							else
							{
								BaseController.Run(content.Control,
								AppWeb.RouteDataValues["mode"].ToString(),
								"ViewList", true, true);
							}
						}
					}
				}
				else if (partParms[0] == "XW")
				{
					if (AppWeb.RouteDataValues.ContainsKey("mode"))
					{
						if (AppWeb.RouteDataValues.ContainsKey("action"))
						{
							MMObjectController.Run(content.Control,
							AppWeb.RouteDataValues["mode"].ToString(),
							AppWeb.RouteDataValues["action"].ToString(), true, true);
						}
						else
						{
							if (AppWeb.RouteDataValues.ContainsKey("oid"))
							{
								MMObjectController.Run(content.Control,
								AppWeb.RouteDataValues["mode"].ToString(),
								"SiteView", true, true);
							}
							else
							{
								MMObjectController.Run(content.Control,
								AppWeb.RouteDataValues["mode"].ToString(),
								"SiteList", true, true);
							}
						}
					}
				}
				/*if (content.Control.Controls.Count == 1 && content.Control.Controls[0] is ViewControl &&
					((ViewControl)content.Control.Controls[0]).RenderMargin)
				{
					content.Control.Controls.AddAt(0, new LiteralControl("<div style='padding:8px'>"));
					content.Control.Controls.Add(new LiteralControl("</div>"));
				}*/
					
				i++;
			}
		}

		public override Toolbar GetToolbar()
		{
			return null;
		}

		public override ButtonBar GetButtonBar()
		{
			return null;
		}
	}

	public class ContentPlaceHolderInfo
	{
		public string Title { get; set; }
		public Control Control { get; set; }
	}
}