using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

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

		private void AppendAttributes(StringBuilder sb)
		{
			foreach (var attribute in Attributes)
			{
				string key = attribute.Key;
				string value = HtmlHelpers.HtmlEncode(attribute.Value);
				sb.Append(' ')
					.Append(key)
					.Append("=\"")
					.Append(value)
					.Append('"');
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
			InnerHtml = HtmlHelpers.HtmlEncode(innerText);
		}

		public override string ToString()
		{
			return ToString(TagRenderMode.Normal);
		}

		public StringBuilder Render(TagRenderMode renderMode = TagRenderMode.Normal)
		{
			StringBuilder sb = new StringBuilder(255);
			switch (renderMode)
			{
				case TagRenderMode.StartTag:
					sb.Append('<')
						.Append(TagName);
					AppendAttributes(sb);
					sb.Append('>');
					break;
				case TagRenderMode.EndTag:
					sb.Append("</")
						.Append(TagName)
						.Append('>');
					break;
				case TagRenderMode.SelfClosing:
					sb.Append('<')
						.Append(TagName);
					AppendAttributes(sb);
					sb.Append(" />");
					break;
				default:
					sb.Append('<')
						.Append(TagName);
					AppendAttributes(sb);
					sb.Append('>')
						.Append(InnerHtml)
						.Append("</")
						.Append(TagName)
						.Append('>');
					break;
			}
			return sb;
		}

		public string ToString(TagRenderMode renderMode)
		{
			return Render(renderMode).ToString();
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