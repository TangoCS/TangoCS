using System;
using System.IO;
using System.Text;

namespace Nephrite.Html
{
	public partial class HtmlWriter : StringWriter
	{
		public HtmlWriter() { }
		public HtmlWriter(StringBuilder sb) : base(sb) { }

		public static implicit operator string (HtmlWriter w)
		{
			return w.ToString();
		}
	}
}