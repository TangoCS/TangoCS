using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Meta;
using Nephrite.Meta.Forms;

namespace Nephrite.Web.FormsGenerator
{
	public abstract class ControlCode
	{
		protected string _prefix { get; set; }

		public abstract string Control();
		public abstract string SetValue(string value);
		public abstract string Load(string var);
		public abstract string SetDefaultOrEmpty();
		public abstract string Save(string var);
		public virtual string SaveDefault(string var) { return ""; }
		public virtual string Init() { return ""; }
		public virtual string ViewOnLoad() { return ""; }
		public virtual string ViewOnInit() { return ""; }
		public virtual string RootLayout() { return ""; }
		public virtual string ValidateRequired() { return ""; }
		public virtual string ValidateValue() { return ""; }
		public virtual string AfterSubmit() { return ""; }
		public virtual string Events() { return ""; }

		public static ControlCode Create(FormElement field)
		{
			return Create(field, String.Empty);
		}
		public static ControlCode Create(FormElement field, string prefix)
		{
			if (field.Control != null)
			{
				switch (field.Control)
				{
					case FormControl.TextBox:
					case FormControl.FormattedTextBox:
						return new TextBoxCode(field, prefix);
					case FormControl.TextArea:
						return new TextAreaCode(field, prefix);
					case FormControl.RichEditor:
						return new TinyMCECode(field, prefix);
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
					case FormControl.Enum:
						return new DDLEnumCode(field, prefix);
					case FormControl.NestedForm:
						return null;
					case FormControl.DropDownList:
						return new DDLCode(field, prefix);
					case FormControl.DropDownListConst:
						return null;
					case FormControl.FileArray:
						return null;
					case FormControl.FileSimple:
						return new FileUploadSimpleCode(field, prefix);
					case FormControl.DropDownListText:
						return new DDLTextCode(field, prefix);
					case FormControl.SingleObject:
						return new SingleObjectCode(field, prefix);
					case FormControl.MultiObject:
						return new MultiObjectCode(field, prefix);
					case FormControl.NumericBox:
						return new NumericBoxCode(field, prefix);
					default:
						return null;
				}
			}
			else
			{
				if (field.Type is MetaGuidType || field.Type is MetaStringType)
					return new TextBoxCode(field, prefix);
				else if (field.Type is MetaDateTimeType || field.Type is MetaDateType)
					return new JSCalendarCode(field, prefix);
				else if (field.Type is IMetaNumericType)
					return new NumericBoxCode(field, prefix);
				else if (field.Type is MetaBooleanType)
					return new CheckBoxCode(field, prefix);
				else if (field.Type is MetaClass)
					if (field.UpperBound == 1)
						return new DDLCode(field, prefix);
					else
						return new MultiObjectCode(field, prefix);
				else if (field.Type is MetaFileType)
					return new FileUploadCode(field, prefix);
				else if (field.Type is MetaEnum)
					return new DDLEnumCode(field, prefix);
				else
					return null;
			}
		}
	}

	public abstract class AttributeControlCode : ControlCode
	{
		protected FormElement _field { get; set; }
		public AttributeControlCode(FormElement field)
		{
			_field = field;
		}
		public AttributeControlCode(FormElement field, string prefix)
		{
			_field = field;
			_prefix = prefix;
		}
		public override string SaveDefault(string var)
		{
			return var + "." + _field.Name + " = " + _field.DefaultValue + ";";
		}
	}

	public abstract class ReferenceControlCode : ControlCode
	{
		protected FormElement _field { get; set; }
		public ReferenceControlCode(FormElement field)
		{
			_field = field;
		}
		public ReferenceControlCode(FormElement field, string prefix)
		{
			_field = field;
			_prefix = prefix;
		}
		public override string SaveDefault(string var)
		{
			return var + "." + _field.Name + " = " + _field.DefaultValue + ";";
		}
	}
}