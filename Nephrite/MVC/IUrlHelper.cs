using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite.MVC
{
	public interface IUrlHelper
	{
		string GeneratePathAndQueryFromRoute(IDictionary<string, object> values);
		string GeneratePathAndQueryFromRoute(string routeName, IDictionary<string, object> values);
	}
}
