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
		public List<FormElement> Content { get; set; }
	}

	public class EditPropertyField : FormElement
	{
		public MetaClassifier Type { get; set; }
		public bool IsRequired { get; set; }
		public string DefaultValue { get; set; }
	}

	public class EditAttributeField : EditPropertyField
	{
		public string Format { get; set; }
	}

	public class EditReferenceField : EditPropertyField
	{
		public string ClassName { get; set; }
		public string DataValueField { get; set; }
		public string DataTextField { get; set; }
		public string Filter { get; set; }
		public string SearchExpression { get; set; }
	}




}