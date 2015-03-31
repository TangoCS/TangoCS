using System;
using System.Collections.Generic;
using System.Linq;
using Nephrite.AccessControl;
using Nephrite.Html.Controls;

namespace Solution.Configuration
{
	public static class Sidebar
	{
		public static void Configure()
		{
			Navigation nav = new Navigation(ActionAccessControl.Instance);
			nav.AddItem("Metadata", "");
			nav.AddItem("Administration", "");
			
		}
	}
}