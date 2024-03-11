using System;
using System.Collections.Generic;
using System.Text;
using Tango.UI;

namespace Tango.Localization
{
	public class MultilingualDefaultRouteResolver : IDefaultRouteResolver
	{
		public string Resolve(ActionContext context)
		{
			return context.Lang.IsEmpty() ? "default" : "default-lang";
		}
	}
}
