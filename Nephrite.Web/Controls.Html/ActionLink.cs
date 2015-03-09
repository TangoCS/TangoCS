using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Identity;
using System.Configuration;
using System.Collections.Specialized;
using Nephrite.Meta;
using Nephrite.AccessControl;
using Nephrite.Html;
using Nephrite.Layout;
using Nephrite.Http;

namespace Nephrite.Web.Controls
{
	public class ActionLink : SimpleLink
	{
		public string ControllerName { get; set; }
		public string ActionName { get; set; }

		public static ActionLink To(MetaOperation operation)
		{
			return new ActionLink(operation);
		}

		public ActionLink(MetaOperation operation)
		{
			if (operation != null)
			{
				Title = operation.Caption;
				Image = operation.Image;
				SecurableObjectKey = operation.ID;
				ControllerName = operation.Parent.Name;
				ActionName = operation.Name;
			}
		}

		public ActionLink(string controllerName, string actionName, string title, string image = null)
		{
			Title = title;
			Image = image;
			SecurableObjectKey = controllerName + "." + actionName;
			ControllerName = controllerName;
			ActionName = actionName;
		}

		public ActionLink With(HtmlParms parametersValues, bool addReturnUrl = true)
		{
			parametersValues.Add("bgroup", UrlHelper.Current().GetString("bgroup"));
			parametersValues.Add("mode", ControllerName);
			parametersValues.Add("action", ActionName);
			if (AppWeb.IsRouting)
				Href = QueryHelpers.CreateUrl(AppWeb.NodeData.FURL, parametersValues);
			else
			{
				HtmlParms pRes = new HtmlParms();
				pRes.Add("mode", ControllerName);
				pRes.Add("action", ActionName);

				NameValueCollection qs = HttpContext.Current.Request.QueryString;
				HtmlParms pSave = new HtmlParms();
				bool m = false;

				for (int i = 0; i < qs.Count; i++)
				{
					string parm = qs.GetKey(i).ToLower();
					switch (parm)
					{
						case "mode":
							m = qs[i].ToLower() == ControllerName.ToLower();
							break;
						case "action":
						case "returnurl":
							break;
						case "lang":
						case "bgroup":
							if (!pRes.ContainsKey(parm))
								pRes.Add(parm, qs[i]);
							break;
						default:
							if (!parametersValues.ContainsKey(parm))
								pSave.Add(parm, qs[i]);
							break;
					}
				}

				if (m)
				{
					foreach (var p in pSave)
					{
						if (!pRes.ContainsKey(p.Key))
							pRes.Add(p.Key, p.Value);
					}
				}

				foreach (var p in parametersValues)
				{
					if (p.Key.ToLower() == "id")
						pRes["oid"] = p.Value;
					else
						pRes[p.Key] = p.Value;
				}

				if (addReturnUrl && !pRes.ContainsKey("returnurl"))
				{
					pRes.Add("returnurl", UrlHelper.Current().ReturnUrl);
				}
				//@sad
				Href = "?" + pRes;
			}

			return this;
		}

		public string Link(string title)
		{
			Title = title;
			if (SecurableObjectKey.IsEmpty() || ActionAccessControl.Instance.Check(SecurableObjectKey))
				return AppLayout.Current.Link(this).ToString();
			else
				return title;
		}
		public string Link()
		{
			return Link(Title);
		}
		public string ImageTextLink(string title)
		{
			Title = title;
			if (SecurableObjectKey.IsEmpty() || ActionAccessControl.Instance.Check(SecurableObjectKey))
				return AppLayout.Current.ImageLink(this) + "&nbsp;" + AppLayout.Current.Link(this);
			else
				return AppLayout.Current.Image(Image, title) + "&nbsp;" + title;
		}
		public string ImageTextLink()
		{
			return ImageTextLink(Title);
		}
		public string ImageLink()
		{
			if (SecurableObjectKey.IsEmpty() || ActionAccessControl.Instance.Check(SecurableObjectKey))
				return AppLayout.Current.ImageLink(this).ToString();
			else
				return "";
		}

		public void Go()
		{
			HttpContext.Current.Response.Redirect(Href);
		}
	}
}