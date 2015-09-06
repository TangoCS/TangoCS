using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Nephrite.Web.Controls;

using Nephrite.Layout;
using Nephrite.MVC;
using Nephrite.Multilanguage;

namespace Nephrite.Web.View
{
	public class StandardDelete : ViewControl
	{
		[Inject]
		public ITextResource TextResource { get; set; }

		SubmitButton bOK = new SubmitButton { CssClass = "ms-ButtonHeightWidth" };
		BackButton bBack = new BackButton();

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);			
            bOK.Click += bOK_Click;

			Controls.Add(bOK);
			Controls.Add(bBack);
		}

		protected override void OnLoad(EventArgs e)
		{
			SetTitle((ViewData as IWithTitle).Title + " &mdash; " + TextResource.Get("Common.Delete.Deletion"));
			bOK.Text = TextResource.Get("Common.Delete");
		}

		void bOK_Click(object sender, EventArgs e)
		{
			var preDel = LogicHelper.GetObjectLogic(ViewData, "PreDelete");
			if (preDel != null) preDel.DynamicInvoke(ViewData);

			var del = LogicHelper.GetObjectLogic(ViewData, "Delete");
			if (del == null)
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
			}
			else
			{
				del.DynamicInvoke(ViewData);
			}

			Query.RedirectBack();
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			var vd = ViewData as IModelObject;
			string title = vd.Title.IsEmpty() ? (vd.ObjectID == 0 ? vd.ObjectGUID.ToString() : vd.ObjectID.ToString()) : vd.Title;
            writer.Write(string.Format("<p>{2} {0} \"{1}\"</p>", vd.MetaClass.Caption.ToLower(), title, TextResource.Get("Common.Delete.Confirm")));
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