using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Nephrite.Web;

namespace Tessera
{
    public partial class Rss : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            p.Controls.Add(LoadControl("/" + Query.GetString("rss") + ".rss.ascx"));
        }
    }
}
