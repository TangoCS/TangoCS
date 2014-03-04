using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Meta.Forms
{
	public abstract class FormElement
	{
		public string Name { get; set; }
		public string Caption { get; set; }
		public Container ParentElement { get; set; }
	}

	public class Container : FormElement
	{
		public MetaClass MetaClass { get; set; }
		public string ViewDataClass { get; set; }
		public List<FormElement> Content { get; set; }
	}

	public class PropertyField : FormElement
	{
		public MetaClassifier Type { get; set; }
		public bool IsRequired { get; set; }
		public string DefaultValue { get; set; }
	}

	public class AttributeField : PropertyField
	{
		public string Format { get; set; }
	}

	public class ReferenceField : PropertyField
	{
		public string ClassName { get; set; }
		public string DataValueField { get; set; }
		public string DataTextField { get; set; }
		public string Filter { get; set; }
		public string SearchExpression { get; set; }
	}
}