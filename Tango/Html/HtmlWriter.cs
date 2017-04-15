using System;
using System.IO;
using System.Text;

namespace Tango.Html
{
	public interface IHtmlWriter
	{
		string IDPrefix { get; }
		void Write(string value);
		void Write(char value);
	}

	public class HtmlWriter : StringWriter, IHtmlWriter
	{
		public string IDPrefix { get; }

		public HtmlWriter() { }
		public HtmlWriter(StringBuilder sb) : base(sb) { }
		public HtmlWriter(string idPrefix) { IDPrefix = idPrefix; }
		public HtmlWriter(string idPrefix, StringBuilder sb) : base(sb) { IDPrefix = idPrefix; }

		public override void WriteLine(string value)
		{
			Write(value);
			Write("<br/>");
		}
	}
}