using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Nephrite.Identity;
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
			//if (!ActionSPMContext.Current.Check(node.FURL + ".View", 1, true))
			//{
			//	LiteralControl l = new LiteralControl("У вас нет прав просматривать данную страницу");
			//	contents[0].Control.Controls.Add(l);
			//	return;
			//}

			foreach (ContentPlaceHolderInfo content in contents)
			{
				HttpContext.Current.Items["ViewContainer"] = content.Control;
				if (!node.ContentPlaceHolderRenderers.ContainsKey(content.Control.ID)) continue;
				if (node.ContentPlaceHolderRenderers[content.Control.ID] == null) continue;
				node.ContentPlaceHolderRenderers[content.Control.ID]();
			}
		}

		public override Toolbar GetToolbar()
		{
			return null;
		}

		//public override ButtonBar GetButtonBar()
		//{
		//	return null;
		//}
	}

	public class ContentPlaceHolderInfo
	{
		public string Title { get; set; }
		public Control Control { get; set; }
	}
}