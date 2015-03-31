using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web
{
	public class ObjectPropertyChange
	{
		public ObjectPropertyChange(string propertyName, string propertyCaption, object oldValue, string oldValueTitle)
		{
			//Property = property;
			PropertyName = propertyName;
			PropertyCaption = propertyCaption;
			OldValue = oldValue;
			OldValueTitle = oldValueTitle;
		}

		public object OldValue { get; private set; }
		public object NewValue { get; set; }
		public string OldValueTitle { get; private set; }
		public string NewValueTitle { get; set; }
		public string PropertyName { get; set; }
		public string PropertyCaption { get; set; }
		//public IMetaProperty Property { get; private set; }
		public bool HideOldValue { get; set; }
	}
}