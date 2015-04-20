using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Nephrite.Identity;
using Nephrite.Web.Controls;

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

			foreach (ContentPlaceHolderInfo content in contents)
			{
				//HttpContext.Current.Items["ViewContainer"] = content.Control;
				if (!node.ContentPlaceHolderRenderers.ContainsKey(content.Control.ID)) continue;
				if (node.ContentPlaceHolderRenderers[content.Control.ID] == null) continue;
				node.ContentPlaceHolderRenderers[content.Control.ID](content.Control);
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