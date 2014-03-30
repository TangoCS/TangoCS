using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Nephrite.Meta;
using Nephrite.Meta.Forms;

namespace Nephrite.Web.FormsGenerator
{
	public partial class ViewCode
	{
		static string EditForm(FormElement form, string width)
		{
			return EditForm(form, width, String.Empty);
		}

		static bool IsRequired(FormElement p)
		{
			return p.IsRequired;
		}

		static string EditForm(FormElement form, string width, string prefix)
		{
			StringBuilder res = new StringBuilder();

			var cols = form.Content.Where(o => o.Content == null);
			var groups = form.Content.Where(o => o.Content != null);

			string w = !String.IsNullOrEmpty(width) ? ("\"" + width + "\"") : "";

			if (cols.Count() > 0)
			{
				res.AppendLine("<%=Layout.FormTableBegin(" + w + ")%>");
				foreach (var p in cols)
				{
					ControlCode c = ControlCode.Create(p, prefix);
					var ml = p.IsMultilingual;
					res.AppendLine(@"<%=Layout.FormRowBegin(""" + p.Caption + @"""" + (ml ? @"+"" "" + Html.Image(""ml.gif"", ""Данное поле является многоязычным"")" : "") + (IsRequired(p) ? ", true" : "") + ")%>");
					if (c != null) res.AppendLine(c.Control());
					res.AppendLine("<%=Layout.FormRowEnd()%>");
				}
				res.AppendLine("<%=Layout.FormTableEnd() %>");
			}

			foreach (var g in groups)
			{
				FieldGroupSelectObject fgc = new FieldGroupSelectObject(g, prefix);

				res.AppendLine("<%=Layout.GroupTitleBegin()%>");
				if (g.Type != null)
				{
					res.Append(g.Caption).Append("&nbsp;");
					res.AppendLine(fgc.Control());
				}
				else
				{
					res.AppendLine(g.Caption);
				}
				res.AppendLine("<%=Layout.GroupTitleEnd()%>");

				if (g.Type != null)
				{
					res.AppendLine(@"<asp:UpdatePanel ID=""up" + fgc.ControlID + @""" runat=""server"" UpdateMode=""Conditional"">");
					res.AppendLine("<ContentTemplate>");
				}
				res.AppendLine("<%=Layout.FormTableBegin(" + w + ")%>");
				foreach (var p in g.Content)
				{
					ControlCode c = ControlCode.Create(p, prefix);
					var ml = p.IsMultilingual;
					if (!String.IsNullOrEmpty(p.Description))
						res.AppendLine(@"<%=Layout.FormRowBegin(""" + p.Caption + @"""" + (ml ? @"+"" "" + Html.Image(""ml.gif"", ""Данное поле является многоязычным"")" : "") + @", """ + p.Description + (IsRequired(p) ? @""", true" : @""", false") + ")%>");
					else
						res.AppendLine(@"<%=Layout.FormRowBegin(""" + p.Caption + @"""" + (ml ? @"+"" "" + Html.Image(""ml.gif"", ""Данное поле является многоязычным"")" : "") + (IsRequired(p) ? ", true" : "") + ")%>");
					if (c != null) res.AppendLine(c.Control());
					res.AppendLine("<%=Layout.FormRowEnd()%>");
				}
				res.AppendLine("<%=Layout.FormTableEnd() %>");
				if (g.Type != null)
				{
					res.AppendLine("</ContentTemplate>");
					res.AppendLine("</asp:UpdatePanel>");
				}
			}

			return res.ToString();
		}


		public static string Edit(FormElement form)
		{
			string idCol = (form.Type as MetaClass).Key.Name;

			var cols = form.Content.Where(o => o.Content == null);
			var groups = form.Content.Where(o => o.Content != null);
	
			var selectobjectgroups = groups.Where(o => o.Type != null);

			ControlCode c = null;
			StringBuilder res = new StringBuilder();

			#region form
			res.AppendLine(EditForm(form, "700px"));
			res.AppendLine("");
			foreach (var p in cols)
			{
				c = ControlCode.Create(p);
				if (c != null)
				{
					string s = c.RootLayout();
					if (!String.IsNullOrEmpty(s))
					{
						res.AppendLine(s);
						res.AppendLine("");
					}
				}
			}
			foreach (var g in selectobjectgroups)
			{
				FieldGroupSelectObject fgc = new FieldGroupSelectObject(g);
				res.AppendLine(fgc.RootLayout());
			}
			res.AppendLine(@"<asp:Label ID=""lMsg"" runat=""server"" Text="""" ForeColor=""Red"" />");


			res.AppendLine("<%=Layout.ButtonsBarBegin() %>");
			res.AppendLine("<%=Layout.ButtonsBarWhiteSpace() %>");
			res.AppendLine("<%=Layout.ButtonsBarItemBegin() %>");
			res.AppendLine(@"<nw:SubmitButton ID=""bOK"" runat=""server"" Text=""ОК"" onclick=""bOK_Click"" />");
			res.AppendLine("<%=Layout.ButtonsBarItemEnd() %>");
			res.AppendLine("<%=Layout.ButtonsBarItemBegin() %>");
			res.AppendLine(@"<nw:BackButton runat=""server"" ID=""BackButton"" />");
			res.AppendLine("<%=Layout.ButtonsBarItemEnd() %>");
			res.AppendLine("<%=Layout.ButtonsBarEnd() %>");
			#endregion

			res.AppendLine(@"<script runat=""server"">");

			#region page load
			res.AppendLine("protected void Page_Load(object sender, EventArgs e)");
			res.AppendLine("{");
			res.AppendLine("\tSetTitle(\"" + form.Caption + "\");");
			res.AppendLine("\tlMsg.Text = \"\";");
			res.AppendLine("");
			foreach (var p in cols)
			{
				c = ControlCode.Create(p);
				if (c != null)
				{
					string s = c.ViewOnLoad();
					if (!String.IsNullOrEmpty(s))
						res.AppendLine("\t" + s);
				}
			}


			res.AppendLine("");
			res.AppendLine("\tif (!IsPostBack)");
			res.AppendLine("\t{");

			foreach (var p in cols)
			{
				c = ControlCode.Create(p);
				if (c != null)
				{
					string s = c.Init();
					if (!String.IsNullOrEmpty(s))
						res.AppendLine("\t\t" + s);
				}
			}

			/*res.AppendLine("\t\tif (ViewData." + idCol + " > 0)");
			res.AppendLine("\t\t{");*/

			foreach (var p in cols)
			{
				c = ControlCode.Create(p);
				if (c != null) res.AppendLine("\t\t" + c.Load("ViewData"));
			}
			foreach (var g in selectobjectgroups)
			{
				FieldGroupSelectObject fgc = new FieldGroupSelectObject(g);
				res.AppendLine("\t\t" + fgc.Load("ViewData"));
			}

			res.AppendLine("\t}");
			res.AppendLine("");
			foreach (var g in selectobjectgroups)
			{
				FieldGroupSelectObject fgc = new FieldGroupSelectObject(g);
				res.AppendLine("\t" + fgc.ViewOnLoad());
			}
			res.AppendLine("");
			res.AppendLine("}");
			#endregion

			res.AppendLine("");
			res.AppendLine("protected void bOK_Click(object sender, EventArgs e)");
			res.AppendLine("{");

			#region fields validate
			res.AppendLine("\tstring requiredMsg = \"\";");

			foreach (var p in cols.Where(o => o.IsRequired))
			{
				c = ControlCode.Create(p);
				if (c != null)
				{
					string v = c.ValidateRequired();
					if (!String.IsNullOrEmpty(v)) res.AppendLine("\t" + v);
				}
			}
			foreach (var g in groups)
				foreach (var p in g.Content.Where(o => o.IsRequired))
				{
					c = ControlCode.Create(p);
					if (c != null)
					{
						string v = c.ValidateRequired();
						if (!String.IsNullOrEmpty(v)) res.AppendLine("\t" + v);
					}
				}

			foreach (var g in selectobjectgroups)
			{
				FieldGroupSelectObject fgc = new FieldGroupSelectObject(g);
				res.AppendLine("\t" + fgc.ValidateRequired());
			}


			res.AppendLine("\tif (!String.IsNullOrEmpty(requiredMsg))");
			res.AppendLine("\t{");
			res.AppendLine("\t\tlMsg.Text = \"<div class='savewarning'>Необходимо задать значения следующих полей:<ul>\" + requiredMsg + \"</ul></div>\";");
			res.AppendLine("\t\treturn;");
			res.AppendLine("\t}");

			#endregion

			foreach (var p in cols)
			{
				c = ControlCode.Create(p);
				if (c != null)
					res.AppendLine("\t" + c.Save("ViewData"));
			}

			foreach (var g in selectobjectgroups)
			{
				FieldGroupSelectObject fgc = new FieldGroupSelectObject(g);
				res.AppendLine(fgc.Save("ViewData"));
			}

			res.AppendLine("");
			res.AppendLine("");

			res.AppendLine("\t" + form.Type.Name + "Controller c = new " + form.Type.Name + "Controller();");
			res.AppendLine("\tc.Update(ViewData);");
			foreach (var p in cols)
			{
				c = ControlCode.Create(p);
				if (c != null)
				{
					string s = c.AfterSubmit();
					if (!String.IsNullOrEmpty(s))
						res.AppendLine("\t\t" + s);
				}
			}
			res.AppendLine("");
			res.AppendLine("\tQuery.RedirectBack();");

			res.AppendLine("}");

			foreach (var g in selectobjectgroups)
			{
				FieldGroupSelectObject fgc = new FieldGroupSelectObject(g);
				res.AppendLine(fgc.Events());
			}
			foreach (var p in cols)
			{
				c = ControlCode.Create(p);
				if (c != null)
				{
					string s = c.Events();
					if (!String.IsNullOrEmpty(s))
						res.AppendLine("\t" + s);
				}
			}

			res.AppendLine("</script>");

			return res.ToString();
		}
	}
}