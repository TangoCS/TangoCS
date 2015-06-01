using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Collections.Generic;
using Nephrite.Identity;
using Nephrite.Multilanguage;
using Nephrite.AccessControl;
using Nephrite.Http;
using Nephrite.Html.Controls;

namespace Nephrite.Web.Controls
{
	public static class ToolbarExtensions
	{
		public static void AddItemFilter(this Toolbar toolbar, Filter filter)
		{
			string s = toolbar.Query.GetString("filterid");
			var textResource = toolbar.TextResource;

			if (!filter.HasValue)
				toolbar.AddItemJS(IconSet.Filter.X16, textResource.Get("Common.Toolbar.Filter", "Фильтр"), filter.RenderMethod());
			else
				toolbar.AddItemJS(IconSet.Filter.X16, "<b>" + textResource.Get("Common.Toolbar.Filter", "Фильтр") + "</b>", filter.RenderMethod());
		}

		public static void EnableViews(this Toolbar toolbar, Filter filter)
		{
			var textResource = toolbar.TextResource;
			
			toolbar.AddRightItemText(String.Format("<div>{0}</div>", textResource.Get("Common.Toolbar.View", "Представление") + ":"));

			ToolbarPopupMenuCompact mc = toolbar.AddRightPopupMenuCompact();
			string currentView = textResource.Get("Common.Toolbar.AllItems", "Все записи");
			int currentViewID = toolbar.Query.GetInt("filterid", 0);


			List<IN_Filter> views = filter.GetViews();
			IN_Filter defaultf = views.Where(o => o.IsDefault).OrderByDescending(o => o.SubjectID ?? 0).FirstOrDefault();
			if (defaultf != null)
			{
				mc.AddItem(defaultf.FilterName, UrlHelper.Current().RemoveParameter("filter").SetParameter("filterid", defaultf.FilterID.ToString()));
				mc.AddSeparator();
				if (defaultf.FilterID == currentViewID) currentView = defaultf.FilterName;
			}
			mc.AddItem(textResource.Get("Common.Toolbar.AllItems", "Все записи"), UrlHelper.Current().SetParameter("filter", "all").RemoveParameter("filterid"));
			bool isPersonal = false;
			foreach (IN_Filter f in views.Where(o => !o.IsDefault || (o.IsDefault && o.SubjectID == null && defaultf != null)).OrderBy(o => o.FilterName))
			{
				if (f.FilterID == currentViewID)
				{
					currentView = f.FilterName;
					isPersonal = f.SubjectID == Subject.Current.ID;
				}
				mc.AddItem(f.IsDefault ? "<b>" + f.FilterName + "</b>" : f.FilterName, UrlHelper.Current().RemoveParameter("filter").SetParameter("filterid", f.FilterID.ToString()));
			}

			if (defaultf != null && toolbar.Query.GetString("filter") != "all")
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

			if (currentViewID > 0 && (isPersonal || toolbar.AccessControl.Check("filter.managecommonviews", true)))
				mc.AddItemJS(textResource.Get("Common.Toolbar.EditThisView", "Изменить это представление"), filter.RenderEditViewMethod(currentViewID), IconSet.Modifyview.X16);

			mc.AddItemJS(textResource.Get("Common.Toolbar.CreateView", "Создать представление"), filter.RenderCreateViewMethod(), IconSet.Createview.X16);


			if (toolbar.Query.GetInt("filterid", 0) > 0 && !views.Any(o => o.FilterID == toolbar.Query.GetInt("filterid", 0)) &&
				toolbar.Query.GetString("filter") != "all")
				currentView = textResource.Get("Common.Toolbar.UserView", "Пользовательское");
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
			toolbar.AddItem(IconSet.Back.X16, toolbar.TextResource.Get("Common.Toolbar.Back"), UrlHelper.Current().ReturnUrl);
		}

		public static void AddItem(this Toolbar toolbar, ActionLink actionLink)
		{
			if (!actionLink.Url.IsEmpty())
				toolbar.AddItem(actionLink.ImageSrc, actionLink.Title, actionLink.Url);
		}
	}
}
