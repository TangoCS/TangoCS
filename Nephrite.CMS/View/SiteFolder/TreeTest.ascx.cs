using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Nephrite.Web;
using Nephrite.Web.Controls;

namespace Nephrite.CMS.View
{
	public partial class SiteFolder_TreeTest : ViewControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RenderMargin = false;
            var x = new NodeTemplateStatic();
            x.Node.Title = "Узел 1";
            x.Node.ID = 11;
            x.Node.HasChildren = true;
            tree.Template.Add(x);
            var x1 = new NodeTemplateStatic();
            x1.Node.Title = "Узел 1.1";
            x1.Node.ID = 12;
            x.Children.Add(x1);
            var x2 = new NodeTemplateStatic();
            x2.Node.Title = "Узел 2";
            x2.Node.ID = 11;
            x2.Node.HasChildren = true;
            tree.Template.Add(x2);
            var x3 = new NodeTemplateStatic();
            x3.Node.Title = "Узел 2.1";
            x3.Node.ID = 12;
            x2.Children.Add(x3);

        }
    }
}