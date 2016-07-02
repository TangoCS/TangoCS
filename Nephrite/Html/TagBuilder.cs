using System;
using System.Collections.Generic;
using System.Net;

namespace Nephrite.Html
{
	public class TagBuilder
	{
		private string _innerHtml;

		public TagBuilder(string tagName)
		{
			TagName = tagName;
		}

		public IDictionary<string, string> Attributes { get; private set; }

		public string InnerHtml
		{
			get { return _innerHtml ?? String.Empty; }
			set { _innerHtml = value; }
		}

		public string TagName { get; private set; }

		private void AppendAttributes(IHtmlWriter w)
		{
			if (Attributes == null) return;
			foreach (var attribute in Attributes)
			{
				if (attribute.Value != null)
				{
					w.Write(' ');
					w.Write(attribute.Key);
					w.Write("=\"");
					w.Write(WebUtility.HtmlEncode(attribute.Value));
					w.Write('"');
				}
			}
		}

		public void SetAttributes(IDictionary<string, string> attributes)
		{
			Attributes = attributes;
		}

		public void SetInnerText(string innerText)
		{
			InnerHtml = WebUtility.HtmlEncode(innerText);
		}

		public void Render(IHtmlWriter w, TagRenderMode renderMode = TagRenderMode.Normal)
		{
			switch (renderMode)
			{
				case TagRenderMode.StartTag:
					w.Write('<');
                    w.Write(TagName);
					AppendAttributes(w);
					w.Write('>');
					break;
				case TagRenderMode.EndTag:
					w.Write("</");
                    w.Write(TagName);
                    w.Write('>');
					break;
				case TagRenderMode.SelfClosing:
					w.Write('<');
                    w.Write(TagName);
					AppendAttributes(w);
					w.Write(" />");
					break;
				default:
					w.Write('<');
                    w.Write(TagName);
					AppendAttributes(w);
					w.Write('>');
                    w.Write(InnerHtml);
                    w.Write("</");
                    w.Write(TagName);
                    w.Write('>');
					break;
			}
		}
	}

	public enum TagRenderMode
	{
		Normal,
		StartTag,
		EndTag,
		SelfClosing
	}
}