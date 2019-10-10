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
		Stack<string> _ids = new Stack<string>(4);

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
			idPrefix.Split('_').ForEach(s => _ids.Push(s));
			SetPrefix();
		}
		public HtmlWriter(string idPrefix, StringBuilder sb) : base(sb)
		{
			NewLine = NEWLINE;
			idPrefix.Split('_').ForEach(s => _ids.Push(s));
			SetPrefix();
		}

		public override void WriteLine(string value)
		{
			Write(value);
			Write("<br/>");
		}

		public void PushID(string id)
		{
			_ids.Push(id.ToLower());
			SetPrefix();
		}

		public void PushPrefix(string prefix)
		{
			_ids.Push(prefix.ToLower());
			SetPrefix();
		}

		public void PopID()
		{
			_ids.Pop();
			SetPrefix();
		}

		public void PopPrefix()
		{
			_ids.Pop();
			SetPrefix();
		}

		void SetPrefix()
		{
			IDPrefix = String.Join("_", _ids.Where(s => !string.IsNullOrEmpty(s)).Reverse());
		}


		IDictionary<string, string> attributes = new Dictionary<string, string>(7, StringComparer.Ordinal);

		public void WriteAttr(string key, string value, bool replaceExisting = true)
		{
			if (replaceExisting || (!string.IsNullOrEmpty(value) && !attributes.ContainsKey(key)))
				attributes[key] = value;
			else if (!string.IsNullOrEmpty(value))
				attributes[key] = attributes[key] + " " + value;
		}

		public void WriteAttr(string key)
		{
			attributes[key] = null;
		}

		public void WriteAttrID(string key, string value)
		{
			attributes[key] = value == null ?
				IDPrefix :
				this.GetID(value);
		}

		public void RenderAttrs()
		{
			foreach (var attribute in attributes)
			{
				if (attribute.Value != null)
					Write($" {attribute.Key}=\"{attribute.Value}\"");
				else
					Write($" {attribute.Key}");
			}
			attributes.Clear();
		}

        public string Write(IEnumerable<string> text)
        {
            return String.Join("<br/>", text);
        }
    }
}