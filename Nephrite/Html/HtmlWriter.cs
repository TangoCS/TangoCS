using System;
using System.IO;
using System.Text;

namespace Nephrite.Html
{
	public interface IHtmlWriter
	{
		void Write(string value);
		void Write(char value);
	}

	public class HtmlWriter : StringWriter, IHtmlWriter
	{
		public static implicit operator string (HtmlWriter w)
		{
			return w.ToString();
		}
	}

	public class HtmlStreamWriter : StreamWriter, IHtmlWriter
	{
		public HtmlStreamWriter(Stream stream) : base(stream) { }
	}
}