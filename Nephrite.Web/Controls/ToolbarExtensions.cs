using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using Nephrite.Web.SPM;
using Nephrite.TextResources;

namespace Nephrite.Web.Controls
{
	public static class ToolbarExtensions
	{
		public static void AddItemFilter(this Toolbar toolbar, Filter filter)
		{
			string s = Query.GetString("filterid");
			if (!filter.HasValue)
				toolbar.AddItemJS(IconSet.Filter.X16, TextResource.Get("Common.Toolbar.Filter", "Фильтр"), filter.RenderMethod());
			else
				toolbar.AddItemJS(IconSet.Filter.X16, "<b>" + TextResource.Get("Common.Toolbar.Filter", "Фильтр") + "</b>", filter.RenderMethod());
		}

		public static void EnableViews(this Toolbar toolbar, Filter filter)
		{
			toolbar.AddRightItemText(String.Format("<div>{0}</div>", TextResource.Get("Common.Toolbar.View", "Представление") + ":"));

			ToolbarPopupMenuCompact mc = toolbar.AddRightPopupMenuCompact();
			string currentView = TextResource.Get("Common.Toolbar.AllItems", "Все записи");
			int currentViewID = Query.GetInt("filterid", 0);


			List<IN_Filter> views = filter.GetViews();
			IN_Filter defaultf = views.Where(o => o.IsDefault).OrderByDescending(o => o.SubjectID ?? 0).FirstOrDefault();
			if (defaultf != null)
			{
				mc.AddItem(defaultf.FilterName, Url.Current.RemoveParameter("filter").SetParameter("filterid", defaultf.FilterID.ToString()));
				mc.AddSeparator();
				if (defaultf.FilterID == currentViewID) currentView = defaultf.FilterName;
			}
			mc.AddItem(TextResource.Get("Common.Toolbar.AllItems", "Все записи"), Url.Current.SetParameter("filter", "all").RemoveParameter("filterid"));
			bool isPersonal = false;
			foreach (IN_Filter f in views.Where(o => !o.IsDefault || (o.IsDefault && o.SubjectID == null && defaultf != null)).OrderBy(o => o.FilterName))
			{
				if (f.FilterID == currentViewID)
				{
					currentView = f.FilterName;
					isPersonal = f.SubjectID == Subject.Current.ID;
				}
				mc.AddItem(f.IsDefault ? "<b>" + f.FilterName + "</b>" : f.FilterName, Url.Current.RemoveParameter("filter").SetParameter("filterid", f.FilterID.ToString()));
			}

			if (defaultf != null && Query.GetString("filter") != "all")
			{
				if (currentViewID == 0 || currentViewID == defaultf.FilterID)
					currentView = defaultf.FilterName;

				if (currentViewID == 0)
				{
					currentViewID = defaultf.FilterID;
					filter.SetView(currentViewID);
				}
			}

			mc.AddSeparator();

			if (currentViewID > 0 && (isPersonal || ActionSPMContext.Current.Check("filter.managecommonviews", 1, true)))
				mc.AddItemJS(TextResource.Get("Common.Toolbar.EditThisView", "Изменить это представление"), filter.RenderEditViewMethod(currentViewID), IconSet.Modifyview.X16);

			mc.AddItemJS(TextResource.Get("Common.Toolbar.CreateView", "Создать представление"), filter.RenderCreateViewMethod(), IconSet.Createview.X16);


			if (Query.GetInt("filterid", 0) > 0 && !views.Any(o => o.FilterID == Query.GetInt("filterid", 0)) &&
				Query.GetString("filter") != "all")
				currentView = TextResource.Get("Common.Toolbar.UserView", "Пользовательское");
			mc.Title = "<b>" + currentView + "</b>";
		}

		public static void AddRightItemQuickFilter(this Toolbar toolbar, QuickFilter qf)
		{
			toolbar.AddRightItemText(qf.SearchTextBox);
			toolbar.Page.ClientScript.RegisterClientScriptBlock(qf.GetType(), "QF_wait", @"function QF_BeginRequest(sender, args)
{
	document.body.style.cursor = 'wait';
	document.getElementById('qfind').style.cursor = 'wait';
}
function QF_EndRequest(sender, args)
{
	document.body.style.cursor = 'default';
	document.getElementById('qfind').style.cursor = 'auto';
}", true);
			toolbar.Page.ClientScript.RegisterStartupScript(qf.GetType(), "QF_wait_start", @"Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(QF_BeginRequest);
Sys.WebForms.PageRequestManager.getInstance().add_endRequest(QF_EndRequest);", true);
		}

		public static void AddBackButton(this Toolbar toolbar)
		{
			toolbar.AddItem(IconSet.Back.X16, TextResource.Get("Common.Toolbar.Back"), Url.Current.ReturnUrl);
		}

		public static void AddItem(this Toolbar toolbar, ActionLink actionLink)
		{
			string img = actionLink.Image;
			if (img.IsEmpty())
			{
				switch (actionLink.Operation.Name)
				{
					case "Edit":
						img = "edititem.gif";
						break;
					case "Delete":
						img = "delete.gif";
						break;
					case "CreateNew":
						img = "add.png";
						break;
					case "ObjectChangeHistory":
						img = "clock.gif";
						break;
				}
			}
			toolbar.AddItem(img, actionLink.Operation.Caption, actionLink.Href, actionLink.TargetBlank);
		}
	}
}
