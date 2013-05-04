using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Metamodel.Model;
using System.Text;

namespace Nephrite.Metamodel
{
	public static class CodeGenHelper
	{
		public static string GetCSharpType(string typeCode, int lowerBounds)
		{
			switch (typeCode)
			{
				case ObjectPropertyType.String:
				case ObjectPropertyType.Guid:
				case ObjectPropertyType.Object:
					return "string";
				case ObjectPropertyType.DateTime:
				case ObjectPropertyType.Date:
					return lowerBounds == 0 ? "DateTime?" : "DateTime";
				case ObjectPropertyType.Number:
					return lowerBounds == 0 ? "int?" : "int";
				case ObjectPropertyType.Code:
					return "string";
				case ObjectPropertyType.Boolean:
					return lowerBounds == 0 ? "bool?" : "bool";
				case ObjectPropertyType.Decimal:
					return lowerBounds == 0 ? "decimal?" : "decimal";
				default:
					return "";
			}
		}

		public static string GetCellValue(MM_ObjectType t, MM_ObjectProperty p, MM_ObjectProperty idProp, string linkCol)
		{
			return GetCellValue(t, p, idProp, linkCol, false, "");
		}
		public static string GetCellValue(MM_ObjectType t, MM_ObjectProperty p, MM_ObjectProperty idProp, string linkCol, bool internalLink, string modal)
		{
			switch (p.TypeCode)
			{
				case ObjectPropertyType.String:
				case ObjectPropertyType.Guid:
					
					if (p.SysName == linkCol && idProp != null)
					{
						if (internalLink)
						{
							return "Html.InternalLink(" + modal + ".RenderRun(o." + idProp.SysName + "), enc(o." + p.SysName + "), true)";
						}
						else
						{
							MM_Method m = t.MM_Methods.SingleOrDefault(o => o.IsDefault);
							return "Html.ActionLink<" + t.SysName + "Controller>(c => c." + (m != null ? m.SysName : "") + "(o." + idProp.SysName + ", Query.CreateReturnUrl()), enc(o." + p.SysName + "))";
						}
					}
					else
						return "enc(o." + p.SysName + ")";
					
				case ObjectPropertyType.DateTime:
					return "o." + p.SysName + ".DateTimeToString()";
				case ObjectPropertyType.Date:
					return "o." + p.SysName + ".DateToString()";
				case ObjectPropertyType.Number:
				case ObjectPropertyType.Decimal:
					MM_FormFieldAttribute dtf = p.MM_FormField.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Format");
					if (p.SysName == linkCol && idProp != null)
					{
						if (internalLink)
						{
							return "Html.InternalLink(" + modal + ".RenderRun(o." + idProp.SysName + "), enc(o." + p.SysName + ".ToString(" + (dtf != null ? ("\"" + dtf.Value + "\"") : "") + ")), true)";
						}
						else
						{
							MM_Method m = t.MM_Methods.SingleOrDefault(o => o.IsDefault);
							return "Html.ActionLink<" + t.SysName + "Controller>(c => c." + (m != null ? m.SysName : "") +
								"(o." + idProp.SysName + ", Query.CreateReturnUrl()), enc(o." + p.SysName + ".ToString(" + (dtf != null ? ("\"" + dtf.Value + "\"") : "") + ")))";
						}
					}
					else
					{
						if (p.LowerBound == 0)
							return "o." + p.SysName + ".HasValue ? o." + p.SysName + ".Value.ToString(" + (dtf != null ? ("\"" + dtf.Value + "\"") : "") + ") : \"\"";
						else
							return "o." + p.SysName + ".ToString(" + (dtf != null ? ("\"" + dtf.Value + "\"") : "") + ")";
					}
				case ObjectPropertyType.Code:
					return p.MM_Codifier.SysName + ".Title(o." + p.SysName + ")";
				case ObjectPropertyType.Boolean:
					return "o." + p.SysName + ".Icon()";
				case ObjectPropertyType.Object:
					if (p.LowerBound > 0)
					{
						if (p.SysName == linkCol && idProp != null)
						{
							if (internalLink)
							{
								return "Html.InternalLink(" + modal + ".RenderRun(o." + idProp.SysName + "), enc(o." + p.SysName + ".Title), true)";
							}
							else
							{
								MM_Method m = t.MM_Methods.SingleOrDefault(o => o.IsDefault);
								return "Html.ActionLink<" + t.SysName + "Controller>(c => c." + (m != null ? m.SysName : "") + "(o." + idProp.SysName + ", Query.CreateReturnUrl()), enc(o." + p.SysName + ".Title))";
							}
						}
						else
							return "enc(o." + p.SysName + ".Title)";
					}
					else
						return "enc(o." + p.SysName + @" == null ? """" : o." + p.SysName + ".Title)";
				case ObjectPropertyType.File:
					return @"String.Format(""<a href='/file.ashx?oid={0}'>{1}</a>"", o." + p.SysName + ".FileID.ToString(), o." + p.SysName + ".Title)";
				case ObjectPropertyType.ZoneDateTime:
					return "o." + p.SysName + "ZoneDateTime.ToString()";
				default:
					return "";
			}
		}

		public static string GetFilterCode(string t, MM_ObjectProperty p)
		{
			string fname = p.Title;
			switch (p.TypeCode)
			{
				case ObjectPropertyType.String:
				case ObjectPropertyType.Guid:
					return "filter.AddFieldString<" + t + ">(\"" + fname + "\", o => o." + p.SysName + ");";
				case ObjectPropertyType.DateTime:
					return "filter.AddFieldDate<" + t + ">(\"" + fname + "\", o => o." + p.SysName + ", false);";
				case ObjectPropertyType.Number:
				case ObjectPropertyType.Decimal:
					return "filter.AddFieldNumber<" + t + ">(\"" + fname + "\", o => o." + p.SysName + ");";
				case ObjectPropertyType.Code:
					return "filter.AddFieldDDL<" + t + ">(\"" + fname + "\", o => o." + p.SysName + ", " + p.MM_Codifier.SysName + ".ToList(), \"Title\", \"Code\");";
				case ObjectPropertyType.Boolean:
					return "filter.AddFieldBoolean<" + t + ">(\"" + fname + "\", o => o." + p.SysName + ");";
				case ObjectPropertyType.Object:
					return "filter.AddFieldString<" + t + ">(\"" + fname + "\", o => o." + p.SysName + ".Title);";
				default:
					return "";
			}
		}

        public static string GetFilterCode(MM_ObjectType t, MM_ObjectProperty p)
        {
            return GetFilterCode(t.SysName, p);
        }

		public static string GetViewFieldValue(MM_ObjectProperty p)
		{
			switch (p.TypeCode)
			{
				case ObjectPropertyType.String:
				case ObjectPropertyType.Guid:
					return "<%=enc(ViewData." + p.SysName + ") %>";
				case ObjectPropertyType.Date:
					return "<%=ViewData." + p.SysName + ".DateToString() %>";
				case ObjectPropertyType.DateTime:
					return "<%=ViewData." + p.SysName + ".DateTimeToString() %>";
				case ObjectPropertyType.Number:
				case ObjectPropertyType.Decimal:
					MM_FormFieldAttribute dtf = p.MM_FormField.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Format");
					if (dtf != null)
						if (p.LowerBound == 0)
							return "<%=ViewData." + p.SysName + ".HasValue ? ViewData." + p.SysName + ".Value.ToString(\"" + dtf.Value + "\") : \"\" %>";
						else
							return "<%=ViewData." + p.SysName + ".ToString(\"" + dtf.Value + "\") %>";
					else
						return "<%=ViewData." + p.SysName + ".ToString() %>";
				case ObjectPropertyType.Code:
					return "<%=" + p.MM_Codifier.SysName + ".Title(ViewData." + p.SysName + ") %>";
				case ObjectPropertyType.Boolean:
					return "<%=ViewData." + p.SysName + ".Icon() %>";
				case ObjectPropertyType.Object:
					if (p.LowerBound == 1 && p.UpperBound == 1)
						return "<%=enc(ViewData." + p.SysName + ".Title) %>";
					else if (p.LowerBound == 0 && p.UpperBound == 1)
						return "<%=ViewData." + p.SysName + " == null ? \"\" : enc(ViewData." + p.SysName + ".Title) %>";
					else
						return "";
				case ObjectPropertyType.ZoneDateTime:
					return "<%=ViewData." + p.SysName + "ZoneDateTime.ToString() %>";
				default:
					return "";
			}
		}

		public static string GetStringValue(MM_ObjectProperty p, string var)
		{
			switch (p.TypeCode)
			{
				case ObjectPropertyType.String:
				case ObjectPropertyType.Guid:
					return var + "." + p.SysName;
				case ObjectPropertyType.Date:
					return var + "." + p.SysName + ".DateToString()";
				case ObjectPropertyType.DateTime:
					return var + "." + p.SysName + ".DateTimeToString()";
				case ObjectPropertyType.Number:
				case ObjectPropertyType.Decimal:
					MM_FormFieldAttribute dtf = p.MM_FormField.MM_FormFieldAttributes.SingleOrDefault(o => o.Title == "Format");
					if (dtf != null)
						return var + "." + p.SysName + ".ToString(\"" + dtf.Value + "\")";
					else
						return var + "." + p.SysName + ".ToString()";
				case ObjectPropertyType.Code:
					return p.MM_Codifier.SysName + ".Title(" + var + "." + p.SysName + ") %>";
				case ObjectPropertyType.Boolean:
					return var + "." + p.SysName + ".Icon()";
				case ObjectPropertyType.Object:
					if (p.LowerBound == 1 && p.UpperBound == 1)
						return var + "." + p.SysName + ".Title";
					else if (p.LowerBound == 0 && p.UpperBound == 1)
						return var + "." + p.SysName + " != null ? " + var + "." + p.SysName + ".Title : \"\"";
					else
						return "";
				case ObjectPropertyType.ZoneDateTime:
					return var + "." + p.SysName + "ZoneDateTime.ToString()";
				default:
					return "";
			}
		}

		/*public static string SysNamePlural(string sysName)
		{
			if (sysName.EndsWith("y"))
				return sysName.Substring(0, sysName.Length - 2) + "ies";
			else if (sysName.EndsWith("s"))
				return sysName;
			else
				return sysName + "s";
		}*/
	}



	public abstract class ControlCode
	{
		protected MM_FormField _field { get; set; }
		protected string _prefix { get; set; }
		public ControlCode(MM_FormField field)
		{
			_field = field;
		}
		public ControlCode(MM_FormField field, string prefix)
		{
			_field = field;
			_prefix = prefix;
		}
		public abstract string Control();
		public abstract string SetValue(string value);
		public abstract string Load(string var);
		public abstract string SetDefaultOrEmpty();
		public abstract string Save(string var);
		public virtual string SaveDefault(string var)
		{
			return var + "." + _field.MM_ObjectProperty.SysName + " = " + _field.DefaultValue + ";";
		}
		public virtual string Init() { return ""; }
		public virtual string ViewOnLoad() { return ""; }
		public virtual string ViewOnInit() { return ""; }
		public virtual string RootLayout() { return ""; }
		public virtual string ValidateRequired() { return ""; }
		public virtual string ValidateValue() { return ""; }
		public virtual string AfterSubmit() { return ""; }
		public virtual string Events() { return ""; } 
		public static ControlCode Create(MM_FormField field)
		{
			return Create(field, String.Empty);
		}
		public static ControlCode Create(MM_FormField field, string prefix)
		{
			if (field.ControlName.HasValue)
			{
				switch ((FormControl)field.ControlName)
				{
					case FormControl.TextBox:
					case FormControl.FormattedTextBox:
						return new TextBoxCode(field, prefix);
					case FormControl.TextArea:
						return new TextAreaCode(field, prefix);
					case FormControl.RichEditor:
						return new TinyMCECode(field, prefix);
					case FormControl.Label:
					case FormControl.FormattedLabel:
						return new LabelCode(field, prefix);
					case FormControl.StringArray:
						return null;
					case FormControl.DateLists:
						return new DateListsCode(field, prefix);
					case FormControl.Calendar:
						return new JSCalendarCode(field, prefix);
					case FormControl.File:
						return new FileUploadCode(field, prefix);
					case FormControl.Image:
						return new ImageUploadCode(field, prefix);
					case FormControl.CheckBox:
						return new CheckBoxCode(field, prefix);
					case FormControl.MMCodifier:
						return new DDLCodifierCode(field, prefix);
					case FormControl.NestedForm:
						return null;
					case FormControl.DropDownList:
						return new DDLCode(field, prefix);
					case FormControl.DropDownListConst:
						return null;
					case FormControl.FileArray:
						return null;
					case FormControl.SelectObjectHierarchic:
						return new SelectObjectHierarchicCode(field, prefix);
					case FormControl.MultiSelectObjectHierarchic:
						return new MultiSelectObjectHierarchicCode(field, prefix);
					case FormControl.FileSimple:
						return new FileUploadSimpleCode(field, prefix);
					case FormControl.TextObject:
						return new TextObjectCode(field, prefix);
					case FormControl.ZoneCalendar:
						return new ZoneCalendarCode(field, prefix);
					case FormControl.MultiSelectObjectHierarchicText:
						return new MultiSelectObjectHierarchicTextCode(field, prefix);
					case FormControl.DropDownListText:
						return new DDLTextCode(field, prefix);
					case FormControl.SingleObject:
						return new SingleObjectCode(field, prefix);
					case FormControl.MultiObject:
						return new MultiObjectCode(field, prefix);
					default:
						return null;
				}
			}
			else
			{
				switch (field.MM_ObjectProperty.TypeCode)
				{
					case ObjectPropertyType.String:
					case ObjectPropertyType.Number:
					case ObjectPropertyType.Decimal:
						return new TextBoxCode(field, prefix);
					case ObjectPropertyType.Boolean:
						return new CheckBoxCode(field, prefix);
					case ObjectPropertyType.Date:
						return new JSCalendarCode(field, prefix);
					case ObjectPropertyType.DateTime:
						return new JSCalendarCode(field, prefix);
					case ObjectPropertyType.Object:
						if (field.MM_ObjectProperty.UpperBound == 1)
							return new DDLCode(field, prefix);
						else
							return null;
					case ObjectPropertyType.Code:
						return new DDLCodifierCode(field, prefix);
					case ObjectPropertyType.File:
						return new FileUploadCode(field, prefix);
					//case ObjectPropertyType.Image:
					//	return new ImageUploadCode(field, prefix);
					default:
						return null;

				}
			}
		}
	}

	


}
