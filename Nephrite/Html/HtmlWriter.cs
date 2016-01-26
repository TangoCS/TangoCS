using System.IO;

namespace Nephrite.Html
{
	public interface IHtmlWriter
	{
		string IDPrefix { get; set; }
		void Write(string value);
		void Write(char value);
	}
}