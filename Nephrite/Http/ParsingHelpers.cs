using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Nephrite.Http
{
	internal static class ParsingHelpers
	{
		private static readonly Action<string, string, object> AddCookieCallback = (name, value, state) =>
		{
			var dictionary = (IDictionary<string, string>)state;
			if (!dictionary.ContainsKey(name))
			{
				dictionary.Add(name, value);
			}
		};

		private static readonly char[] SemicolonAndComma = new[] { ';', ',' };

		internal static void ParseCookies(string cookiesHeader, IDictionary<string, string> cookiesCollection)
		{
			ParseDelimited(cookiesHeader, SemicolonAndComma, AddCookieCallback, cookiesCollection);
		}

		internal static void ParseDelimited(string text, char[] delimiters, Action<string, string, object> callback, object state)
		{
			int textLength = text.Length;
			int equalIndex = text.IndexOf('=');
			if (equalIndex == -1)
			{
				equalIndex = textLength;
			}
			int scanIndex = 0;
			while (scanIndex < textLength)
			{
				int delimiterIndex = text.IndexOfAny(delimiters, scanIndex);
				if (delimiterIndex == -1)
				{
					delimiterIndex = textLength;
				}
				if (equalIndex < delimiterIndex)
				{
					while (scanIndex != equalIndex && char.IsWhiteSpace(text[scanIndex]))
					{
						++scanIndex;
					}
					string name = text.Substring(scanIndex, equalIndex - scanIndex);
					string value = text.Substring(equalIndex + 1, delimiterIndex - equalIndex - 1);
					callback(
						Uri.UnescapeDataString(name.Replace('+', ' ')),
						Uri.UnescapeDataString(value.Replace('+', ' ')),
						state);
					equalIndex = text.IndexOf('=', delimiterIndex);
					if (equalIndex == -1)
					{
						equalIndex = textLength;
					}
				}
				scanIndex = delimiterIndex + 1;
			}
		}
	}
}
