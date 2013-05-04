using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Nephrite.Web.SPM
{
	public partial class delete : ViewControl<SPM_Role>
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			SetTitle("Роль - удаление");
			lo.SetObject(ViewData);
		}

		protected void bDelete_Click(object sender, EventArgs e)
		{
			AppSPM.DataContext.SPM_RoleAssos.DeleteAllOnSubmit(ViewData.SPM_RoleAssos);
			AppSPM.DataContext.SPM_Roles.DeleteOnSubmit(ViewData);
			AppSPM.DataContext.SPM_CasheFlags.First().IsChange = true;
			AppSPM.DataContext.SubmitChanges();
			AppSPM.Instance.RefreshCache();
			AppSPM.AccessRightManager.RefreshCache();
			BaseController.RedirectTo<SPMController>(c => c.ViewList());
		}
	}
}