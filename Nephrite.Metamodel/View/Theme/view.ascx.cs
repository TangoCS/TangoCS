using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Nephrite.Web;
using Nephrite.Metamodel.Model;
using Nephrite.Web.App;

namespace Nephrite.Metamodel.View
{
    public partial class Theme_view : ViewControl<List<Theme>>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetTitle("Тема");

            if (!IsPostBack)
            {
                lbTheme.DataSource = ViewData;
                lbTheme.DataBind();
                lbTheme.Items.Insert(0, new ListItem("По умолчанию", ""));
                if (Query.GetString("showtheme") != "")
                    lbTheme.SelectedValue = Query.GetString("showtheme");
            }
        }

        protected void lbTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            Response.Redirect(Query.RemoveParameter("showtheme") + "&showtheme=" + lbTheme.SelectedValue);
        }

        protected void bOK_Click(object sender, EventArgs e)
        {
            AppSettings.Set("theme", lbTheme.SelectedValue);
            Response.Redirect(Query.RemoveParameter("showtheme"));
        }
    }
}