using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Nephrite.Web.Controls;

using Nephrite.Layout;

namespace Nephrite.Web.View
{
	public class StandardUndelete : ViewControl
	{
		Button bOK = new Button { Text = "Продолжить", CssClass = "ms-ButtonHeightWidth" };
		BackButton bBack = new BackButton();

		protected override void OnInit(EventArgs e)
		{
			bOK.Click += bOK_Click;

			Controls.Add(bOK);
			Controls.Add(bBack);
		}

		protected override void OnLoad(EventArgs e)
		{
			SetTitle((ViewData as IModelObject).Title + " &mdash; отмена удаления");
		}

		void bOK_Click(object sender, EventArgs e)
		{
			if (ViewData is IWithLogicalDelete)
			{
				(ViewData as IWithLogicalDelete).IsDeleted = false;
				A.Model.SubmitChanges();
			}			
			Query.RedirectBack();
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			var vd = ViewData as IModelObject;
			writer.Write(string.Format("<p>Вы уверены, что хотите отменить удаление {0} \"{1}\"</p>", vd.MetaClass.Caption, vd.Title));
			writer.Write(Layout.ButtonsBarBegin());
			writer.Write(Layout.ButtonsBarItemBegin());
			bOK.RenderControl(writer);
			writer.Write(Layout.ButtonsBarItemEnd());
			writer.Write(Layout.ButtonsBarItemBegin());
			bBack.RenderControl(writer);
			writer.Write(Layout.ButtonsBarItemEnd());
			writer.Write(Layout.ButtonsBarWhiteSpace());
			writer.Write(Layout.ButtonsBarEnd());
		}
	}
}