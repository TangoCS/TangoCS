using System;
using System.IO;

namespace Nephrite.Html
{
	public partial class HtmlWriter : StringWriter
	{
		public static implicit operator string (HtmlWriter w)
		{
			return w.ToString();
		}
	}
}