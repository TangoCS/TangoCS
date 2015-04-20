using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Nephrite.Meta;

namespace Nephrite.Web.FormsGenerator
{
	internal class MultiObjectCode : ReferenceControlCode
	{
		string controlID = "";

		public MultiObjectCode(FormElement field)
			: base(field)
		{
			controlID = "mo" + field.Name;
		}
		public MultiObjectCode(FormElement field, string prefix)
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
			res.AppendLine(controlID + ".AllObjects = App.DataContext." + _field.Type.Name + (!_field.Filter.IsEmpty() ? (".Where(" + _field.Filter + ")") : ".Where(o => !o.IsDeleted)"));
			if (!String.IsNullOrEmpty(_field.SearchExpression))
				res.AppendLine(controlID + ".SearchExpression = " + _field.SearchExpression + ";");
			res.AppendLine(controlID + ".Type = typeof(" + _field.Type.Name + ");");
			return res.ToString();
		}

		public override string SetValue(string value)
		{
			return controlID + ".Text = " + value + ";";
		}

		public override string Load(string var)
		{
			return controlID + ".LoadObjects<" + _field.Type.Name + ">(" + var + "." + _field.Name + ");";
		}

		public override string Save(string var)
		{
			StringBuilder res = new StringBuilder();
			res.AppendLine(var + "." + _field.Name + ".Sync(" + controlID + ".GetSelected<" + _field.Type.Name + ">())");
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

		public SingleObjectCode(FormElement field)
			: base(field)
		{
			controlID = "so" + field.Name;
			keySuffix = ((field.Type as MetaClass).Key.Type as IMetaIdentifierType).ColumnSuffix;
		}
		public SingleObjectCode(FormElement field, string prefix)
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
			res.AppendLine(controlID + ".AllObjects = App.DataContext." + _field.Type.Name + (!_field.Filter.IsEmpty() ? (".Where(" + _field.Filter + ")") : ".Where(o => !o.IsDeleted)"));
			if (!String.IsNullOrEmpty(_field.SearchExpression))
				res.AppendLine(controlID + ".SearchExpression = " + _field.SearchExpression + ";");
			res.AppendLine(controlID + ".Type = typeof(" + _field.Type.Name + ");");
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

	internal class FieldGroupSelectObject
	{
		FormElement g = null;
		string controlID = "";
		string className = "";
		string _prefix = "";

		public FieldGroupSelectObject(FormElement group)
		{
			g = group;

			controlID = g.Name;
			className = g.Type.Name;

		}
		public FieldGroupSelectObject(FormElement group, string prefix)
		{
			g = group;
			_prefix = prefix;

			controlID = prefix + g.Name;
			className = g.Type.Name;
		}

		public string ControlID
		{
			get
			{
				return controlID;
			}
		}

		string ClassName(MetaClass ot)
		{
			if (ot.IsMultilingual)
				return "V_" + ot.Name + ".Where(o => o.LanguageCode == Language.Current.Code)";
			return ot.Name;
		}

		public string RootLayout()
		{
			return @"<nw:SelectObjectHierarchic ID=""select" + controlID + @""" runat=""server"" Title=""Выбрать объект"" DataTextField=""" + g.DataTextField + @""" PageSize=""20"" OnSelect=""OnSelect_" + controlID + @""" OnSelected=""select" + controlID + @"_OKClick"" />
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
			var ot = g.Type as MetaClass;
			string sort = "";
			string table = ClassName(ot);
			if (!String.IsNullOrEmpty(g.Filter))
				table += ".Where(" + g.Filter + ")";
			if (!sort.ToLower().StartsWith("orderby")) sort = "OrderBy(" + sort + ")";
			StringBuilder res = new StringBuilder();
			res.AppendLine("select" + controlID + ".AllObjects = App.DataContext." + table + "." + sort + ";");
			if (!String.IsNullOrEmpty(g.SearchExpression))
				res.AppendLine("select" + controlID + ".SearchExpression = " + g.SearchExpression + ";");
			res.AppendLine("select" + controlID + ".Type = typeof(" + className + ");");
			return res.ToString();
		}

		public string Events()
		{
			StringBuilder res = new StringBuilder();
			res.AppendLine("");
			string vf = (g.Type as MetaClass).Key.Name;

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
			if (g.MapWithProperty)
			{
				res.AppendLine("if (!String.IsNullOrEmpty(selected" + controlID + ".Value))");
				res.AppendLine("\t" + var + "." + g.Name + "ID = selected" + controlID + ".Value.ToInt32(0);");
				res.AppendLine("else");
				res.AppendLine("\t" + var + "." + g.Name + " = null;");

			}
			return res.ToString();
		}

		public string Load(string var)
		{
			StringBuilder res = new StringBuilder();
			if (g.MapWithProperty)
			{
				string table = className;
				string idCol = (g.Type as MetaClass).Key.Name;
				if (!String.IsNullOrEmpty(g.Filter))
					table += ".Where(" + g.Filter + ")";

				res.AppendLine("selected" + controlID + ".Value = " + var + "." + g.Name + "ID.ToString();");
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
			if (g.MapWithProperty && g.IsRequired)
				return "if (String.IsNullOrEmpty(selected" + controlID + ".Value) || selected" + controlID + ".Value == \"0\") requiredMsg += \"<li>" +
					g.Caption + " (необходимо выбрать значение из справочника)</li>\";";
			else
				return "";
		}

		void Set(StringBuilder res, string tabs)
		{
			IEnumerable<FormElement> setFields = g.Content;

			foreach (var f in setFields)
			{
				ControlCode c = ControlCode.Create(f, _prefix);
				if (c != null)
				{
					res.AppendLine(tabs + c.SetValue("obj." + f.Name));
				}
			}

			if (g.MapWithProperty)
				res.AppendLine("selected" + controlID + ".Value = obj." + className + "ID.ToString();");
			res.AppendLine(tabs + "up" + controlID + ".Update();");
		}

	}
	
}