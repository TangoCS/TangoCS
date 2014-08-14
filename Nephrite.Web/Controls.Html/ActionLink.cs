using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web.SPM;
using System.Configuration;
using System.Collections.Specialized;
using Nephrite.Web.Layout;
using Nephrite.Meta;

namespace Nephrite.Web.Controls
{
	public class SimpleLink : ILink
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string Image { get; set; }
		public string OnClick { get; set; }
		public bool TargetBlank { get; set; }
		public string AccessKey { get; set; }
		public object Attributes { get; set; }
		public string Href { get; set; }
	}

	public class ActionLink : SimpleLink
	{
		public MetaOperation Operation { get; private set; }

		public static ActionLink To(MetaOperation operation)
		{
			return new ActionLink(operation);
		}

		public ActionLink(MetaOperation operation)
		{
			Operation = operation;

			if (operation != null)
			{
				Title = operation.Caption;
				Image = operation.Image;
			}
		}

		public ActionLink With(HtmlParms parametersValues, bool addReturnUrl = true)
		{
			parametersValues.Add("bgroup", Url.Current.GetString("bgroup"));
			if (AppWeb.IsRouting)
				Href = Url.Create(Operation.Parent.Name, Operation.Name, parametersValues);
			else
			{
				HtmlParms pRes = new HtmlParms();
				pRes.Add("mode", Operation.Parent.Name);
				pRes.Add("action", Operation.Name);

				NameValueCollection qs = HttpContext.Current.Request.QueryString;
				HtmlParms pSave = new HtmlParms();
				bool m = false;

				for (int i = 0; i < qs.Count; i++)
				{
					string parm = qs.GetKey(i).ToLower();
					switch (parm)
					{
						case "mode":
							m = qs[i].ToLower() == Operation.Parent.Name.ToLower();
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
					pRes.Add("returnurl", Url.Current.ReturnUrl);
				}
				//@sad
				Href = "?" + pRes;
			}

			return this;
		}

		public string Link(string title)
		{
			Title = title;
			if (Operation == null || ActionSPMContext.Current.Check(Operation.ID, 1))
				return AppLayout.Current.Link(this).ToString();
			else
				return title;
		}
		public string Link()
		{
			return Link(Operation.Caption);
		}
		public string ImageTextLink(string title)
		{
			Title = title;
			if (Operation == null || ActionSPMContext.Current.Check(Operation.ID, 1))
				return AppLayout.Current.ImageLink(this) + "&nbsp;" + AppLayout.Current.Link(this);
			else
				return AppLayout.Current.Image(Image, title) + "&nbsp;" + title;
		}
		public string ImageTextLink()
		{
			return ImageTextLink(Operation.Caption);
		}
		public string ImageLink()
		{
			if (Operation == null || ActionSPMContext.Current.Check(Operation.ID, 1))
				return AppLayout.Current.ImageLink(this).ToString();
			else
				return "";
		}
	}
}