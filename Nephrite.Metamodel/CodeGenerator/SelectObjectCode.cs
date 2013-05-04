using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Nephrite.Metamodel.Model;

namespace Nephrite.Metamodel
{
	internal class TextObjectCode : ControlCode
	{
		string controlID = "";
		MM_FormFieldAttribute ffa_class;
		MM_FormFieldAttribute ffa_dtf;
		MM_FormFieldAttribute ffa_filter;
		MM_FormFieldAttribute ffa_se;
		string table = "";
		string dvf = "";

		public TextObjectCode(MM_FormField field)
			: base(field)
		{
			controlID = "tb" + field.MM_ObjectProperty.SysName;
			ffa_class = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Class");
			ffa_dtf = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "DataTextField");
			ffa_filter = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Filter");
			ffa_se = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "SearchExpression");
			table = ffa_class.Value;
			if (!String.IsNullOrEmpty(ffa_filter.Value))
				table += ".Where(" + ffa_filter.Value + ")";
			dvf = AppMM.DataContext.MM_ObjectTypes.Single(o => o.SysName == ffa_class.Value).MM_ObjectProperties.Single(o => o.IsPrimaryKey).SysName;
		}
		public TextObjectCode(MM_FormField field, string prefix)
			: base(field, prefix)
		{
			controlID = "tb" + prefix + field.MM_ObjectProperty.SysName;
			ffa_class = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Class");
			ffa_dtf = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "DataTextField");
			ffa_filter = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Filter");
			ffa_se = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "SearchExpression");
			table = ffa_class.Value;
			if (!String.IsNullOrEmpty(ffa_filter.Value))
				table += ".Where(" + ffa_filter.Value + ")";
			dvf = AppMM.DataContext.MM_ObjectTypes.Single(o => o.SysName == ffa_class.Value).MM_ObjectProperties.Single(o => o.IsPrimaryKey).SysName;
		}

		public override string Control()
		{
            string clearCode = "";
            if (_field.MM_ObjectProperty.LowerBound == 0)
                clearCode = @"
                <td>&nbsp;</td>
                <td><%=Html.InternalImageLink(""document.getElementById('"" + " + controlID + @".ClientID + ""').value = ''; document.getElementById('"" + " + controlID + @"ID.ClientID + ""').value = '';"", ""Очистить"", ""delete.gif"")%></td>";

            return @"<table width=""100%"" cellpadding=""0"" cellspacing=""0"">
    <tr>
    <td width=""100%"">
        <asp:TextBox runat=""server"" ID=""" + controlID + @""" Width=""100%"" />
    </td>
    <td>&nbsp;</td>
    <td><%=HtmlHelperBase.Instance.InternalImageLink(select" + controlID + @".RenderRun(), ""Выбрать из списка"", ""list.gif"")%></td>" + clearCode + @"
    </tr>
</table>";
		}

		public override string RootLayout()
		{
			return @"<nw:SelectObjectHierarchic ID=""select" + controlID + @""" runat=""server"" Title=""Выбрать объект"" DataTextField=""" + ffa_dtf.Value + @""" PageSize=""20"" OnSelect=""OnSelect_" + controlID + @""" />
<script type=""text/javascript"">
    function OnSelect_" + controlID + @"(title, id) {
        document.getElementById('<%=" + controlID + @".ClientID %>').value = title;
    }
</script>";
		}

		public override string ViewOnLoad()
		{
			string sort = AppMM.DataContext.MM_ObjectTypes.Single(o => o.SysName == ffa_class.Value).DefaultOrderBy;
			if (!sort.ToLower().StartsWith("orderby")) sort = "OrderBy(" + sort + ")";
			StringBuilder res = new StringBuilder();
            res.AppendLine("select" + controlID + ".AllObjects = " + AppMM.DBName() + ".App.DataContext." + table + "." + sort + ";");
			if (!String.IsNullOrEmpty(ffa_se.Value))
				res.AppendLine("select" + controlID + ".SearchExpression = " + ffa_se.Value + ";");
			res.AppendLine("select" + controlID + ".Type = typeof(" + ffa_class.Value + ");");
			return res.ToString();
		}

		public override string SetValue(string value)
		{
			return controlID + ".Text = " + value + ";";
		}

		public override string Load(string var)
		{
			return controlID + ".Text = " + var + "." + _field.MM_ObjectProperty.SysName + ";";
		}

		public override string Save(string var)
		{
			return var + "." + _field.MM_ObjectProperty.SysName + " = " + controlID + ".Text;";
		}

		public override string SetDefaultOrEmpty()
		{
			if (!String.IsNullOrEmpty(_field.DefaultValue))
				return controlID + ".Text = " + _field.DefaultValue + ";";
			else
				return controlID + @".Text = """";";
		}

		public override string ValidateRequired()
		{
			return "if (String.IsNullOrEmpty(" + controlID + ".Text)) requiredMsg += \"<li>" +
				(_field.FormFieldGroupID.HasValue ? (_field.MM_FormFieldGroup.Title + " \\\\ ") : "") +
				_field.MM_ObjectProperty.Title + "</li>\";";
		}
	}

	internal class SelectObjectHierarchicCode : ControlCode
	{
		string controlID = "";
		//MM_FormFieldAttribute ffa_class;
		MM_ObjectType ot = null;
		MM_FormFieldAttribute ffa_dtf;
		MM_FormFieldAttribute ffa_filter;
		MM_FormFieldAttribute ffa_se;
		string table = "";
		string dvf = "";

		public SelectObjectHierarchicCode(MM_FormField field)
			: base(field)
		{
			controlID = "tb" + field.MM_ObjectProperty.SysName;
			//ffa_class = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Class");
			ffa_dtf = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "DataTextField");
			ffa_filter = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Filter");
			ffa_se = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "SearchExpression");
			MM_ObjectType ot = _field.MM_ObjectProperty.RefObjectType;
			table = ot.SysName;
			if (!String.IsNullOrEmpty(ffa_filter.Value))
				table += ".Where(" + ffa_filter.Value + ")";
			dvf = ot.MM_ObjectProperties.Single(o => o.IsPrimaryKey).SysName;
		}
		public SelectObjectHierarchicCode(MM_FormField field, string prefix)
			: base(field, prefix)
		{
			controlID = "tb" + prefix + field.MM_ObjectProperty.SysName;
			//ffa_class = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Class");
			ffa_dtf = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "DataTextField");
			ffa_filter = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Filter");
			ffa_se = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "SearchExpression");
			ot = _field.MM_ObjectProperty.RefObjectType;
			table = ot.SysName;
			if (!String.IsNullOrEmpty(ffa_filter.Value))
				table += ".Where(" + ffa_filter.Value + ")";
			dvf = ot.MM_ObjectProperties.Single(o => o.IsPrimaryKey).SysName;
		}

		public override string Control()
		{
            string clearCode = "";
            if (_field.MM_ObjectProperty.LowerBound == 0)
                clearCode = @"
                <td>&nbsp;</td>
                <td><%=HtmlHelperBase.Instance.InternalImageLink(""document.getElementById('"" + " + controlID + @".ClientID + ""').value = ''; document.getElementById('"" + " + controlID + @"ID.ClientID + ""').value = '';"", ""Очистить"", ""delete.gif"")%></td>";

            return @"<table width=""100%"" cellpadding=""0"" cellspacing=""0"">
    <tr>
    <td width=""100%"">
        <asp:TextBox runat=""server"" ID=""" + controlID + @""" Width=""100%"" ReadOnly=""True"" />
		<asp:HiddenField runat=""server"" ID=""" + controlID + @"ID"" />
    </td>
    <td>&nbsp;</td>
    <td><%=Html.InternalImageLink(select" + controlID + @".RenderRun(), ""Выбрать из списка"", ""list.gif"")%></td>" + clearCode + @"
    </tr>
</table>";
		}

		public override string RootLayout()
		{
			return @"<nw:SelectObjectHierarchic ID=""select" + controlID + @""" runat=""server"" Title=""Выбрать объект"" DataTextField=""" + ffa_dtf.Value + @""" PageSize=""20"" OnSelect=""OnSelect_" + controlID + @""" />
<script type=""text/javascript"">
    function OnSelect_" + controlID + @"(title, id) {
        document.getElementById('<%=" + controlID + @".ClientID %>').value = title;
		document.getElementById('<%=" + controlID + @"ID.ClientID %>').value = id;
    }
</script>";
		}

		public override string ViewOnLoad()
		{
			string sort = ot.DefaultOrderBy;
			if (!sort.ToLower().StartsWith("orderby")) sort = "OrderBy(" + sort + ")";
			StringBuilder res = new StringBuilder();
            res.AppendLine("select" + controlID + ".AllObjects = App.DataContext." + ClassName(ot) + "." + sort + ";");
			if (!String.IsNullOrEmpty(ffa_se.Value))
				res.AppendLine("select" + controlID + ".SearchExpression = " + ffa_se.Value + ";");
			res.AppendLine("select" + controlID + ".Type = typeof(" + ot.SysName + ");");
			return res.ToString();
		}

		public override string SetValue(string value)
		{
			return controlID + ".Text = " + value + ";";
		}

        string ClassName(MM_ObjectType ot)
        {
            if (ot.MM_ObjectProperties.Any(o => o.IsMultilingual))
                return "V_" + table + ".Where(o => o.LanguageCode == Nephrite.Metamodel.AppMM.CurrentLanguage.LanguageCode)";
            return ot.SysName;
        }

		public override string Load(string var)
		{
			StringBuilder res = new StringBuilder();
			res.AppendLine("if (" + var + "." + _field.MM_ObjectProperty.SysName + "ID > 0)");
			res.AppendLine("{");
			res.AppendLine("\t" + controlID + "ID.Value = " + var + "." + _field.MM_ObjectProperty.SysName + "ID.ToString();");
			res.AppendLine("\t" + ot.SysName + " _" + controlID + " = App.DataContext." + ot.SysName + ".SingleOrDefault(o => o." + dvf + " == " + var + "." + _field.MM_ObjectProperty.SysName + "ID);");
			res.AppendLine("\t" + controlID + ".Text = (_" + controlID + " == null ? \"\" : _" + controlID + ".Title);");
			res.AppendLine("}");
			res.AppendLine("else");
			res.AppendLine("{");
			res.AppendLine("\t" + controlID + "ID.Value = \"\";");
			res.AppendLine("\t" + controlID + ".Text = \"\";");
			res.AppendLine("}");
			return res.ToString();
		}

		public override string Save(string var)
		{
			if (_field.MM_ObjectProperty.LowerBound == 1)
				return var + "." + _field.MM_ObjectProperty.SysName + "ID = " + controlID + "ID.Value.ToInt32(0);";
			else
				return var + "." + _field.MM_ObjectProperty.SysName + "ID = " + controlID + "ID.Value.ToInt32();";
		}

		public override string SetDefaultOrEmpty()
		{
			if (!String.IsNullOrEmpty(_field.DefaultValue))
				return controlID + "ID.Value = " + _field.DefaultValue + ";";
			else
			{
				StringBuilder res = new StringBuilder();
				res.AppendLine(controlID + @"ID.Value = """";");
				res.AppendLine(controlID + @".Text = """";");

				return res.ToString();
			}
		}

		public override string ValidateRequired()
		{
			return "if (String.IsNullOrEmpty(" + controlID + "ID.Value)) requiredMsg += \"<li>" +
				(_field.FormFieldGroupID.HasValue ? (_field.MM_FormFieldGroup.Title + " \\\\ ") : "") +
				_field.MM_ObjectProperty.Title + "</li>\";";
		}
	}


	internal class SingleObjectCode : ControlCode
	{
		string controlID = "";
		//MM_FormFieldAttribute ffa_class;
		MM_ObjectType ot = null;
		MM_FormFieldAttribute ffa_dtf;
		MM_FormFieldAttribute ffa_filter;
		MM_FormFieldAttribute ffa_se;
		string table = "";
		string dvf = "";

		public SingleObjectCode(MM_FormField field)
			: base(field)
		{
			controlID = "so" + field.MM_ObjectProperty.SysName;
			//ffa_class = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Class");
			ffa_dtf = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "DataTextField");
			ffa_filter = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Filter");
			ffa_se = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "SearchExpression");
			MM_ObjectType ot = _field.MM_ObjectProperty.RefObjectType;
			table = ot.SysName;
			if (!String.IsNullOrEmpty(ffa_filter.Value))
				table += ".Where(" + ffa_filter.Value + ")";
			dvf = ot.MM_ObjectProperties.Single(o => o.IsPrimaryKey).SysName;
		}
		public SingleObjectCode(MM_FormField field, string prefix)
			: base(field, prefix)
		{
			controlID = "so" + prefix + field.MM_ObjectProperty.SysName;
			//ffa_class = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Class");
			ffa_dtf = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "DataTextField");
			ffa_filter = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Filter");
			ffa_se = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "SearchExpression");
			ot = _field.MM_ObjectProperty.RefObjectType;
			table = ot.SysName;
			if (!String.IsNullOrEmpty(ffa_filter.Value))
				table += ".Where(" + ffa_filter.Value + ")";
			dvf = ot.MM_ObjectProperties.Single(o => o.IsPrimaryKey).SysName;
		}

		public override string Control()
		{

			return @"<nw:SingleObject runat=""server"" ID=""" + controlID + @""" />";
		}

		public override string ViewOnLoad()
		{
			string sort = ot.DefaultOrderBy ?? "o => o.Title";
			if (!sort.ToLower().StartsWith("orderby")) sort = "OrderBy(" + sort + ")";
			StringBuilder res = new StringBuilder();
			res.AppendLine(controlID + ".AllObjects = App.DataContext." + ClassName(ot) + "." + sort + ";");
			if (!String.IsNullOrEmpty(ffa_se.Value))
				res.AppendLine(controlID + ".SearchExpression = " + ffa_se.Value + ";");
			res.AppendLine("\t" + controlID + ".Type = typeof(" + ot.SysName + ");");
			return res.ToString();
		}

		public override string SetValue(string value)
		{
			return controlID + ".Text = " + value + ";";
		}

		string ClassName(MM_ObjectType ot)
		{
			if (ot.MM_ObjectProperties.Any(o => o.IsMultilingual))
				return "V_" + table + ".Where(o => o.LanguageCode == Nephrite.Metamodel.AppMM.CurrentLanguage.LanguageCode)";
			return ot.SysName;
		}

		public override string Load(string var)
		{
			return controlID + ".SetObject(" + var + "." + _field.MM_ObjectProperty.SysName + ");";
		}

		public override string Save(string var)
		{
			return var + "." + _field.MM_ObjectProperty.SysName + "ID = " + controlID + ".ObjectID" + (_field.MM_ObjectProperty.LowerBound == 1 ? ".Value" : "") + ";";
		}

		public override string SetDefaultOrEmpty()
		{
			if (!String.IsNullOrEmpty(_field.DefaultValue))
				return "";
			else
			{
				return controlID + ".Clear()";
			}
		}

		public override string ValidateRequired()
		{
			return "if (" + controlID + ".ObjectID == null) requiredMsg += \"<li>" +
				(_field.FormFieldGroupID.HasValue ? (_field.MM_FormFieldGroup.Title + " \\\\ ") : "") +
				_field.MM_ObjectProperty.Title + "</li>\";";
		}
	}


	internal class FieldGroupSelectObject
	{
		MM_FormFieldGroup g = null;
		string controlID = "";
		string className = "";
		string _prefix = "";

		public FieldGroupSelectObject(MM_FormFieldGroup group)
		{
			g = group;
			if (g.SelectObjectPropertyID.HasValue)
			{
				controlID = g.MM_ObjectProperty.SysName;
				className = g.MM_ObjectProperty.RefObjectType.SysName;
			}
			else if (!String.IsNullOrEmpty(g.SelectObjectPrefix))
			{
				controlID = g.SelectObjectPrefix;
				className = g.SelectObjectClass;
			}
			else
			{
				controlID = g.SelectObjectClass;
				className = g.SelectObjectClass;
			}
		}
		public FieldGroupSelectObject(MM_FormFieldGroup group, string prefix)
		{
			g = group;
			_prefix = prefix;
			if (g.SelectObjectPropertyID.HasValue)
			{
				controlID = prefix + g.MM_ObjectProperty.SysName;
				className = g.MM_ObjectProperty.RefObjectType.SysName;
			}
			else if (!String.IsNullOrEmpty(g.SelectObjectPrefix))
			{
				controlID = prefix + g.SelectObjectPrefix;
				className = g.SelectObjectClass;
			}
			else
			{
				controlID = prefix + g.SelectObjectClass;
				className = g.SelectObjectClass;
			}
		}

		public string ControlID
		{
			get
			{
				return controlID;
			}
		}

        string ClassName(MM_ObjectType ot)
        {
            if (ot.MM_ObjectProperties.Any(o => o.IsMultilingual))
                return "V_" + ot.SysName + ".Where(o => o.LanguageCode == Nephrite.Metamodel.AppMM.CurrentLanguage.LanguageCode)";
            return ot.SysName;
        }

		public string RootLayout()
		{
			return @"<nw:SelectObjectHierarchic ID=""select" + controlID + @""" runat=""server"" Title=""Выбрать объект"" DataTextField=""" + g.SelectObjectDataTextField + @""" PageSize=""20"" OnSelect=""OnSelect_" + controlID + @""" OnSelected=""select" + controlID + @"_OKClick"" />
<script type=""text/javascript"">
    function OnSelect_" + controlID + @"(title, id) {
		document.getElementById('<%=selected" + controlID + @".ClientID %>').value = id;
    }
</script>";
		}

		public string Control()
		{
			StringBuilder res = new StringBuilder();
			res.AppendLine("<%=Html.InternalImageLink(select" + controlID + @".RenderRun(), ""Выбрать из списка"", ""list.gif"") %>");
			res.AppendLine(@"<asp:HiddenField ID=""selected" + controlID + @""" runat=""server"" />");
			return res.ToString();
		}

		public string ViewOnLoad()
		{
            var ot = AppMM.DataContext.MM_ObjectTypes.Single(o => o.SysName == className);
			string sort = ot.DefaultOrderBy;
			string table = ClassName(ot);
			if (!String.IsNullOrEmpty(g.SelectObjectFilter))
				table += ".Where(" + g.SelectObjectFilter + ")";
			if (!sort.ToLower().StartsWith("orderby")) sort = "OrderBy(" + sort + ")";
			StringBuilder res = new StringBuilder();
			res.AppendLine("select" + controlID + ".AllObjects = App.DataContext." + table + "." + sort + ";");
			if (!String.IsNullOrEmpty(g.SelectObjectSearchExpression))
				res.AppendLine("select" + controlID + ".SearchExpression = " + g.SelectObjectSearchExpression + ";");
			res.AppendLine("select" + controlID + ".Type = typeof(" + className + ");");
			return res.ToString();
		}

		public string Events()
		{
			StringBuilder res = new StringBuilder();
			res.AppendLine("");
			string vf = AppMM.DataContext.MM_ObjectTypes.Single(o => o.SysName == className).MM_ObjectProperties.Single(o => o.IsPrimaryKey).SysName;

			res.AppendLine("protected void select" + controlID + "_OKClick(object sender, SelectObjectHierarchicEventArgs e)");
			res.AppendLine("{");
			res.AppendLine("\t" + className + " obj = App.DataContext." + className + ".Single(o => o." + vf + " == e.ObjectID);");
			Set(res, "\t");
			res.AppendLine("}");
			return res.ToString();
		}

		public string Save(string var)
		{
			StringBuilder res = new StringBuilder();
			if (g.SelectObjectPropertyID.HasValue)
			{
				res.AppendLine("if (!String.IsNullOrEmpty(selected" + controlID + ".Value))");
				res.AppendLine("\t" + var + "." + g.MM_ObjectProperty.SysName + "ID = selected" + controlID + ".Value.ToInt32(0);");
				res.AppendLine("else");
				res.AppendLine("\t" + var + "." + g.MM_ObjectProperty.SysName + " = null;");

			}
			return res.ToString();
		}

		public string Load(string var)
		{
			StringBuilder res = new StringBuilder();
			if (g.SelectObjectPropertyID.HasValue)
			{
				string table = className;
				string idCol = g.MM_ObjectType.MM_ObjectProperties.Single(o => o.IsPrimaryKey).SysName;
				if (!String.IsNullOrEmpty(g.SelectObjectFilter))
					table += ".Where(" + g.SelectObjectFilter + ")";

				res.AppendLine("selected" + controlID + ".Value = " + var + "." + g.MM_ObjectProperty.SysName + "ID.ToString();");
				res.AppendLine("");
				res.AppendLine("if (ViewData." + idCol + " == 0 && App.DataContext." + table + ".Count() == 1)");
				res.AppendLine("{");
				res.AppendLine("\t" + className + " obj = App.DataContext." + table + ".First();");
				Set(res, "\t");
				res.AppendLine("}");
				return res.ToString();
			}
			else
				return "";

		}

		public string ValidateRequired()
		{
			if (g.SelectObjectPropertyID.HasValue && g.MM_ObjectProperty.LowerBound != 0)
				return "if (String.IsNullOrEmpty(selected" + controlID + ".Value) || selected" + controlID + ".Value == \"0\") requiredMsg += \"<li>" +
					g.Title + " (необходимо выбрать значение из справочника)</li>\";";
			else
				return "";
		}

		void Set(StringBuilder res, string tabs)
		{
			IEnumerable<MM_FormField> setFields = null;
			if (!String.IsNullOrEmpty(g.SelectObjectPrefix))
				setFields = g.MM_FormFields.Where(o => o.MM_ObjectProperty.SysName.StartsWith(g.SelectObjectPrefix));
			else
				setFields = g.MM_FormFields;

			foreach (MM_FormField f in setFields)
			{
				ControlCode c = ControlCode.Create(f, _prefix);
				if (c != null)
				{
					if (!String.IsNullOrEmpty(g.SelectObjectPrefix))
						res.AppendLine(tabs + c.SetValue("obj." + f.MM_ObjectProperty.SysName.Replace(g.SelectObjectPrefix, "")));
					else
						res.AppendLine(tabs + c.SetValue("obj." + f.MM_ObjectProperty.SysName));
				}
			}

			if (g.MM_ObjectProperty != null)
				res.AppendLine("selected" + controlID + ".Value = obj." + className + "ID.ToString();");
			res.AppendLine(tabs + "up" + controlID + ".Update();");
		}

	}
}
