using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Nephrite.Web;

namespace Nephrite.Metamodel.View.Utils
{
    public partial class generatedb : ViewControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetTitle("Генерация таблиц БД");

            if (!IsPostBack)
            {
                cblObjectTypes.DataSource = AppMM.DataContext.MM_ObjectTypes.Where(o => o.IsSeparateTable).ToList().OrderBy(o => o.FullSysName);
                cblObjectTypes.DataBind();

                script.Visible = false;
                identity.Checked = true;
            }
        }

        protected void bSelectAll_Click(object sender, EventArgs e)
        {
            foreach (ListItem item in cblObjectTypes.Items)
                item.Selected = true;
        }


        protected void bGenerate_Click(object sender, EventArgs e)
        {
            script.Visible = true;
            DbGenerator gen = new DbGenerator(scriptOnly.Checked, identity.Checked);
            lMsg.Text = "";
            List<int> ids = new List<int>();
            foreach (ListItem item in cblObjectTypes.Items)
            {
                if (item.Selected)
                    ids.Add(item.Value.ToInt32(0));
            }
            
            var list = gen.GenerateObjectType(ids.ToArray());
            foreach (var s in list)
                lMsg.Text += s + "\r\n";
            
            script.Text = "";
            foreach (var s in gen.ResultScript)
                script.Text += s + "\r\n";
        }
    }
}