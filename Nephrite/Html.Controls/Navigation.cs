using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nephrite.AccessControl;

namespace Nephrite.Html.Controls
{
	//public class Navigation
	//{
	//	IAccessControl _accessControl;
	//	List<SimpleLink> _items = new List<SimpleLink>();
	//	public IEnumerable<SimpleLink> Items { get { return _items; } } 

	//	public Navigation(IAccessControl accessControl)
	//	{
	//		_accessControl = accessControl;
	//	}

	//	public SimpleLink AddItem(string title, string href, string securableObjectKey = "", string image = "")
	//	{
	//		return AddItem(title, href, securableObjectKey, false, image);
	//	}

	//	public SimpleLink AddItem(string title, string href, string securableObjectKey, bool defaultAccess, string image = "")
	//	{
	//		if (!securableObjectKey.IsEmpty())
	//			if (!_accessControl.Check(securableObjectKey, defaultAccess))
	//				return null;

	//		var mi = new SimpleLink { Title = title, Href = href, Image = image, SecurableObjectKey = securableObjectKey };
	//		_items.Add(mi);
	//		return mi;
	//	}
	//}
}
