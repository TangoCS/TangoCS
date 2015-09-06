using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Nephrite.Web.Controls;

using Nephrite.Layout;
using Nephrite.Multilanguage;

namespace Nephrite.Web.View
{
	public class StandardUndelete : ViewControl
	{
		[Inject]
		public ITextResource TextResource { get; set; }

		Button bOK = new Button { CssClass = "ms-ButtonHeightWidth" };
		BackButton bBack = new BackButton();

		protected override void OnInit(EventArgs e)
		{
			bOK.Click += bOK_Click;
			bOK.Text = TextResource.Get("Common.Continue");

			Controls.Add(bOK);
			Controls.Add(bBack);
		}

		protected override void OnLoad(EventArgs e)
		{
			SetTitle((ViewData as IModelObject).Title + " &mdash; " + TextResource.Get("Common.Undelete.Undeletion"));
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
			string title = vd.Title.IsEmpty() ? (vd.ObjectID == 0 ? vd.ObjectGUID.ToString() : vd.ObjectID.ToString()) : vd.Title;
			writer.Write(string.Format("<p>{2} {0} \"{1}\"</p>", vd.MetaClass.Caption.ToLower(), title, TextResource.Get("Common.Undelete.Confirm")));
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