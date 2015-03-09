using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Nephrite.Web.Controls;

using Nephrite.Layout;

namespace Nephrite.Web.View
{
	public class StandardDelete : ViewControl
	{
		Button bOK = new Button { Text = "Удалить", CssClass = "ms-ButtonHeightWidth" };
		BackButton bBack = new BackButton();

		protected override void OnInit(EventArgs e)
		{
			bOK.Click += bOK_Click;

			Controls.Add(bOK);
			Controls.Add(bBack);
		}

		protected override void OnLoad(EventArgs e)
		{
			SetTitle((ViewData as IModelObject).Title + " &mdash; удаление"); 
		}

		void bOK_Click(object sender, EventArgs e)
		{
			if (ViewData is IWithLogicalDelete)
			{
				(ViewData as IWithLogicalDelete).IsDeleted = true;
			}
			else
			{
				A.Model.GetTable(ViewData.GetType()).DeleteOnSubmit(ViewData);
			}
			A.Model.SubmitChanges();
			Query.RedirectBack();
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			var vd = ViewData as IModelObject;
			writer.Write(string.Format("<p>Вы уверены, что хотите удалить {0} \"{1}\"</p>", vd.MetaClass.Caption, vd.Title));
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