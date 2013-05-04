using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Nephrite.Web;
using Nephrite.Web.Controls;
using Nephrite.CMS.Model;
using System.Data.Linq.SqlClient;
using System.Linq.Expressions;

namespace Nephrite.CMS.View
{
	public partial class Content_list : ViewControl<IQueryable<V_SiteSection>>
	{
		protected Nephrite.CMS.Model.MM_ObjectType _c = null;
		protected void Page_Load(object sender, EventArgs e)
		{
			_c = AppCMS.DataContext.MM_ObjectTypes.Single(ot => ot.ObjectTypeID.ToString() == Query.GetString("classID"));
 

			SetTitle(_c.Title + " - Разделы");
			RenderMargin = false;
			toolbar.AddItemFilter(filter);
			toolbar.AddItemSeparator();
			toolbar.AddItem("add.png", "Создать раздел", String.Format("?mode=SiteSection&action=CreateNew&classid={2}&bgroup={0}&returnurl={1}",
		Query.GetString("bgroup"), Query.CreateReturnUrl(), Query.GetString("classid")));

			filter.AddFieldString<V_SiteSection>("Название", o => o.Title);
			
			toolbar.AddRightItemQuickFilter(qfilter);
			SearchExpression = s => (o => SqlMethods.Like(o.Title, "%" + s + "%"));
		}

		public Func<string, Expression<Func<V_SiteSection, bool>>> SearchExpression { get; set; }

		protected global::Nephrite.Web.Controls.Toolbar toolbar;
		protected global::Nephrite.Web.Controls.Filter filter;
		protected global::Nephrite.Web.Controls.TableDnD<SiteObject> tList;
		protected global::Nephrite.Web.Controls.QuickFilter qfilter;
		protected global::System.Web.UI.UpdatePanel up;
	}

}