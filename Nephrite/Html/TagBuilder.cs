using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;

namespace Nephrite.Html
{
	public class TagBuilder
	{
		private string _innerHtml;

		public TagBuilder(string tagName)
		{
			TagName = tagName;
			Attributes = new SortedDictionary<string, string>(StringComparer.Ordinal);
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
			foreach (var attribute in Attributes)
			{
				string key = attribute.Key;
				if (!attribute.Value.IsEmpty())
				{
					string value = WebUtility.HtmlEncode(attribute.Value);
					w.Write(' ');
					w.Write(key);
					w.Write("=\"");
					w.Write(value);
					w.Write('"');
				}
			}
		}

		public void MergeAttribute(string key, string value, bool replaceExisting = false)
		{
			if (replaceExisting || !Attributes.ContainsKey(key))
			{
				Attributes[key] = value;
			}
		}

		public void MergeAttributes<TKey, TValue>(IDictionary<TKey, TValue> attributes, bool replaceExisting = false)
		{
			if (attributes != null)
			{
				foreach (var entry in attributes)
				{
					string key = Convert.ToString(entry.Key, CultureInfo.InvariantCulture);
					string value = Convert.ToString(entry.Value, CultureInfo.InvariantCulture);
					MergeAttribute(key, value, replaceExisting);
				}
			}
		}

		public void SetInnerText(string innerText)
		{
			InnerHtml = WebUtility.HtmlEncode(innerText);
		}

		//public override string ToString()
		//{
		//	return ToString(TagRenderMode.Normal);
		//}

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

		//public string ToString(TagRenderMode renderMode)
		//{
		//	return Render(renderMode).ToString();
		//}
	}

	public enum TagRenderMode
	{
		Normal,
		StartTag,
		EndTag,
		SelfClosing
	}
}