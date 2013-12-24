using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Meta;
using Nephrite.Meta.Forms;

namespace Nephrite.Web.FormsGenerator
{
	internal class TextBoxCode : AttributeControlCode
	{
		string controlID = "";
		public TextBoxCode(EditAttributeField field)
			: base(field)
		{
			controlID = "tb" + field.Name;
		}
		public TextBoxCode(EditAttributeField field, string prefix)
			: base(field, prefix)
		{
			controlID = "tb" + prefix + field.Name;
		}
		public override string Control()
		{
			string s = @"<asp:TextBox ID=""" + controlID + @""" runat=""server"" Width=""100%"" />";
			return s;

		}
		public override string SetValue(string value)
		{
			return controlID + ".Text = " + value + ";";
		}
		public override string Load(string var)
		{
			if (_field.Type is IMetaNumericType)
			{
				if (!_field.Format.IsEmpty())
					if (_field.IsRequired)
						return SetValue(var + "." + _field.Name + ".ToString(\"" + _field.Format + "\")");
					else
					{
						return "if (" + var + "." + _field.Name + ".HasValue) " +
							SetValue(var + "." + _field.Name + ".Value.ToString(\"" + _field.Format + "\")");
					}
				else
					return SetValue(var + "." + _field.Name + ".ToString()");
			}
			else
				return SetValue(var + "." + _field.Name);
		}

		public override string Save(string var)
		{
			string s = "";
			if (_field.Type is MetaIntType)
				s += var + "." + _field.Name + " = " + controlID + ".Text.ToInt32(0);";
			else if (_field.Type is MetaDecimalType)
				s += var + "." + _field.Name + " = " + controlID + ".Text.ToDecimal(0);";
			else
				s += var + "." + _field.Name + " = " + controlID + ".Text;";

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
				(_field.ParentElement != null ? (_field.ParentElement.Caption + " \\\\ ") : "") +
				_field.Caption + "</li>\";";
		}

		public override string Init()
		{
			return base.Init();
		}


	}

	internal class TextAreaCode : AttributeControlCode
	{
		string controlID = "";
		public TextAreaCode(EditAttributeField field)
			: base(field)
		{
			controlID = "tb" + field.Name;
		}
		public TextAreaCode(EditAttributeField field, string prefix)
			: base(field, prefix)
		{
			controlID = "tb" + prefix + field.Name;
		}
		public override string Control()
		{
			string s = @"<asp:TextBox ID=""" + controlID + @""" runat=""server"" Width=""100%"" TextMode=""MultiLine"" />";
			return s;
		}
		public override string SetValue(string value)
		{
			return controlID + ".Text = " + value + ";";
		}
		public override string Load(string var)
		{
			return controlID + ".Text = " + var + "." + _field.Name + ";";
		}

		public override string Save(string var)
		{
			return var + "." + _field.Name + " = " + controlID + ".Text;";
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
				(_field.ParentElement != null ? (_field.ParentElement.Caption + " \\\\ ") : "") +
				_field.Caption + "</li>\";";
		}
	}

	internal class CheckBoxCode : AttributeControlCode
	{
		string controlID = "";
		public CheckBoxCode(EditAttributeField field)
			: base(field)
		{
			controlID = "cb" + field.Name;
		}
		public CheckBoxCode(EditAttributeField field, string prefix)
			: base(field, prefix)
		{
			controlID = "cb" + prefix + field.Name;
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
			return controlID + ".Checked = " + var + "." + _field.Name + ";";
		}

		public override string Save(string var)
		{
			return var + "." + _field.Name + " = " + controlID + ".Checked;";
		}

		public override string SetDefaultOrEmpty()
		{
			if (!String.IsNullOrEmpty(_field.DefaultValue))
				return controlID + ".Checked = " + _field.DefaultValue + ";";
			else
				return controlID + @".Checked = false;";
		}
	}

	internal class TinyMCECode : AttributeControlCode
	{
		string controlID = "";
		public TinyMCECode(EditAttributeField field)
			: base(field)
		{
			controlID = "tb" + field.Name;
		}
		public TinyMCECode(EditAttributeField field, string prefix)
			: base(field, prefix)
		{
			controlID = "tb" + prefix + field.Name;
		}
		public override string Control()
		{
			return @"<nw:TinyMCEEditor ID=""" + controlID + @""" runat=""server"" Width=""100%"" Tags=""*"" />";
		}
		public override string SetValue(string value)
		{
			return controlID + ".Text = " + value + ";";
		}
		public override string Load(string var)
		{
			return controlID + ".Text = " + var + "." + _field.Name + ";";
		}

		public override string Save(string var)
		{
			return var + "." + _field.Name + " = " + controlID + ".Text;";
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
				(_field.ParentElement != null ? (_field.ParentElement.Caption + " \\\\ ") : "") +
				_field.Caption + "</li>\";";
		}
	}

	internal class JSCalendarCode : AttributeControlCode
	{
		string controlID = "";
		public JSCalendarCode(EditAttributeField field)
			: base(field)
		{
			controlID = "cal" + field.Name;
		}
		public JSCalendarCode(EditAttributeField field, string prefix)
			: base(field, prefix)
		{
			controlID = "cal" + prefix + field.Name;
		}
		public override string Control()
		{
			if (_field.Type is MetaDateType)
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
			return controlID + ".Date = " + var + "." + _field.Name + ";";
		}

		public override string Save(string var)
		{
			string s = "if (" + controlID + ".Date.HasValue)\r\n";
			s += "\t\t" + var + "." + _field.Name + " = " + controlID + ".Date.Value;";
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
				(_field.ParentElement != null ? (_field.ParentElement.Caption + " \\\\ ") : "") +
				_field.Caption + "</li>\";";
		}
	}

	internal class DDLCode : ReferenceControlCode
	{
		string controlID = "";
		public DDLCode(EditReferenceField field)
			: base(field)
		{
			controlID = "ddl" + field.Name;
		}
		public DDLCode(EditReferenceField field, string prefix)
			: base(field, prefix)
		{
			controlID = "ddl" + prefix + field.Name;
		}
		public override string Control()
		{
			string s = @"<asp:DropDownList ID=""" + controlID + @""" runat=""server"" Width=""100%"" DataTextField=""" + (_field.DataTextField.IsEmpty() ? "Title" : _field.DataTextField) + @""" DataValueField=""" + _field.DataValueField + @"""/>";
			return s;
		}

		public override string SetValue(string value)
		{
			return controlID + ".SetValue(" + value + ");";
		}

		public override string Load(string var)
		{
			return controlID + ".SetValue(" + var + "." + _field.Name + "ID);";
		}

		public override string Save(string var)
		{
			string s = "";
			if (_field.IsRequired)
				s += var + "." + _field.Name + "ID = " + controlID + ".GetValue(0);";
			else
				s += var + "." + _field.Name + "ID = " + controlID + ".GetValue();";

			return s;
		}

		public override string Init()
		{
			string s = controlID + ".DataBindOnce(App.DataContext.";
			string req = !_field.IsRequired ? "" : ", true";
			string filter = !_field.Filter.IsEmpty() ? (".Where(" + _field.Filter + ")") : ".Where(o => !o.IsDeleted)";
			return s + _field.ClassName + filter + req + ");";
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
				(_field.ParentElement != null ? (_field.ParentElement.Caption + " \\\\ ") : "") +
				_field.Caption + "</li>\";";
		}
	}

	internal class DDLTextCode : ReferenceControlCode
	{
		string controlID = "";
		public DDLTextCode(EditReferenceField field)
			: base(field)
		{
			controlID = "ddl" + field.Name;
		}
		public DDLTextCode(EditReferenceField field, string prefix)
			: base(field, prefix)
		{
			controlID = "ddl" + prefix + field.Name;
		}
		public override string Control()
		{
			string s = @"<asp:DropDownList ID=""" + controlID + @""" runat=""server"" Width=""100%"" DataTextField=""" + (_field.DataTextField.IsEmpty() ? "Title" : _field.DataTextField) + @""" DataValueField=""" + _field.DataValueField + @"""/>";
			return s;
		}

		public override string SetValue(string value)
		{
			return controlID + ".SetValue( " + value + ");";
		}

		public override string Load(string var)
		{
			return controlID + ".SetValue(" + var + "." + _field.Name + ");";
		}

		public override string Save(string var)
		{
			string s = "";
			s += var + "." + _field.Name + " = " + controlID + ".SelectedValue;";
			return s;
		}

		public override string Init()
		{
			string s = controlID + ".DataBindOnce(App.DataContext.";
			string req = !_field.IsRequired ? "" : ", true";
			string filter = !String.IsNullOrEmpty(_field.Filter) ? (".Where(" + _field.Filter + ")") : ".Where(o => !o.IsDeleted)";

			return s + _field.ClassName + filter + req + ");";
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
				(_field.ParentElement != null ? (_field.ParentElement.Caption + " \\\\ ") : "") +
				_field.Caption + "</li>\";";
		}
	}

	internal class DateListsCode : AttributeControlCode
	{
		string controlID = "";
		public DateListsCode(EditAttributeField field)
			: base(field)
		{
			controlID = "dl" + field.Name;
		}

		public DateListsCode(EditAttributeField field, string prefix)
			: base(field, prefix)
		{
			controlID = "dl" + prefix + field.Name;
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
			return controlID + ".Date = " + var + "." + _field.Name + ";";
		}

		public override string Save(string var)
		{
			if (_field.IsRequired)
				return var + "." + _field.Name + " = " + controlID + ".Date;";
			else
				return var + "." + _field.Name + " = " + controlID + ".Date.Value;";
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
				(_field.ParentElement != null ? (_field.ParentElement.Caption + " \\\\ ") : "") +
				_field.Caption + "</li>\";";
		}
	}

	internal class NumericBoxCode : AttributeControlCode
	{
		string controlID = "";
		int decimals = 0;

		public NumericBoxCode(EditAttributeField field)
			: base(field)
		{
			controlID = "nb" + field.Name;
			decimals = field.Type is MetaDecimalType ? (field.Type as MetaDecimalType).Scale : 0;
		}

		public NumericBoxCode(EditAttributeField field, string prefix)
			: base(field, prefix)
		{
			controlID = "tb" + prefix + field.Name;
		}
		public override string Control()
		{
			string s = @"<nw:NumericBox ID=""" + controlID + @""" runat=""server"" Width=""100%"" Decimals=""" + decimals.ToString() + @""" MinValue=""0"" />";
			return s;

		}
		public override string SetValue(string value)
		{
			if (_field.Type is MetaIntType)
				return controlID + ".IntValue = " + value + ";";
			else
				return controlID + ".Value = " + value + ";";
		}

		public override string Load(string var)
		{
			return SetValue(var + "." + _field.Name);
		}

		public override string Save(string var)
		{
			string s = "";
			if (_field.Type is MetaIntType)
				s += var + "." + _field.Name + " = " + controlID + ".IntValue";
			else if (_field.Type is MetaDecimalType)
				s += var + "." + _field.Name + " = " + controlID + ".Value";
			if (!_field.IsRequired)
				s += ".Value";
			s += ";";

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
			return "if (!" + controlID + ".Value.HasValue) requiredMsg += \"<li>" +
				(_field.ParentElement != null ? (_field.ParentElement.Caption + " \\\\ ") : "") +
				_field.Caption + "</li>\";";
		}
	}

}