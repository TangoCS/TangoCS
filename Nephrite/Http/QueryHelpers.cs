using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Nephrite.Http
{
	public static class QueryHelpers
	{
		/// <summary>
		/// Parse a query string into its component key and value parts.
		/// </summary>
		/// <param name="text">The raw query string value, with or without the leading '?'.</param>
		/// <returns>A collection of parsed keys and values.</returns>
		public static IDictionary<string,string> ParseQuery(string queryString)
		{
			if (!string.IsNullOrEmpty(queryString) && queryString[0] == '?')
			{
				queryString = queryString.Substring(1);
			}
			var accumulator = new Dictionary<string, string>();

			int textLength = queryString.Length;
			int equalIndex = queryString.IndexOf('=');
			if (equalIndex == -1)
			{
				equalIndex = textLength;
			}
			int scanIndex = 0;
			while (scanIndex < textLength)
			{
				int delimiterIndex = queryString.IndexOf('&', scanIndex);
				if (delimiterIndex == -1)
				{
					delimiterIndex = textLength;
				}
				if (equalIndex < delimiterIndex)
				{
					while (scanIndex != equalIndex && char.IsWhiteSpace(queryString[scanIndex]))
					{
						++scanIndex;
					}
					string name = queryString.Substring(scanIndex, equalIndex - scanIndex);
					string value = queryString.Substring(equalIndex + 1, delimiterIndex - equalIndex - 1);
					accumulator.Add(
						Uri.UnescapeDataString(name.Replace('+', ' ')),
						Uri.UnescapeDataString(value.Replace('+', ' ')));
					equalIndex = queryString.IndexOf('=', delimiterIndex);
					if (equalIndex == -1)
					{
						equalIndex = textLength;
					}
				}
				scanIndex = delimiterIndex + 1;
			}

			return accumulator;
		}

		public static string CreateUrl(string route, HtmlParms parms = null)
		{
			string s = "/" + route;
			HtmlParms p = new HtmlParms();
			foreach (var parm in parms)
			{
				if (route.IndexOf("{" + parm.Key + "}") != -1)
					s = s.Replace("{" + parm.Key + "}", parm.Value);
				else
					p.Add(parm.Key, parm.Value);
			}
			string ps = p.ToString();
			return s + (ps.IsEmpty() ? "" : ("?" + ps));
		}

	}

	public class HtmlParms : Dictionary<string, string>
	{
		public HtmlParms() : base() { }
		public HtmlParms(IDictionary<string, string> dictionary) : base(dictionary) { }

		public override string ToString()
		{
			return this.Select(o => o.Value.IsEmpty() ? "" : (o.Key + "=" + o.Value)).Join("&");
		}

		public HtmlParms(string key, string value)
			: base()
		{
			Add(key, value);
		}
	}


}
