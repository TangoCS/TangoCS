using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Nephrite.Metamodel.Model;

namespace Nephrite.Metamodel
{
	internal class TextBoxCode : ControlCode
	{
		string controlID = "";
		public TextBoxCode(MM_FormField field)
			: base(field)
		{
			controlID = "tb" + field.MM_ObjectProperty.SysName;
		}
		public TextBoxCode(MM_FormField field, string prefix)
			: base(field, prefix)
		{
			controlID = "tb" + prefix + field.MM_ObjectProperty.SysName;
		}
		public override string Control()
		{
			string s = @"<asp:TextBox ID=""" + controlID + @""" runat=""server"" Width=""100%"" />";
			/*if (_field.MM_ObjectProperty.LowerBound == 1)
				s += "\r\n" + @"<asp:RequiredFieldValidator runat=""server"" ID=""rfv" +
					controlID + @""" ControlToValidate=""" +
					controlID + @""" ErrorMessage=""Поле обязательно для заполнения!"" Display=""Dynamic"" />";*/
			return s;

		}
		public override string SetValue(string value)
		{
			return controlID + ".Text = " + value + ";";
		}
		public override string Load(string var)
		{
			if (_field.MM_ObjectProperty.TypeCode == ObjectPropertyType.Number ||
				_field.MM_ObjectProperty.TypeCode == ObjectPropertyType.Decimal)
			{
				MM_FormFieldAttribute dtf = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Format");
				if (dtf != null)
					if (_field.MM_ObjectProperty.LowerBound == 1)
						return SetValue(var + "." + _field.MM_ObjectProperty.SysName + ".ToString(\"" + dtf.Value + "\")");
					else
					{
						return "if (" + var + "." + _field.MM_ObjectProperty.SysName + ".HasValue) " +
							SetValue(var + "." + _field.MM_ObjectProperty.SysName + ".Value.ToString(\"" + dtf.Value + "\")");
					}
				else
					return SetValue(var + "." + _field.MM_ObjectProperty.SysName + ".ToString()");
			}
			else
				return SetValue(var + "." + _field.MM_ObjectProperty.SysName);
		}

		public override string Save(string var)
		{
			string s = "";
			if (_field.MM_ObjectProperty.TypeCode == ObjectPropertyType.Number)
				s += var + "." + _field.MM_ObjectProperty.SysName + " = " + controlID + ".Text.ToInt32(0);";
			else if (_field.MM_ObjectProperty.TypeCode == ObjectPropertyType.Decimal)
				s += var + "." + _field.MM_ObjectProperty.SysName + " = " + controlID + ".Text.ToDecimal(0);";
			else
				s += var + "." + _field.MM_ObjectProperty.SysName + " = " + controlID + ".Text;";

			return s;
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

		public override string Init()
		{
			return base.Init();
		}


	}

