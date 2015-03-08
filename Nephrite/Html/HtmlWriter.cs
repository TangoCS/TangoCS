using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace Nephrite.Html
{
	public abstract partial class HtmlWriter : StringWriter
	{
		public HtmlPage Page { get; private set; }

		public HtmlWriter(HtmlPage page)
		{
			Page = page;	
		}

		public abstract void Render();
	}
}