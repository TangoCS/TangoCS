using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Nephrite.Web;
using Microsoft.SqlServer.Management.Smo;
using Nephrite.Metamodel.Controllers;

namespace Nephrite.Metamodel.View
{
	public partial class Triggers_edit : ViewControl
	{
		Trigger trigger;
		DbGenerator gen = new DbGenerator(false, false);
			
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Query.GetString("action").ToLower() != "createnew")
			{
				trigger = gen.GetTableTrigger(Query.GetString("tablename"), Query.GetString("triggername"));
				if (trigger == null)
					Query.RedirectBack();
			}
			if (!IsPostBack)
			{
				ddlTableName.DataBindOnce(gen.GetTables());
				if (trigger == null)
				{
					tbTemplate.Area.Value = "BEGIN\r\n\r\nEND";
					cbEnabled.Checked = true;
					cbInsert.Checked = true;
					cbDelete.Checked = true;
					cbUpdate.Checked = true;
					ddlTableName_SelectedIndexChanged(null, EventArgs.Empty);
				}
				else
				{
					tbTemplate.Area.Value = trigger.TextBody;
					ddlTableName.SetValue(((Microsoft.SqlServer.Management.Smo.Table)trigger.Parent).Name);
					tbName.Text = trigger.Name;

					ddlTableName.Enabled = false;
					cbEnabled.Checked = trigger.IsEnabled;
					cbInsert.Checked = trigger.Insert;
					cbDelete.Checked = trigger.Delete;
					cbUpdate.Checked = trigger.Update;
				}
			}
			SetTitle("Триггер");
			ScriptManager.RegisterOnSubmitStatement(this, GetType(), "disableApply", "document.getElementById('" + bApply.ClientID + "').disabled = true;");
			//Page.ClientScript.RegisterClientScriptInclude("jquery", Nephrite.Web.Settings.JSPath + "jquery-1.2.6.min.js");
			//Page.ClientScript.RegisterClientScriptInclude("jquery.textarea", Nephrite.Web.Settings.JSPath + "jquery.textarea.js");
			//Page.ClientScript.RegisterStartupScript(GetType(), "textareatab", "$(document).ready(function () { $('textarea').tabby();});", true);
			Cramb.Add("Триггеры", Html.ActionUrl<TriggersController>(c => c.ViewList()));
			if (Query.GetString("action").ToLower() != "createnew")
				Cramb.Add(((Microsoft.SqlServer.Management.Smo.Table)trigger.Parent).Name + "." + trigger.Name);
			else
				Cramb.Add("Создание");
		}

		bool needRedirect = true;
		bool Save()
		{
			try
			{
				msg.Text = "";
				gen.BeginTransaction();
				if (trigger != null)
				{
					needRedirect = trigger.Name != tbName.Text;
					trigger.Drop();
				}

				trigger = gen.CreateTrigger(ddlTableName.SelectedValue, tbName.Text);
				trigger.TextMode = false;
				trigger.Insert = cbInsert.Checked;
				trigger.Delete = cbDelete.Checked;
				trigger.Update = cbUpdate.Checked;
				trigger.IsEnabled = cbEnabled.Checked;
				trigger.TextBody = tbTemplate.Area.Value;
				trigger.Create();

				gen.CommitTransaction();
				msg.Text = "<span style=\"color:DarkGreen; font-size:15px; font-weight:bold\">Триггер обновлен!</span>";

				ddlTableName.Enabled = false;
				tbName.ReadOnly = true;
				up.Update();
				return true;
			}
			catch (Exception e)
			{
				gen.RollBackTransaction();
				while (e != null)
				{
					msg.Text += "<pre>" + e.Message + "</pre>";
					e = e.InnerException;
				}
				up.Update();
				return false;
			}

		}

		protected void bOK_Click(object sender, EventArgs e)
		{
			if (Save())
				Query.RedirectBack();
		}

		protected void bApply_Click(object sender, EventArgs e)
		{
			if (Save() && needRedirect)
				BaseController.RedirectTo<TriggersController>(c => c.Edit(ddlTableName.SelectedValue, tbName.Text, Query.GetString("returnurl")));
		}

		protected void ddlTableName_SelectedIndexChanged(object sender, EventArgs e)
		{
			tbName.Text = "t" + (cbUpdate.Checked ? "U" : "") + (cbInsert.Checked ? "I" : "") + (cbDelete.Checked ? "D" : "") + "_" + ddlTableName.SelectedValue;
		}
		
	}
}