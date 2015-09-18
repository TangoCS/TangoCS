using System.Collections.Generic;

namespace Nephrite.Html
{
	public interface IUrlResolver
	{
		string Resolve(IDictionary<string, object> parameters);
	}
}
