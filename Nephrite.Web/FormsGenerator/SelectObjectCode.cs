using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Nephrite.Meta;
using Nephrite.Meta.Forms;

namespace Nephrite.Web.FormsGenerator
{
	internal class MultiObjectCode : ReferenceControlCode
	{
		string controlID = "";

		public MultiObjectCode(EditReferenceField field)
			: base(field)
		{
			controlID = "mo" + field.Name;
		}
		public MultiObjectCode(EditReferenceField field, string prefix)
			: base(field, prefix)
		{
			controlID = "mo" + prefix + field.Name;
		}

		public override string Control()
		{

			return @"<nw:MultiObject runat=""server"" ID=""" + controlID + @""" DataTextField=""" + (_field.DataTextField.IsEmpty() ? "Title" : _field.DataTextField) + @""" DataValueField=""" + _field.DataValueField + @""" />";
		}

		public override string ViewOnLoad()
		{
			StringBuilder res = new StringBuilder();
			res.AppendLine(controlID + ".AllObjects = App.DataContext." + _field.ClassName + (!_field.Filter.IsEmpty() ? (".Where(" + _field.Filter + ")") : ".Where(o => !o.IsDeleted)"));
			if (!String.IsNullOrEmpty(_field.SearchExpression))
				res.AppendLine(controlID + ".SearchExpression = " + _field.SearchExpression + ";");
			res.AppendLine(controlID + ".Type = typeof(" + _field.ClassName + ");");
			return res.ToString();
		}

		public override string SetValue(string value)
		{
			return controlID + ".Text = " + value + ";";
		}

		public override string Load(string var)
		{
			return controlID + ".LoadObjects<" + _field.ClassName + ">(" + var + "." + _field.Name + ");";
		}

		public override string Save(string var)
		{
			StringBuilder res = new StringBuilder();
			res.AppendLine(var + "." + _field.Name + ".Sync(" + controlID + ".GetSelected<" + _field.ClassName + ">())");
			return res.ToString();
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
			return "if (" + controlID + ".Count() == 0) requiredMsg += \"<li>" +
				(_field.ParentElement != null ? (_field.ParentElement.Caption + " \\\\ ") : "") +
				_field.Caption + "</li>\";";
		}
	}

	internal class SingleObjectCode : ReferenceControlCode
	{
		string controlID = "";
		string keySuffix = "";

		public SingleObjectCode(EditReferenceField field)
			: base(field)
		{
			controlID = "so" + field.Name;
			keySuffix = ((field.Type as MetaClass).Key.Type as IMetaIdentifierType).ColumnSuffix;
		}
		public SingleObjectCode(EditReferenceField field, string prefix)
			: base(field, prefix)
		{
			controlID = "so" + prefix + field.Name;
			keySuffix = ((field.Type as MetaClass).Key.Type as IMetaIdentifierType).ColumnSuffix;
		}

		public override string Control()
		{
			return @"<nw:SingleObject runat=""server"" ID=""" + controlID + @""" DataTextField=""" + (_field.DataTextField.IsEmpty() ? "Title" : _field.DataTextField) + @""" DataValueField=""" + _field.DataValueField + @""" />";
		}

		public override string ViewOnLoad()
		{
			StringBuilder res = new StringBuilder();
			res.AppendLine(controlID + ".AllObjects = App.DataContext." + _field.ClassName + (!_field.Filter.IsEmpty() ? (".Where(" + _field.Filter + ")") : ".Where(o => !o.IsDeleted)"));
			if (!String.IsNullOrEmpty(_field.SearchExpression))
				res.AppendLine(controlID + ".SearchExpression = " + _field.SearchExpression + ";");
			res.AppendLine(controlID + ".Type = typeof(" + _field.ClassName + ");");
			return res.ToString();
		}

		public override string SetValue(string value)
		{
			return controlID + ".Text = " + value + ";";
		}

		public override string Load(string var)
		{
			return controlID + ".SetObject(" + var + "." + _field.Name + ");";
		}

		public override string Save(string var)
		{
			return var + "." + _field.Name + keySuffix + " = " + controlID + ".Object" + keySuffix + (!_field.IsRequired ? ".Value" : "") + ";";
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
			return "if (" + controlID + ".Object" + keySuffix + " == null) requiredMsg += \"<li>" +
				(_field.ParentElement != null ? (_field.ParentElement.Caption + " \\\\ ") : "") +
				_field.Caption + "</li>\";";
		}
	}
	
}