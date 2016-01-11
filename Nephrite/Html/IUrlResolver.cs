using System;
using System.Collections.Generic;
using System.Text;

namespace Nephrite.Html
{
	public interface IUrlResolver
	{
		StringBuilder Resolve(IDictionary<string, string> parameters, bool isHashPart = false);
	}
}
