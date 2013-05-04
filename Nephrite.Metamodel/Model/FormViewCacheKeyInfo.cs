using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Metamodel.Model
{
	public class FormViewCacheKeyInfo
	{
		public string[] Tables { get; set; }
		public string[] QueryParams { get; set; }
		public string[] Macro { get; set; }

		public FormViewCacheKeyInfo()
		{
			Tables = new string[0];
			QueryParams = new string[0];
			Macro = new string[0];
		}
	}
}