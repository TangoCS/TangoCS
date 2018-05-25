using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

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
		string _idPrefix;
		string _initialPrefix;
		Stack<string> _ids = new Stack<string>(4);

		public string IDPrefix => _idPrefix;

		public HtmlWriter() { }
		public HtmlWriter(StringBuilder sb) : base(sb) { }
		public HtmlWriter(string idPrefix)
		{
			_initialPrefix = idPrefix;
			SetPrefix();
		}
		public HtmlWriter(string idPrefix, StringBuilder sb) : base(sb)
		{
			_initialPrefix = idPrefix;
			SetPrefix();
		}

		public override void WriteLine(string value)
		{
			Write(value);
			Write("<br/>");
		}

		public void PushID(string id)
		{
			_ids.Push(id);
			SetPrefix();
		}

		public void PopID()
		{
			_ids.Pop();
			SetPrefix();
		}

		void SetPrefix()
		{
			var str = new[] { _initialPrefix, String.Join("_", _ids.Where(s => !string.IsNullOrEmpty(s))) };
			_idPrefix = String.Join("_", str.Where(s => !string.IsNullOrEmpty(s)));
		}
	}
}