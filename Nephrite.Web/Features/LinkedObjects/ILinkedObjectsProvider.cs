using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.Web.LinkedObjects
{
	public interface ILinkedObjectsProvider
	{
		IEnumerable<ObjectLinks> GetLinks(LinkType linkType);
	}
}
