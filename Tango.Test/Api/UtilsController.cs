﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tango.Razor;

namespace Tessera3.Api
{
	public class UtilsController
	{
		public void ClearCache()
		{
			//RazorRenderer.Configure("Views");
			HttpContext.Current.Response.Redirect("/");
		}

		public void Test()
		{

		}
	}
}