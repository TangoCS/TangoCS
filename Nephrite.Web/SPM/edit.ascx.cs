using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;


namespace Nephrite.Web.SPM
{
	public partial class edit : ViewControl<SPM_Role>
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			SetTitle("Роль");


			if (!IsPostBack)
			{
				if (ViewData.RoleID > 0)
				{
					tbTitle.Text = ViewData.Title;
					tbSysName.Text = ViewData.SysName;
				}
			}

			
		}

		protected void bOK_Click(object sender, EventArgs e)
		{
			ViewData.Title = tbTitle.Text;
			ViewData.SysName = tbSysName.Text;

			SPMController c = new SPMController();
			c.Update(ViewData);

			BaseController.RedirectTo<SPMController>(o => o.View(ViewData.RoleID));
		}
	}
}