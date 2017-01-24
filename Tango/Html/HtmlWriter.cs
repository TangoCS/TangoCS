using System;
using System.IO;

namespace Tango.Html
{
	public interface IHtmlWriter
	{
		string IDPrefix { get; set; }
		void Write(string value);
		void Write(char value);
	}

	public class HtmlWriter : StringWriter, IHtmlWriter
	{
		public string IDPrefix { get; set; }

		public override void WriteLine(string value)
		{
			Write(value);
			Write("<br/>");
		}
	}
}