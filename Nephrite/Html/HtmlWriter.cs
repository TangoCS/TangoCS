using System.IO;

namespace Nephrite.Html
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
	}

	public class HtmlStreamWriter : StreamWriter, IHtmlWriter
	{
		public string IDPrefix { get; set; }
		public HtmlStreamWriter(Stream stream) : base(stream) { }
	}
}