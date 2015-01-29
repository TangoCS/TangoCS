using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Nephrite
{
	public static partial class SimpleTypeExtensions
	{
		public static int ToInt32(this string src, int defaultValue)
		{
			int x;
			if (int.TryParse(src, out x))
				return x;
			return defaultValue;
		}

		public static long ToInt64(this string src, long defaultValue)
		{
			long x;
			if (long.TryParse(src, out x))
				return x;
			return defaultValue;
		}

		public static int? ToInt32(this string src)
		{
			int x;
			if (int.TryParse(src, out x))
				return x;
			return null;
		}

		public static Guid ToGuid(this string src)
		{
			try
			{
				return string.IsNullOrEmpty(src) ? Guid.Empty : new Guid(src);
			}
			catch
			{
				return Guid.Empty;
			}
		}

		public static double ToDouble(this string src, double defaultValue)
		{
			double x;
			if (double.TryParse(src.Replace(",", ".").Replace(" ", "").Replace(" ", ""), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out x))
				return x;
			return defaultValue;
		}

		public static double? ToDouble(this string src)
		{
			double x;
			if (double.TryParse(src.Replace(",", ".").Replace(" ", "").Replace(" ", ""), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out x))
				return x;
			return null;
		}

		public static decimal ToDecimal(this string src, decimal defaultValue)
		{
			if (src == null)
				return defaultValue;
			decimal x;
			if (decimal.TryParse(src.Replace(",", ".").Replace(" ", "").Replace(" ", ""), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out x))
				return x;
			return defaultValue;
		}

		public static decimal? ToDecimal(this string src)
		{
			if (src == null)
				return null;
			decimal x;
			if (decimal.TryParse(src.Replace(",", ".").Replace(" ", "").Replace(" ", ""), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out x))
				return x;
			return null;
		}



		public static DateTime ToDate(this string src, DateTime defaultValue)
		{
			DateTime dt;
			if (DateTime.TryParseExact(src, "d.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out dt))
				return dt;
			return defaultValue;
		}

		public static DateTime? ToDate(this string src)
		{
			DateTime dt;
			if (DateTime.TryParseExact(src, "d.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out dt))
				return dt;
			return null;
		}

		public static DateTime ToDateTime(this string src, DateTime defaultValue)
		{
			DateTime dt;
			src = src.Replace("%20", " ");
			src = src.Replace("%3a", ":");
			src = src.Replace("+", " ");
			if (DateTime.TryParseExact(src, "d.MM.yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dt))
				return dt;
			if (DateTime.TryParseExact(src, "d.MM.yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dt))
				return dt;
			return defaultValue;
		}

		public static DateTime? ToDateTime(this string src)
		{
			DateTime dt;
			src = src.Replace("%20", " ");
			src = src.Replace("%3a", ":");
			src = src.Replace("+", " ");
			if (DateTime.TryParseExact(src, "d.MM.yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dt))
				return dt;
			if (DateTime.TryParseExact(src, "d.MM.yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dt))
				return dt;
			return null;
		}

		public static string DateToString(this DateTime? src)
		{
			return src.DateToString(String.Empty);
		}

		public static string DateToString(this DateTime src)
		{
			return src.ToString("dd.MM.yyyy");
		}

		public static string DateToString(this DateTime? src, string defaultValue)
		{
			if (src.HasValue)
				return src.Value.ToString("dd.MM.yyyy");
			return defaultValue;
		}

		public static string TimeToString(this DateTime? src)
		{
			if (src.HasValue)
				return src.Value.ToString("HH:mm");
			return String.Empty;
		}

		public static string TimeToString(this DateTime src)
		{
			return src.ToString("HH:mm");
		}

		public static string DateTimeToString(this DateTime? src)
		{
			return src.DateTimeToString(String.Empty);
		}

		public static string DateTimeToString(this DateTime src)
		{
			return src == DateTime.MinValue ? "" : src.ToString("dd.MM.yyyy HH:mm");
		}

		public static string DateTimeToString(this DateTime? src, string defaultValue)
		{
			if (src.HasValue)
				return src.Value.ToString("dd.MM.yyyy HH:mm");
			return defaultValue;
		}

		public static string Join(this string[] str, string separator)
		{
			return String.Join(separator, str.Where(s => !String.IsNullOrEmpty(s)).ToArray());
		}

		public static string Join(this IEnumerable<string> str, string separator)
		{
			return String.Join(separator, str.Where(s => !String.IsNullOrEmpty(s)).ToArray());
		}

		public static StringBuilder Append(this StringBuilder sb, StringBuilder value)
		{
			sb.EnsureCapacity(sb.Length + value.Length);
			for (int i = 0; i < value.Length; i++)
			{
				sb.Append(value[i]);
			}
			return sb;
		}

		/// <summary>
		/// Число римскими цифрами
		/// </summary>
		/// <param name="n"></param>
		/// <returns></returns>
		public static string GetRomanString(this int n)
		{
			if ((n < 0) || (n > 3999)) throw new ArgumentOutOfRangeException("Value must be between 1 and 3999");
			if (n < 1) return string.Empty;
			if (n >= 1000) return "M" + GetRomanString(n - 1000);
			if (n >= 900) return "CM" + GetRomanString(n - 900);
			if (n >= 500) return "D" + GetRomanString(n - 500);
			if (n >= 400) return "CD" + GetRomanString(n - 400);
			if (n >= 100) return "C" + GetRomanString(n - 100);
			if (n >= 90) return "XC" + GetRomanString(n - 90);
			if (n >= 50) return "L" + GetRomanString(n - 50);
			if (n >= 40) return "XL" + GetRomanString(n - 40);
			if (n >= 10) return "X" + GetRomanString(n - 10);
			if (n >= 9) return "IX" + GetRomanString(n - 9);
			if (n >= 5) return "V" + GetRomanString(n - 5);
			if (n >= 4) return "IV" + GetRomanString(n - 4);
			if (n >= 1) return "I" + GetRomanString(n - 1);
			throw new ArgumentOutOfRangeException("Value must be between 1 and 3999");
		}

		public static bool In(this string str, params string[] items)
		{
			return items.Contains(str);
		}

		public static bool IsEmpty(this string str)
		{
			return String.IsNullOrEmpty(str) ? true : str.Trim() == String.Empty;
		}

		public static bool Validate(this string str, string pattern)
		{
			if (str.IsEmpty())
				return true;

			return Regex.IsMatch(str, pattern);
		}

		public static string GetAttributeValue(this XElement element, XName name)
		{
			var attribute = element.Attribute(name);
			return attribute != null ? attribute.Value : null;
		}

		private enum State
		{
			AtBeginningOfToken,
			InNonQuotedToken,
			InQuotedToken,
			ExpectingComma,
			InEscapedCharacter
		};

		public static string[] CsvSplit(this String source)
		{
			return CsvSplit(source, ';');
		}

		public static string[] CsvSplit(this String source, char delimiter)
		{
			List<string> splitString = new List<string>();
			List<int> slashesToRemove = null;
			State state = State.AtBeginningOfToken;
			char[] sourceCharArray = source.ToCharArray();
			int tokenStart = 0;
			int len = sourceCharArray.Length;
			for (int i = 0; i < len; ++i)
			{
				switch (state)
				{
					case State.AtBeginningOfToken:
						if (sourceCharArray[i] == '"')
						{
							state = State.InQuotedToken;
							slashesToRemove = new List<int>();
							continue;
						}
						if (sourceCharArray[i] == delimiter)
						{
							splitString.Add("");
							tokenStart = i + 1;
							continue;
						}
						state = State.InNonQuotedToken;
						continue;
					case State.InNonQuotedToken:
						if (sourceCharArray[i] == delimiter)
						{
							splitString.Add(
								source.Substring(tokenStart, i - tokenStart));
							state = State.AtBeginningOfToken;
							tokenStart = i + 1;
						}
						continue;
					case State.InQuotedToken:
						if (sourceCharArray[i] == '"')
						{
							state = State.ExpectingComma;
							continue;
						}
						if (sourceCharArray[i] == '\\')
						{
							state = State.InEscapedCharacter;
							slashesToRemove.Add(i - tokenStart);
							continue;
						}
						continue;
					case State.ExpectingComma:
						if (sourceCharArray[i] != delimiter)
							throw new Exception("Expecting comma. String: " + source + ". Position " + i.ToString());
						string stringWithSlashes =
							source.Substring(tokenStart, i - tokenStart);
						foreach (int item in slashesToRemove.Reverse<int>())
							stringWithSlashes =
								stringWithSlashes.Remove(item, 1);
						splitString.Add(
							stringWithSlashes.Substring(1,
								stringWithSlashes.Length - 2));
						state = State.AtBeginningOfToken;
						tokenStart = i + 1;
						continue;
					case State.InEscapedCharacter:
						state = State.InQuotedToken;
						continue;
				}
			}
			switch (state)
			{
				case State.AtBeginningOfToken:
					splitString.Add("");
					return splitString.ToArray();
				case State.InNonQuotedToken:
					splitString.Add(
						source.Substring(tokenStart,
							source.Length - tokenStart));
					return splitString.ToArray();
				case State.InQuotedToken:
					throw new Exception("Expecting ending quote. String: " + source);
				case State.ExpectingComma:
					string stringWithSlashes =
						source.Substring(tokenStart, source.Length - tokenStart);
					foreach (int item in slashesToRemove.Reverse<int>())
						stringWithSlashes = stringWithSlashes.Remove(item, 1);
					splitString.Add(
						stringWithSlashes.Substring(1,
							stringWithSlashes.Length - 2));
					return splitString.ToArray();
				case State.InEscapedCharacter:
					throw new Exception("Expecting escaped character. String: " + source);
			}
			throw new Exception("Unexpected error");
		}

		public static string ConvertToFtsQuery(this string str)
		{
			string[] words = str.Split(new char[] { ',', ' ', '?' }, StringSplitOptions.RemoveEmptyEntries);
			StringBuilder sb = new StringBuilder(200);
			for (int i = 0; i < words.Length; i++)
			{
				words[i] = words[i].Replace("*", "").Replace("\"", "").Replace("'", "");
				sb.Append(sb.Length > 0 ? " AND " : "");
				sb.AppendFormat("(\"{0}*\")", words[i]);
			}
			return sb.ToString();
		}

		public static string ConvertToFtsQueryOR(this string str)
		{
			string[] words = str.Split(new char[] { ',', ' ', '?' }, StringSplitOptions.RemoveEmptyEntries);
			StringBuilder sb = new StringBuilder(200);
			for (int i = 0; i < words.Length; i++)
			{
				words[i] = words[i].Replace("*", "").Replace("\"", "").Replace("'", "");
				sb.Append(sb.Length > 0 ? " OR " : "");
				sb.AppendFormat("(\"{0}*\")", words[i]);
			}
			return sb.ToString();
		}

		public static string Arg(this string str, int index)
		{
			if (str == null)
				return String.Empty;

			string[] args = str.Split('|');
			if (index >= args.Length)
				return String.Empty;
			return args[index];
		}

		public static void SetPropertyValue(this object obj, string propertyName, object value)
		{
			obj.GetType().GetProperty(propertyName).SetValue(obj, value, null);
		}

	}
}