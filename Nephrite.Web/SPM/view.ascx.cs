using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web.Hosting;

namespace Nephrite.Web.SPM
{
	public partial class view : ViewControl<SPM_Role>
	{
		protected List<ADUser> _findUsers
		{
			get
			{
				return ViewState["_findUsers"] as List<ADUser>;
			}
			set
			{
				ViewState["_findUsers"] = value;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			SetTitle("Роль");

			toolbar.AddItem<SPMController>("edititem.gif", "Редактировать", c => c.Edit(ViewData.RoleID));
			toolbar.AddItemSeparator();
			toolbar.AddItem<SPMController>("delete.gif", "Удалить", "return confirm('Вы действительно хотите удалить текущую выбранную роль?')", c => c.Delete(ViewData.RoleID));

			BackButton1.Url = BackButton2.Url = Html.ActionUrl<SPMController>(c => c.ViewList());
		}

		protected void bOK_Click(object sender, EventArgs e)
		{
			string[] checks = Request.Form.AllKeys.Where(o => o.StartsWith("m_")).ToArray();
			AppSPM.DataContext.SPM_RoleAccesses.DeleteAllOnSubmit(
				AppSPM.DataContext.SPM_RoleAccesses.Where(o => o.RoleID == ViewData.RoleID)
				);
			foreach (string check in checks)
			{
				string[] s = check.Split(new char[] { '_' });
				if (Request.Form[check] == "on")
				{
					SPM_RoleAccess access = new SPM_RoleAccess();
					access.RoleID = ViewData.RoleID;
					access.ActionID = Convert.ToInt32(s[1]);
					AppSPM.DataContext.SPM_RoleAccesses.InsertOnSubmit(access);
				}
			}


			SPMController c = new SPMController();
			c.Update(ViewData);
            Response.Redirect(Request.Url.AbsoluteUri);
		}

		protected void adselect_Populate(object sender, EventArgs e)
		{
			if (adselect.IsFirstPopulate)
			{
				cblUsers.Items.Clear();
				tbAdFilter.Text = "";
			}
		}

		protected void adselect_OKClick(object sender, EventArgs e)
		{
			foreach (ListItem li in cblUsers.Items)
				if (li.Selected)
				{
					ADUser u = _findUsers.Where(o => o.Sid == li.Value).First();


					SPM_Subject s = AppSPM.DataContext.SPM_Subjects.SingleOrDefault(o => o.SID == u.Sid);
					if (s == null)
					{
						s = new SPM_Subject();
						AppSPM.DataContext.SPM_Subjects.InsertOnSubmit(s);
					}
					s.Title = u.Title;
					s.SID = u.Sid;
					s.SystemName = u.Login;
					

					SPM_SubjectRole sr = new SPM_SubjectRole();
					sr.RoleID = ViewData.RoleID;
					sr.SPM_Subject = s;
					AppSPM.DataContext.SPM_SubjectRoles.InsertOnSubmit(sr);
				}

			SPMController c = new SPMController();
			c.Update(ViewData);
		}

		protected void bAdFind_Click(object sender, EventArgs e)
		{


			try
			{
				using (HostingEnvironment.Impersonate())
				{

					List<string> current = AppSPM.DataContext.SPM_SubjectRoles.Where(o => o.RoleID == ViewData.RoleID).Select(o => o.SPM_Subject.SID).ToList();
					_findUsers = ADSearcher.Search(tbAdFilter.Text, tbAdFilter.Text, tbAdFilter.Text).Where(o => !current.Contains(o.Sid)).ToList();
					cblUsers.DataSource = _findUsers.OrderBy(o => o.Title);
					cblUsers.DataBind();
				}
			}
			catch(Exception ex)
			{
				ErrorLogger.Log(ex, AppSPM.DataContext);
				throw;
			}
		}

		protected void roleselect_Populate(object sender, EventArgs e)
		{
			if (roleselect.IsFirstPopulate)
			{
				List<int> current = AppSPM.DataContext.SPM_RoleAssos.Where(o => o.RoleID == ViewData.RoleID).Select(o => o.ParentRoleID).ToList();
				cblRoles.DataSource = AppSPM.DataContext.SPM_Roles.Where(o => o.RoleID != ViewData.RoleID && !current.Contains(o.RoleID));
				cblRoles.DataBind();
				lMsg.Text = "";
			}
		}

		protected void roleselect_OKClick(object sender, EventArgs e)
		{
			foreach (ListItem li in cblRoles.Items)
				if (li.Selected)
				{
					SPM_Role role = AppSPM.DataContext.SPM_Roles.SingleOrDefault(o => o.RoleID == int.Parse(li.Value));
					if (GetInhitedRoles(role).Select(o => o.RoleID).Contains(ViewData.RoleID))
					{
						lMsg.Text = "Нельзя наследовать полномочия от роли " + role.Title + ", так как она сама наследует полномочия от роли " + ViewData.Title + "!";
						roleselect.Reopen();
						return;
					}
					SPM_RoleAsso ra = new SPM_RoleAsso();
					AppSPM.DataContext.SPM_RoleAssos.InsertOnSubmit(ra);
					ra.SPM_ParentRole = role;
					ra.SPM_Role = ViewData;
				}

			SPMController c = new SPMController();
			c.Update(ViewData);
		}

		public List<SPM_Role> GetInhitedRoles(SPM_Role role)
		{
			List<SPM_Role> roles = new List<SPM_Role>();
			foreach (var r in role.SPM_RoleAssos.Select(o => o.SPM_ParentRole))
			{
				roles.AddRange(GetInhitedRoles(r));
				roles.Add(r);
			}
			return roles;
		}
	}
}