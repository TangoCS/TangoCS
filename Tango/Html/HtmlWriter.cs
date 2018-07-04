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

		IAttributeWriter AttributeWriter { get; }

		void Write(string value);
		void Write(char value);
	}

	public class HtmlWriter : StringWriter, IHtmlWriter
	{
		string _idPrefix;
		string _initialPrefix;
		Stack<string> _ids = new Stack<string>(4);

		public IAttributeWriter AttributeWriter { get; private set; }
		public string IDPrefix => _idPrefix;

		public HtmlWriter()
		{
			AttributeWriter = new AttributeWriter(this);
		}
		public HtmlWriter(StringBuilder sb) : base(sb)
		{
			AttributeWriter = new AttributeWriter(this);
		}
		public HtmlWriter(string idPrefix)
		{
			AttributeWriter = new AttributeWriter(this);
			_initialPrefix = idPrefix;
			SetPrefix();
		}
		public HtmlWriter(string idPrefix, StringBuilder sb) : base(sb)
		{
			AttributeWriter = new AttributeWriter(this);
			_initialPrefix = idPrefix;
			SetPrefix();
		}

		public override void WriteLine(string value)
		{
			Write(value);
			Write("<br/>");
		}

		public void PushPrefix(string prefix)
		{
			_ids.Push(prefix);
			SetPrefix();
		}

		public void PopPrefix()
		{
			_ids.Pop();
			SetPrefix();
		}

		void SetPrefix()
		{
			var str = new[] { _initialPrefix, String.Join("_", _ids.Where(s => !string.IsNullOrEmpty(s)).Reverse()) };
			_idPrefix = String.Join("_", str.Where(s => !string.IsNullOrEmpty(s)));
		}
	}
}