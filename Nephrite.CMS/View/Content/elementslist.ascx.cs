using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Nephrite.Web;
using Nephrite.Web.Controls;
using Nephrite.CMS.Model;
using Nephrite.Metamodel;
using System.Linq.Expressions;
using System.Data.Linq.SqlClient;
using Nephrite.CMS.Controllers;


namespace Nephrite.CMS.View
{
	public partial class Content_elementslist : ViewControl<IQueryable<V_SiteObject>>
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			MM_ObjectType c = AppCMS.DataContext.MM_ObjectTypes.Single(ot => ot.ObjectTypeID.ToString() == Query.GetString("classid"));
			V_SiteSection ss = AppCMS.DataContext.V_SiteSections.Where(o => o.LanguageCode == AppMM.CurrentLanguage.LanguageCode).Single(o => o.SiteSectionID.ToString() == Query.GetString("sectionid"));
			
			SetTitle(c.TitlePlural + " раздела \"" + ss.Title + "\"");

			Page.ClientScript.RegisterClientScriptInclude("tablednd", Settings.JSPath + "jquery.tablednd_0_5.js");
			RenderMargin = false;

			toolbar.AddItemFilter(filter);
			toolbar.AddItemSeparator();
			toolbar.AddItem("add.png", "Создать", String.Format("?mode={0}&action=CreateNew&parent={1}&bgroup={2}&returnurl={3}",c.SysName, ss.ObjectID, Query.GetString("bgroup"), Query.CreateReturnUrl()));

			filter.AddFieldString<V_SiteObject>("Название", o => o.Title);
			filter.AddFieldString<V_SiteObject>("Дата создания", o => o.CreateDate);
			filter.AddFieldString<V_SiteObject>("Дата публикации", o => o.PublishDate);

			toolbar.AddRightItemQuickFilter(qfilter);
			SearchExpression = s => (o => SqlMethods.Like(o.Title, "%" + s + "%"));

			Cramb.Add(c.Title, Html.ActionUrl<ContentController>(cc => cc.ViewList(c.ObjectTypeID.ToString())));
			Cramb.Add(ss.Title, "");

			tList.DataContext = AppCMS.DataContext;
			tList.ObjectList = AppCMS.DataContext.SiteObjects;
		}

		public Func<string, Expression<Func<V_SiteObject, bool>>> SearchExpression { get; set; }

		protected global::Nephrite.Web.Controls.Toolbar toolbar;
		protected global::Nephrite.Web.Controls.Filter filter;
		protected global::Nephrite.Web.Controls.TableDnD<SiteObject> tList;
		protected global::Nephrite.Web.Controls.QuickFilter qfilter;
		protected global::System.Web.UI.UpdatePanel up;
		//protected global::System.Web.UI.WebControls.HiddenField hfQuickFilter;
		//protected global::System.Web.UI.WebControls.LinkButton lbRefresh;
	}
}