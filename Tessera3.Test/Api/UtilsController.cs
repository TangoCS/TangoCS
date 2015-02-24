using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Razor;

namespace Tessera3.Api
{
	public class UtilsController
	{
		public void ClearCache()
		{
			RazorFormRenderer.Configure("Views");
			HttpContext.Current.Response.Redirect("/");
		}

		public void Test()
		{

		}
	}
}