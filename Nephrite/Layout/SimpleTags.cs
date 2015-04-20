using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nephrite.Html;

namespace Nephrite.Layout
{
	public class SimpleTags : ILayoutSimpleTags
	{
		public string Image(string src, string alt, object attributes)
		{
			return string.Format("<img src='{0}' class='middle' alt='{1}' title='{1}' />", IconSet.RootPath + src, alt);
		}
	}
}