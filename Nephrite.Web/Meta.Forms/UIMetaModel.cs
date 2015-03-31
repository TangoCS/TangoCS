using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Meta.Forms
{
	public class FormElement
	{
		public string Name { get; set; }
		public string Caption { get; set; }
		public string Description { get; set; }
		public FormElement ParentElement { get; set; }
		public FormControl Control { get; set; }

		//public MetaClass MetaClass { get; set; }
		public bool MapWithProperty { get; set; }
		public string ViewDataClass { get; set; }
		public List<FormElement> Content { get; set; }

		public MetaClassifier Type { get; set; }
		public bool IsRequired { get; set; }
		public int UpperBound { get; set; }
		public string DefaultValue { get; set; }

		public bool IsMultilingual { get; set; }
		public string Format { get; set; }

		//public string ClassName { get; set; }
		public string DataValueField { get; set; }
		public string DataTextField { get; set; }
		public string Filter { get; set; }
		public string SearchExpression { get; set; }
	}

	public enum FormControl
	{
		TextBox = 1,
		TextArea = 2,
		RichEditor = 3,
		Label = 4,
		StringArray = 5,
		DateLists = 6,
		Calendar = 7,
		File = 8,
		Image = 9,
		CheckBox = 10,
		Enum = 11,
		NestedForm = 12,
		DropDownList = 13,
		DropDownListConst = 14,
		FileArray = 15,
		SelectObjectHierarchic = 16,
		FileSimple = 17,
		TextObject = 18,
		FormattedTextBox = 19,
		FormattedLabel = 20,
		ZoneCalendar = 21,
		Modal = 22,
		MultiSelectObjectHierarchic = 23,
		MultiSelectObjectHierarchicText = 24,
		DropDownListText = 25,
		SingleObject = 26,
		MultiObject = 27,
		NumericBox = 28
	}
}