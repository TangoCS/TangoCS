using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
	}

	public abstract class AttributeControlCode : ControlCode
	{
		protected AttributeField _field { get; set; }
		public AttributeControlCode(AttributeField field)
		{
			_field = field;
		}
		public AttributeControlCode(AttributeField field, string prefix)
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
		protected ReferenceField _field { get; set; }
		public ReferenceControlCode(ReferenceField field)
		{
			_field = field;
		}
		public ReferenceControlCode(ReferenceField field, string prefix)
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