	internal class TextAreaCode : ControlCode
	{
		string controlID = "";
		public TextAreaCode(MM_FormField field)
			: base(field)
		{
			controlID = "tb" + field.MM_ObjectProperty.SysName;
		}
		public TextAreaCode(MM_FormField field, string prefix)
			: base(field, prefix)
		{
			controlID = "tb" + prefix + field.MM_ObjectProperty.SysName;
		}
		public override string Control()
		{
			string s = @"<asp:TextBox ID=""" + controlID + @""" runat=""server"" Width=""100%"" TextMode=""MultiLine"" />";
			/*if (_field.MM_ObjectProperty.LowerBound == 1)
				s += "\r\n" + @"<asp:RequiredFieldValidator runat=""server"" ID=""rfv" +
					controlID + @""" ControlToValidate=""" +
					controlID + @""" ErrorMessage=""Поле обязательно для заполнения!"" Display=""Dynamic"" />";*/
			return s;
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

	internal class CheckBoxCode : ControlCode
	{
		string controlID = "";
		public CheckBoxCode(MM_FormField field)
			: base(field)
		{
			controlID = "cb" + field.MM_ObjectProperty.SysName;
		}
		public CheckBoxCode(MM_FormField field, string prefix)
			: base(field, prefix)
		{
			controlID = "cb" + prefix + field.MM_ObjectProperty.SysName;
		}
		public override string Control()
		{
			return @"<asp:CheckBox ID=""" + controlID + @""" runat=""server"" />";
		}
		public override string SetValue(string value)
		{
			return controlID + ".Checked = " + value + ";";
		}
		public override string Load(string var)
		{
			return controlID + ".Checked = " + var + "." + _field.MM_ObjectProperty.SysName + ";";
		}

		public override string Save(string var)
		{
			return var + "." + _field.MM_ObjectProperty.SysName + " = " + controlID + ".Checked;";
		}

		public override string SetDefaultOrEmpty()
		{
			if (!String.IsNullOrEmpty(_field.DefaultValue))
				return controlID + ".Checked = " + _field.DefaultValue + ";";
			else
				return controlID + @".Checked = false;";
		}


	}

	internal class TinyMCECode : ControlCode
	{
		string controlID = "";
		public TinyMCECode(MM_FormField field)
			: base(field)
		{
			controlID = "tb" + field.MM_ObjectProperty.SysName;
		}
		public TinyMCECode(MM_FormField field, string prefix)
			: base(field, prefix)
		{
			controlID = "tb" + prefix + field.MM_ObjectProperty.SysName;
		}
		public override string Control()
		{
			MM_FormFieldAttribute t = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "TagFile");
			return @"<nw:TinyMCEEditor ID=""" + controlID + @""" runat=""server"" Width=""100%""" + (t != null ? ("Tags=\"" + t.Value + "\"") : "") +" />";
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

	internal class JSCalendarCode : ControlCode
	{
		string controlID = "";
		public JSCalendarCode(MM_FormField field)
			: base(field)
		{
			controlID = "cal" + field.MM_ObjectProperty.SysName;
		}
		public JSCalendarCode(MM_FormField field, string prefix)
			: base(field, prefix)
		{
			controlID = "cal" + prefix + field.MM_ObjectProperty.SysName;
		}
		public override string Control()
		{
			if (_field.MM_ObjectProperty.TypeCode == ObjectPropertyType.Date)
				return @"<nw:JSCalendar ID=""" + controlID + @""" runat=""server"" />";
			else
				return @"<nw:JSCalendar ID=""" + controlID + @""" runat=""server"" ShowTime=""true"" />";
		}
		public override string SetValue(string value)
		{
			return controlID + ".Date = " + value + ";";
		}
		public override string Load(string var)
		{
			return controlID + ".Date = " + var + "." + _field.MM_ObjectProperty.SysName + ";";
		}

		public override string Save(string var)
		{
			string s = "if (" + controlID + ".Date.HasValue)\r\n";
			s += "\t\t" + var + "." + _field.MM_ObjectProperty.SysName + " = " + controlID + ".Date.Value;";
			return s;
		}

		public override string SetDefaultOrEmpty()
		{
			if (!String.IsNullOrEmpty(_field.DefaultValue))
				return controlID + ".Date = " + _field.DefaultValue + ";";
			else
				return controlID + @".Date = null;";
		}

		public override string ValidateRequired()
		{
			return "if (!" + controlID + ".Date.HasValue) requiredMsg += \"<li>" +
				(_field.FormFieldGroupID.HasValue ? (_field.MM_FormFieldGroup.Title + " \\\\ ") : "") +
				_field.MM_ObjectProperty.Title + "</li>\";";
		}
	}

	internal class DDLCode : ControlCode
	{
		string controlID = "";
		public DDLCode(MM_FormField field)
			: base(field)
		{
			controlID = "ddl" + field.MM_ObjectProperty.SysName;
		}
		public DDLCode(MM_FormField field, string prefix)
			: base(field, prefix)
		{
			controlID = "ddl" + prefix + field.MM_ObjectProperty.SysName;
		}
		public override string Control()
		{
			string idCol = _field.MM_ObjectProperty.RefObjectType.MM_ObjectProperties.Single(o => o.IsPrimaryKey).SysName;
			string fv = "";
			if (_field.MM_ObjectProperty.IsReferenceToVersion)
				fv = idCol.Replace("ID", "VersionID");
			else
				fv = idCol;

			MM_FormFieldAttribute dtf = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "DataTextField");

			string s = @"<asp:DropDownList ID=""" + controlID + @""" runat=""server"" Width=""100%"" DataTextField=""" + (dtf == null ? "Title" : dtf.Value) + @""" DataValueField=""" + fv + @"""/>";
			/*if (_field.MM_ObjectProperty.LowerBound == 1)
				s += "\r\n" + @"<asp:RequiredFieldValidator runat=""server"" ID=""rfv" +
					controlID + @""" ControlToValidate=""" +
					controlID + @""" ErrorMessage=""Поле обязательно для заполнения!"" Display=""Dynamic"" />";*/
			return s;
		}

		public override string SetValue(string value)
		{
			return controlID + ".SetValue( " + value + ");";
		}

		public override string Load(string var)
		{
			return controlID + ".SetValue(" + var + "." + _field.MM_ObjectProperty.SysName + "ID);";
		}

		public override string Save(string var)
		{
			string s = "";
			if (_field.MM_ObjectProperty.LowerBound == 0)
				s += var + "." + _field.MM_ObjectProperty.SysName + "ID = " + controlID + ".GetValue();";
			else
				s += var + "." + _field.MM_ObjectProperty.SysName + "ID = " + controlID + ".GetValue(0);";

			return s;
		}

		public override string Init()
		{
			string s = controlID + ".DataBindOnce(App.DataContext.";
			string req = _field.MM_ObjectProperty.LowerBound == 1 ? "" : ", true";
			string filter = !String.IsNullOrEmpty(_field.MM_ObjectProperty.ValueFilter) ?
				(".Where(" + _field.MM_ObjectProperty.ValueFilter + ")") : "";

			if (_field.MM_ObjectProperty.IsReferenceToVersion)
			{
				return s + "HST_" + _field.MM_ObjectProperty.RefObjectType.SysName + ".Where(o => o.IsCurrentVersion)" + filter + req + ");";
			}
			else
			{
				return s + _field.MM_ObjectProperty.RefObjectType.SysName + filter + req + ");";
			}
		}

		public override string SetDefaultOrEmpty()
		{
			if (!String.IsNullOrEmpty(_field.DefaultValue))
				return controlID + ".SetValue(" + _field.DefaultValue + ");";
			else
				return controlID + @".SetValue("""");";
		}

		public override string ValidateRequired()
		{
			return "if (String.IsNullOrEmpty(" + controlID + ".SelectedValue)) requiredMsg += \"<li>" +
				(_field.FormFieldGroupID.HasValue ? (_field.MM_FormFieldGroup.Title + " \\\\ ") : "") +
				_field.MM_ObjectProperty.Title + "</li>\";";
		}
	}

	internal class DDLTextCode : ControlCode
	{
		string controlID = "";

		MM_FormFieldAttribute dtf = null;
		MM_FormFieldAttribute dvf = null;
		MM_FormFieldAttribute cf = null;
		MM_FormFieldAttribute ff = null;

		public DDLTextCode(MM_FormField field)
			: base(field)
		{
			controlID = "ddl" + field.MM_ObjectProperty.SysName;
			dtf = field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "DataTextField");
			dvf = field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "DataValueField");
			cf = field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Class");
			ff = field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Filter");
		}
		public DDLTextCode(MM_FormField field, string prefix)
			: base(field, prefix)
		{
			controlID = "ddl" + prefix + field.MM_ObjectProperty.SysName;
			dtf = field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "DataTextField");
			dvf = field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "DataValueField");
			cf = field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Class");
			ff = field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Filter");
		}
		public override string Control()
		{
			
			string s = @"<asp:DropDownList ID=""" + controlID + @""" runat=""server"" Width=""100%"" DataTextField=""" + (dtf == null ? "Title" : dtf.Value) + @""" DataValueField=""" + dvf.Value + @"""/>";
			return s;
		}

		public override string SetValue(string value)
		{
			return controlID + ".SetValue( " + value + ");";
		}

		public override string Load(string var)
		{
			return controlID + ".SetValue(" + var + "." + _field.MM_ObjectProperty.SysName + ");";
		}

		public override string Save(string var)
		{
			string s = "";
			s += var + "." + _field.MM_ObjectProperty.SysName + " = " + controlID + ".SelectedValue;";
			return s;
		}

		public override string Init()
		{
			string s = controlID + ".DataBindOnce(App.DataContext.";
			string req = _field.MM_ObjectProperty.LowerBound == 1 ? "" : ", true";
			string filter = !String.IsNullOrEmpty(ff.Value) ? (".Where(" + ff.Value + ")") : "";

			return s + cf.Value + filter + req + ");";
		}

		public override string SetDefaultOrEmpty()
		{
			if (!String.IsNullOrEmpty(_field.DefaultValue))
				return controlID + ".SetValue(" + _field.DefaultValue + ");";
			else
				return controlID + @".SetValue("""");";
		}

		public override string ValidateRequired()
		{
			return "if (String.IsNullOrEmpty(" + controlID + ".SelectedValue)) requiredMsg += \"<li>" +
				(_field.FormFieldGroupID.HasValue ? (_field.MM_FormFieldGroup.Title + " \\\\ ") : "") +
				_field.MM_ObjectProperty.Title + "</li>\";";
		}
	}


	internal class DDLCodifierCode : ControlCode
	{
		string controlID = "";
		public DDLCodifierCode(MM_FormField field)
			: base(field)
		{
			controlID = "ddl" + field.MM_ObjectProperty.SysName;
		}
		public DDLCodifierCode(MM_FormField field, string prefix)
			: base(field, prefix)
		{
			controlID = "ddl" + prefix + field.MM_ObjectProperty.SysName;
		}
		public override string Control()
		{
			return @"<asp:DropDownList ID=""" + controlID + @""" runat=""server"" Width=""100%"" DataTextField=""Title"" DataValueField=""Code""/>";
		}

		public override string SetValue(string value)
		{
			return controlID + ".SetValue( " + value + ");";
		}

		public override string Load(string var)
		{
			return controlID + ".SetValue(" + var + "." + _field.MM_ObjectProperty.SysName + ");";
		}

		public override string Save(string var)
		{
			string s = "";
			if (_field.MM_ObjectProperty.LowerBound == 0)
			{
				s += "if (!String.IsNullOrEmpty(" + controlID + ".SelectedValue))\r\n\t\t";
			}
			s += var + "." + _field.MM_ObjectProperty.SysName + " = " + controlID + ".SelectedValue[0];";
			if (_field.MM_ObjectProperty.LowerBound == 0)
			{
				s += "\r\n\telse\r\n\t\t" + var + "." + _field.MM_ObjectProperty.SysName + " = null;";
			}
			return s;

			//return var + "." + _field.MM_ObjectProperty.SysName + " = " + controlID + ".SelectedValue[0];";
		}

		public override string Init()
		{
			string s = controlID + ".DataBindOnce(";
			if (_field.MM_ObjectProperty.LowerBound == 1)
				return s + _field.MM_ObjectProperty.MM_Codifier.SysName + ".ToList());";
			else
				return s + _field.MM_ObjectProperty.MM_Codifier.SysName + ".ToList(), true);";
		}

		public override string SetDefaultOrEmpty()
		{
			if (!String.IsNullOrEmpty(_field.DefaultValue))
				return controlID + ".SetValue(" + _field.DefaultValue + ");";
			else
				return controlID + @".SetValue("""");";
		}
	}

	internal class LabelCode : ControlCode
	{
		string controlID = "";
		public LabelCode(MM_FormField field)
			: base(field)
		{
			controlID = "l" + field.MM_ObjectProperty.SysName;
		}
		public LabelCode(MM_FormField field, string prefix)
			: base(field, prefix)
		{
			controlID = "l" + prefix + field.MM_ObjectProperty.SysName;
		}
		public override string Control()
		{
			return @"<asp:Label ID=""" + controlID + @""" runat=""server"" />";
		}

		public override string SetValue(string value)
		{
			return controlID + ".Text = " + value + ";";
		}

		public override string Load(string var)
		{
			string s = controlID + ".Text = ";
			switch (_field.MM_ObjectProperty.TypeCode)
			{
				case ObjectPropertyType.Number:
				case ObjectPropertyType.Decimal:
					MM_FormFieldAttribute dtf = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "DataTextField");
					if (dtf != null)
						s += var + "." + _field.MM_ObjectProperty.SysName + ".ToString(\"" + dtf.Value + "\")";
					else
						s += var + "." + _field.MM_ObjectProperty.SysName + ".ToString()";
					break;
				case ObjectPropertyType.Boolean:
					s += var + "." + _field.MM_ObjectProperty.SysName + ".Icon()";
					break;
				case ObjectPropertyType.DateTime:
					s += var + "." + _field.MM_ObjectProperty.SysName + ".DateTimeToString()";
					break;
				case ObjectPropertyType.Date:
					s += var + "." + _field.MM_ObjectProperty.SysName + ".DateToString()";
					break;
				case ObjectPropertyType.Object:
					if (_field.MM_ObjectProperty.UpperBound == 1)
						s += var + "." + _field.MM_ObjectProperty.SysName + ".Title";
					break;
				case ObjectPropertyType.Code:
					s += _field.MM_ObjectProperty.MM_Codifier.Title + ".Title(" + var + "." + _field.MM_ObjectProperty.SysName + ")";
					break;
				case ObjectPropertyType.String:
					s += var + "." + _field.MM_ObjectProperty.SysName;
					break;
				default:
					break;

			}
			s += ";";
			return s;
		}

		public override string Save(string var)
		{
			return "";
		}

		public override string SetDefaultOrEmpty()
		{
			if (!String.IsNullOrEmpty(_field.DefaultValue))
			{
				MM_FormFieldAttribute dtf = _field.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "DataTextField");
				if (dtf != null)
					return controlID + ".Text = (" + _field.DefaultValue + ").ToString(\"" + dtf.Value + "\");";
				else
					return controlID + ".Text = (" + _field.DefaultValue + ").ToString();";
			}
			else
				return controlID + @".Text = """";";
		}


	}

	internal class DateListsCode : ControlCode
	{
		string controlID = "";
		public DateListsCode(MM_FormField field)
			: base(field)
		{
			controlID = "dl" + field.MM_ObjectProperty.SysName;
		}

		public DateListsCode(MM_FormField field, string prefix)
			: base(field, prefix)
		{
			controlID = "dl" + prefix + field.MM_ObjectProperty.SysName;
		}
		public override string Control()
		{
			return @"<nw:DateLists ID=""" + controlID + @""" runat=""server"" />";
		}

		public override string SetValue(string value)
		{
			return controlID + ".Date = " + value + ";";
		}

		public override string Load(string var)
		{
			return controlID + ".Date = " + var + "." + _field.MM_ObjectProperty.SysName + ";";
		}

		public override string Save(string var)
		{
            if (_field.MM_ObjectProperty.LowerBound == 1)
			    return var + "." + _field.MM_ObjectProperty.SysName + " = " + controlID + ".Date.Value;";
            else
                return var + "." + _field.MM_ObjectProperty.SysName + " = " + controlID + ".Date;";
		}

		public override string SetDefaultOrEmpty()
		{
			if (!String.IsNullOrEmpty(_field.DefaultValue))
				return controlID + ".Date = " + _field.DefaultValue + ";";
			else
				return controlID + @".Date = null;";
		}

		public override string ValidateRequired()
		{
			return "if (!" + controlID + ".Date.HasValue) requiredMsg += \"<li>" +
				(_field.FormFieldGroupID.HasValue ? (_field.MM_FormFieldGroup.Title + " \\\\ ") : "") +
				_field.MM_ObjectProperty.Title + "</li>\";";
		}
	}

	internal class ZoneCalendarCode : ControlCode
	{
		string controlID = "";
		public ZoneCalendarCode(MM_FormField field)
			: base(field)
		{
			controlID = "cal" + field.MM_ObjectProperty.SysName;
		}
		public ZoneCalendarCode(MM_FormField field, string prefix)
			: base(field, prefix)
		{
			controlID = "cal" + prefix + field.MM_ObjectProperty.SysName;
		}
		public override string Control()
		{
			return @"<nw:ZoneCalendar ID=""" + controlID + @""" runat=""server"" />";
		}
		public override string SetValue(string value)
		{
			return controlID + ".ZoneDateTime = " + value + ";";
		}
		public override string Load(string var)
		{
			return controlID + ".ZoneDateTime = " + var + "." + _field.MM_ObjectProperty.SysName + "ZoneDateTime;";
		}

		public override string Save(string var)
		{
			string s = "if (" + controlID + ".ZoneDateTime.HasValue)\r\n";
			s += "\t" + var + "." + _field.MM_ObjectProperty.SysName + "ZoneDateTime = " + controlID + ".ZoneDateTime.Value;";
			return s;
		}

		public override string SetDefaultOrEmpty()
		{
			if (!String.IsNullOrEmpty(_field.DefaultValue))
				return controlID + ".ZoneDateTime = " + _field.DefaultValue + ";";
			else
				return controlID + @".ZoneDateTime = null;";
		}

		public override string ValidateRequired()
		{
			return "if (!" + controlID + ".ZoneDateTime.HasValue) requiredMsg += \"<li>" +
				(_field.FormFieldGroupID.HasValue ? (_field.MM_FormFieldGroup.Title + " \\\\ ") : "") +
				_field.MM_ObjectProperty.Title + "</li>\";";
		}
	}
}
