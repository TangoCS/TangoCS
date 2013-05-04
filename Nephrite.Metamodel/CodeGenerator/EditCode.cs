using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Metamodel.Model;
using System.Text;

namespace Nephrite.Metamodel
{
	public partial class ViewCode
	{
		static string EditForm(MM_ObjectType objectType, string width)
		{
			return EditForm(objectType, width, String.Empty);
		}

		static bool IsRequired(MM_FormField p)
		{
			return p.MM_ObjectProperty.LowerBound > 0 && 
				(!p.ControlName.HasValue ||
				(p.ControlName.HasValue && p.ControlName.Value != (int)FormControl.Label && p.ControlName.Value != (int)FormControl.CheckBox));
		}

		static string EditForm(MM_ObjectType objectType, string width, string prefix)
		{
			StringBuilder res = new StringBuilder();

			List<MM_FormField> cols = (from f in AppMM.DataContext.MM_FormFields
					   where f.MM_ObjectProperty.ObjectTypeID == objectType.BaseObjectTypeID
					   && f.ShowInEdit && !f.MM_ObjectProperty.IsAggregate
					   orderby f.SeqNo
					   select f).ToList();
			cols.AddRange((from f in AppMM.DataContext.MM_FormFields
					   where f.MM_ObjectProperty.ObjectTypeID == objectType.ObjectTypeID
					   && f.ShowInEdit && !f.MM_ObjectProperty.IsAggregate
					   orderby f.SeqNo
					   select f).ToList());
			
			var groups = from g in objectType.MM_FormFieldGroups orderby g.SeqNo select g;
			var nongroupcols = cols.Where(o => !o.FormFieldGroupID.HasValue);

			string w = !String.IsNullOrEmpty(width) ? ("\"" + width + "\"") : "";

			if (nongroupcols.Count() > 0)
			{
				res.AppendLine("<%=Layout.FormTableBegin(" + w + ")%>");
				foreach (MM_FormField p in nongroupcols)
				{
					ControlCode c = ControlCode.Create(p, prefix);
					res.AppendLine(@"<%=Layout.FormRowBegin(""" + p.MM_ObjectProperty.Title + @"""" + (p.MM_ObjectProperty.IsMultilingual ? @"+"" "" + Html.Image(""ml.gif"", ""Данное поле является многоязычным"")" : "") + (IsRequired(p) ? ", true" : "") + ")%>");
					if (c != null) res.AppendLine(c.Control());
					res.AppendLine("<%=Layout.FormRowEnd()%>");
				}
				res.AppendLine("<%=Layout.FormTableEnd() %>");
			}

			foreach (MM_FormFieldGroup g in groups.Where(o => o.MM_FormFields.Count > 0))
			{
				FieldGroupSelectObject fgc = new FieldGroupSelectObject(g, prefix);

				res.AppendLine("<%=Layout.GroupTitleBegin()%>");
				if (g.SelectObjectPropertyID.HasValue || !String.IsNullOrEmpty(g.SelectObjectClass))
				{
					if (g.ShowTitle) res.Append(g.Title).Append("&nbsp;");
					res.AppendLine(fgc.Control());
				}
				else
				{
					if (g.ShowTitle) res.AppendLine(g.Title);
				}
				res.AppendLine("<%=Layout.GroupTitleEnd()%>");

				if (g.SelectObjectPropertyID.HasValue || !String.IsNullOrEmpty(g.SelectObjectClass))
				{
					res.AppendLine(@"<asp:UpdatePanel ID=""up" + fgc.ControlID + @""" runat=""server"" UpdateMode=""Conditional"">");
					res.AppendLine("<ContentTemplate>");
				}
				res.AppendLine("<%=Layout.FormTableBegin(" + w + ")%>");
				foreach (MM_FormField p in g.MM_FormFields.Where(o => o.ShowInEdit).OrderBy(o => o.SeqNo))
				{
					ControlCode c = ControlCode.Create(p, prefix);
					if (!String.IsNullOrEmpty(p.Comment))
						res.AppendLine(@"<%=Layout.FormRowBegin(""" + p.MM_ObjectProperty.Title + @"""" + (p.MM_ObjectProperty.IsMultilingual ? @"+"" "" + Html.Image(""ml.gif"", ""Данное поле является многоязычным"")" : "") + @", """ + p.Comment + (IsRequired(p) ? @""", true" : @""", false") + ")%>");
					else
						res.AppendLine(@"<%=Layout.FormRowBegin(""" + p.MM_ObjectProperty.Title + @"""" + (p.MM_ObjectProperty.IsMultilingual ? @"+"" "" + Html.Image(""ml.gif"", ""Данное поле является многоязычным"")" : "") + (IsRequired(p) ? ", true" : "") + ")%>");
					if (c != null) res.AppendLine(c.Control());
					res.AppendLine("<%=Layout.FormRowEnd()%>");
				}
				res.AppendLine("<%=Layout.FormTableEnd() %>");
				if (g.SelectObjectPropertyID.HasValue || !String.IsNullOrEmpty(g.SelectObjectClass))
				{
					res.AppendLine("</ContentTemplate>");
					res.AppendLine("</asp:UpdatePanel>");
				}
			}

			var modalcols = from f in AppMM.DataContext.MM_FormFields
					   where f.MM_ObjectProperty.ObjectTypeID == objectType.ObjectTypeID
					   && f.ShowInEdit && f.ControlName.HasValue 
					   && f.ControlName.Value == (int)FormControl.Modal
					   && f.MM_ObjectProperty.LowerBound == 1
					   orderby f.SeqNo
					   select f;
			if (modalcols.Count() > 0)
			{
				string idCol = objectType.MM_ObjectProperties.Single(o => o.IsPrimaryKey).SysName;
				res.AppendLine("<asp:Panel ID=\"pModalCols\" runat=\"server\">");
			}

			foreach (MM_FormField p in modalcols)
			{
				res.AppendLine("<%=Layout.GroupTitleBegin()%>");
				res.Append(p.MM_ObjectProperty.Title);
				res.AppendLine("<%=Layout.GroupTitleEnd()%>");
				res.AppendLine("Необходимо ввести первую запись перечня \"" + p.MM_ObjectProperty.Title + "\"");

				res.AppendLine(EditForm(p.MM_ObjectProperty.RefObjectType, width, p.MM_ObjectProperty.RefObjectType.SysName));
			}
			if (modalcols.Count() > 0)
				res.AppendLine("</asp:Panel>");


			return res.ToString();
		}


		public static string Edit(MM_ObjectType objectType)
		{
			string idCol = objectType.MM_ObjectProperties.Single(o => o.IsPrimaryKey).SysName;
			List<MM_FormField> cols = (from f in AppMM.DataContext.MM_FormFields
					   where f.MM_ObjectProperty.ObjectTypeID == objectType.BaseObjectTypeID
					   && f.ShowInEdit
					   orderby f.SeqNo
					   select f).ToList();
			cols.AddRange((from f in AppMM.DataContext.MM_FormFields
					   where f.MM_ObjectProperty.ObjectTypeID == objectType.ObjectTypeID
					   && f.ShowInEdit
					   orderby f.SeqNo
					   select f).ToList());

			var modalcols = from f in AppMM.DataContext.MM_FormFields
							where f.MM_ObjectProperty.ObjectTypeID == objectType.ObjectTypeID
							&& f.ShowInEdit && f.ControlName.HasValue
							&& f.ControlName.Value == (int)FormControl.Modal
							&& f.MM_ObjectProperty.LowerBound == 1
							orderby f.SeqNo
							select f;

			var groups = from g in objectType.MM_FormFieldGroups orderby g.SeqNo select g;
			var selectobjectgroups = groups.Where(o => o.SelectObjectPropertyID.HasValue || !String.IsNullOrEmpty(o.SelectObjectClass));
			var nongroupcols = cols.Where(o => !o.FormFieldGroupID.HasValue);

			ControlCode c = null;
			StringBuilder res = new StringBuilder();

			if (cols.Count() == 0)
			{
				return "ERROR: Нет ни одного поля, отображаемого в представлении Edit";
			}

			#region form
			res.AppendLine(EditForm(objectType, "700px"));
			res.AppendLine("");
			foreach (MM_FormField p in cols)
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
			foreach (MM_FormFieldGroup g in selectobjectgroups)
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
			res.AppendLine("\tSetTitle(\"" + objectType.Title + "\");");
			res.AppendLine("\tlMsg.Text = \"\";");
			res.AppendLine("");
			foreach (MM_FormField p in cols)
			{
				c = ControlCode.Create(p);
				if (c != null)
				{
					string s = c.ViewOnLoad();
					if (!String.IsNullOrEmpty(s))
						res.AppendLine("\t" + s);
				}
			}


			if (modalcols.Count() > 0)
				res.AppendLine("\tif (ViewData." + idCol + " > 0) pModalCols.Visible = false;");

			res.AppendLine("");
			res.AppendLine("\tif (!IsPostBack)");
			res.AppendLine("\t{");

			foreach (MM_FormField p in cols)
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

			foreach (MM_FormField p in cols)
			{
				c = ControlCode.Create(p);
				if (c != null) res.AppendLine("\t\t" + c.Load("ViewData"));
			}
			foreach (MM_FormFieldGroup g in selectobjectgroups)
			{
				FieldGroupSelectObject fgc = new FieldGroupSelectObject(g);
				res.AppendLine("\t\t" + fgc.Load("ViewData"));
			}

			res.AppendLine("\t}");
			res.AppendLine("");
			foreach (MM_FormFieldGroup g in selectobjectgroups)
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
			int reqCnt = cols.Count(o => o.MM_ObjectProperty.LowerBound > 0);
			if (reqCnt > 0)
				res.AppendLine("\tstring requiredMsg = \"\";");

			foreach (MM_FormField p in nongroupcols.Where(o => o.MM_ObjectProperty.LowerBound > 0))
			{
				c = ControlCode.Create(p);
				if (c != null)
				{
					string v = c.ValidateRequired();
					if (!String.IsNullOrEmpty(v)) res.AppendLine("\t" + v);
				}
			}
			foreach (MM_FormFieldGroup g in groups)
				foreach (MM_FormField p in g.MM_FormFields.OrderBy(o => o.SeqNo).Where(o => o.MM_ObjectProperty.LowerBound > 0))
				{
					c = ControlCode.Create(p);
					if (c != null)
					{
						string v = c.ValidateRequired();
						if (!String.IsNullOrEmpty(v)) res.AppendLine("\t" + v);
					}
				}

			foreach (MM_FormFieldGroup g in selectobjectgroups)
			{
				FieldGroupSelectObject fgc = new FieldGroupSelectObject(g);
				res.AppendLine("\t" + fgc.ValidateRequired());
			}

			foreach (MM_FormField p in modalcols)
			{
				var ctablecols = from op in AppMM.DataContext.MM_ObjectProperties
								 where op.ObjectTypeID == p.MM_ObjectProperty.RefObjectTypeID
								 orderby op.SeqNo
								 select op;

				foreach (MM_ObjectProperty pp in ctablecols.Where(o => o.MM_FormField.ShowInEdit && o.LowerBound > 0))
				{
					c = ControlCode.Create(pp.MM_FormField, p.MM_ObjectProperty.SysName);
					if (c != null)
					{
						string v = c.ValidateRequired();
						if (!String.IsNullOrEmpty(v)) res.AppendLine("\t" + v);
					}
				}
			}

			if (reqCnt > 0)
			{
				res.AppendLine("\tif (!String.IsNullOrEmpty(requiredMsg))");
				res.AppendLine("\t{");
				res.AppendLine("\t\tlMsg.Text = \"<div class='savewarning'>Необходимо задать значения следующих полей:<ul>\" + requiredMsg + \"</ul></div>\";");
				res.AppendLine("\t\treturn;");
				res.AppendLine("\t}");
			}
			#endregion

			foreach (MM_FormField p in cols)
			{
				c = ControlCode.Create(p);
				if (c != null)
					res.AppendLine("\t" + c.Save("ViewData"));
			}

			if (modalcols.Count() > 0)
			{
				res.AppendLine("\tif (ViewData." + idCol + " == 0)");
				res.AppendLine("\t{");
			}

			foreach (MM_FormField p in modalcols)
			{
				var ctablecols = from op in AppMM.DataContext.MM_ObjectProperties
								 where op.ObjectTypeID == p.MM_ObjectProperty.RefObjectTypeID
								 orderby op.SeqNo
								 select op.MM_FormField;

				res.AppendLine("\t\t" + p.MM_ObjectProperty.RefObjectType.SysName + " " + p.MM_ObjectProperty.SysName + "obj = new " + p.MM_ObjectProperty.RefObjectType.SysName + "();");
				res.AppendLine("");
				foreach (MM_FormField pp in ctablecols.Where(o => o.ShowInEdit))
				{
					c = ControlCode.Create(pp, p.MM_ObjectProperty.RefObjectType.SysName);
					if (c != null)
						res.AppendLine("\t\t" + c.Save(p.MM_ObjectProperty.SysName + "obj"));
				}
				
				res.AppendLine("");
				var ctablecalccols = from f in AppMM.DataContext.MM_FormFields
							   where f.MM_ObjectProperty.ObjectTypeID == p.MM_ObjectProperty.RefObjectTypeID
							   && f.ValueFunction != null && f.ValueFunction != ""
							   orderby f.SeqNo
							   select f;

				foreach (MM_FormField pp in ctablecalccols)
				{
					res.AppendLine("\t\t" + p.MM_ObjectProperty.SysName + "obj." + pp.MM_ObjectProperty.SysName +
						" = (new Func<" + objectType.SysName + ", " +
						CodeGenHelper.GetCSharpType(pp.MM_ObjectProperty.TypeCode, pp.MM_ObjectProperty.UpperBound) +
						">(" + pp.ValueFunction + ")).Invoke(" + p.MM_ObjectProperty.SysName + "obj);");
				}

				res.AppendLine("\t\tViewData." + p.MM_ObjectProperty.SysName + ".Add(" + p.MM_ObjectProperty.SysName + "obj);");
			}
			if (modalcols.Count() > 0)
			{
				res.AppendLine("\t}");
			}

			foreach (MM_FormFieldGroup g in selectobjectgroups)
			{
				FieldGroupSelectObject fgc = new FieldGroupSelectObject(g);
				res.AppendLine(fgc.Save("ViewData"));
			}

			res.AppendLine("");

			var calccols = from f in AppMM.DataContext.MM_FormFields
						   where f.MM_ObjectProperty.ObjectTypeID == objectType.BaseObjectTypeID
						   && f.ValueFunction != null && f.ValueFunction != ""
						   orderby f.MM_ObjectProperty.SeqNo
						   select f;
			calccols.Union(from f in AppMM.DataContext.MM_FormFields
						   where f.MM_ObjectProperty.ObjectTypeID == objectType.ObjectTypeID
						   && f.ValueFunction != null && f.ValueFunction != ""
						   orderby f.MM_ObjectProperty.SeqNo
						   select f);

			foreach (MM_FormField p in calccols)
			{
				if (p.ValueFunctionExecType)
				{
					res.AppendLine("\tif (ViewData." + idCol + " == 0)");
					res.Append("\t");
				}
				res.AppendLine("\tViewData." + p.MM_ObjectProperty.SysName +
					" = (new Func<" + objectType.SysName + ", " +
					CodeGenHelper.GetCSharpType(p.MM_ObjectProperty.TypeCode, p.MM_ObjectProperty.LowerBound) +
					">(" + p.ValueFunction + ")).Invoke(ViewData);");
			}

			/*#region data validate
			if (objectType.MM_DataValidations.Count > 0)
			{
				res.AppendLine("");
				res.AppendLine("\tstring dataValidateMsg = \"\";");
			}
			foreach (MM_DataValidation dv in objectType.MM_DataValidations)
			{
				string expr = String.Format(dv.Expression, "ViewData");
				expr = expr.Replace("{{", "{").Replace("}}", "}");
				
				res.AppendLine("\tif(!(" + expr + ")) dataValidateMsg += \"<li>" + dv.Message + "</li>\";");
			}
			if (objectType.MM_DataValidations.Count > 0)
			{
				res.AppendLine("\tif (!String.IsNullOrEmpty(dataValidateMsg))");
				res.AppendLine("\t{");
				
				res.AppendLine("\t\tlMsg.Text = \"<div class='savewarning'><ul>\" + dataValidateMsg + \"</ul></div>\";");
				res.AppendLine("\t\treturn;");
				res.AppendLine("\t}");
			}
			#endregion*/


			res.AppendLine("");

			res.AppendLine("\t" + objectType.SysName + "Controller c = new " + objectType.SysName + "Controller();");
			res.AppendLine("\tc.Update(ViewData);");
			foreach (MM_FormField p in cols)
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

			foreach (MM_FormFieldGroup g in selectobjectgroups)
			{
				FieldGroupSelectObject fgc = new FieldGroupSelectObject(g);
				res.AppendLine(fgc.Events());
			}
			foreach (MM_FormField p in cols)
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
