using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nephrite.IFML;

namespace Tessera.Test
{
	public static class IFML
	{
		public static void Example()
		{
			var masterArea = new ViewContainer();

			var header = new ViewContainer { Name = "header" };
			var navigation = new ViewContainer { Name = "navigation" };
			var workAreaTitle = new ViewContainer { Name = "workareatitle" };
			var workArea = new ViewContainer { Name = "workarea", IsXOR = true };

			masterArea.ViewElements.Add(header);
			masterArea.ViewElements.Add(navigation);
			masterArea.ViewElements.Add(workAreaTitle);
			masterArea.ViewElements.Add(workArea);
		}
	}
}
