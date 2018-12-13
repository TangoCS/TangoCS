using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Tango.Html
{
	public partial class HtmlWriter : StringWriter, IContentWriter
	{
		const string NEWLINE = "<br/>";
		string _initialPrefix;
		Stack<string> _ids = new Stack<string>(4);
		Stack<(string prefix, Stack<string> ids)> _prefixes = new Stack<(string prefix, Stack<string> ids)>(4);

		public string IDPrefix { get; private set; }

		public HtmlWriter()
		{
			NewLine = NEWLINE;
		}
		public HtmlWriter(StringBuilder sb) : base(sb)
		{
			NewLine = NEWLINE;
		}
		public HtmlWriter(string idPrefix)
		{
			NewLine = NEWLINE;
			_initialPrefix = idPrefix;
			SetPrefix();
		}
		public HtmlWriter(string idPrefix, StringBuilder sb) : base(sb)
		{
			NewLine = NEWLINE;
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


		IDictionary<string, string> attributes = new Dictionary<string, string>(7, StringComparer.Ordinal);

		public void WriteAttr(string key, string value, bool replaceExisting = true)
		{
			if (replaceExisting || (!string.IsNullOrEmpty(value) && !attributes.ContainsKey(key)))
				attributes[key] = value;
			else if (!string.IsNullOrEmpty(value))
				attributes[key] = attributes[key] + " " + value;
		}

		public void WriteAttrID(string key, string value)
		{
			attributes[key] = value == null ?
				IDPrefix :
				value.StartsWith("#") ? value.Substring(1) : this.GetID(value);
		}

		public void RenderAttrs()
		{
			foreach (var attribute in attributes)
			{
				if (attribute.Value != null)
					Write($" {attribute.Key}=\"{attribute.Value}\"");
			}
			attributes.Clear();
		}
    }
}