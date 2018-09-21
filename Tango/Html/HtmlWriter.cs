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
		string _initialPrefix;
		Stack<string> _ids = new Stack<string>(4);
		Stack<(string prefix, Stack<string> ids)> _prefixes = new Stack<(string prefix, Stack<string> ids)>(4);

		public IAttributeWriter AttributeWriter { get; private set; }
		public string IDPrefix { get; private set; }

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

		public void PushPrefix(string prefix)
		{
			_prefixes.Push((_initialPrefix, _ids));
			_ids.Clear();
			_initialPrefix = prefix;
			SetPrefix();
		}

		public void PopPrefix()
		{
			(_initialPrefix, _ids) = _prefixes.Pop();
			SetPrefix();
		}

		void SetPrefix()
		{
			var str = new[] { _initialPrefix, String.Join("_", _ids.Where(s => !string.IsNullOrEmpty(s)).Reverse()) };
			IDPrefix = String.Join("_", str.Where(s => !string.IsNullOrEmpty(s)));
		}
	}
}