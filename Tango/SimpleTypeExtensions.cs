using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Tango
{
	public static partial class SimpleTypeExtensions
	{
		public static bool ToBoolean(this string src, bool defaultValue)
		{
			return src?.ToLower().In("true", "1") ?? defaultValue;
		}

		public static bool? ToBoolean(this string src)
		{
			return src?.ToLower().In("true", "1");
		}

		public static int ToInt32(this string src, int defaultValue)
		{
			if (int.TryParse(src, out int x))
				return x;
			return defaultValue;
		}

		public static long ToInt64(this string src, long defaultValue)
		{
			if (long.TryParse(src, out long x))
				return x;
			return defaultValue;
		}

		public static long? ToInt64(this string src)
		{
			if (long.TryParse(src, out long x))
				return x;
			return null;
		}

		public static int? ToInt32(this string src)
		{
			if (int.TryParse(src, out int x))
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
			if (src == null)
				return defaultValue;
			if (double.TryParse(src.Replace(",", ".").Replace(" ", "").Replace(" ", ""), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double x))
				return x;
			return defaultValue;
		}

		public static double? ToDouble(this string src)
		{
			if (src == null)
				return null;
			if (double.TryParse(src.Replace(",", ".").Replace(" ", "").Replace(" ", ""), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double x))
				return x;
			return null;
		}

		public static decimal ToDecimal(this string src, decimal defaultValue)
		{
			if (src == null)
				return defaultValue;
			if (decimal.TryParse(src.Replace(",", ".").Replace(" ", "").Replace(" ", ""), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out decimal x))
				return x;
			return defaultValue;
		}

		public static decimal? ToDecimal(this string src)
		{
			if (src == null)
				return null;
			if (decimal.TryParse(src.Replace(",", ".").Replace(" ", "").Replace(" ", ""), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out decimal x))
				return x;
			return null;
		}



		public static DateTime ToDate(this string src, DateTime defaultValue)
		{
			return src.ToDate(null, defaultValue);
		}

        public static DateTime ToDate(this string src, string format, DateTime defaultValue)
        {
			var formats = new List<string>() { format, "dd.MM.yyyy", "yyyy-MM-dd", "yyyyMMdd" }
					.Where(x => x != null).ToArray();

			if (DateTime.TryParseExact(src, formats, null, DateTimeStyles.None, out DateTime dt))
				return dt;

            return defaultValue;
        }

        public static DateTime? ToDate(this string src)
		{
			var formats = new List<string>() { "dd.MM.yyyy", "yyyy-MM-dd", "yyyyMMdd" }
					.Where(x => x != null).ToArray();

			if (DateTime.TryParseExact(src, formats, null, DateTimeStyles.None, out DateTime dt))
				return dt;

			return null;
		}

		public static DateTime? ToDateTime(this string src)
		{
			if (src.IsEmpty()) return null;
			return src.ToDateTime(null);
		}

		public static DateTime ToDateTime(this string src, DateTime defaultValue)
		{
			if (src.IsEmpty()) return defaultValue;
			return src.ToDateTime(defaultValue, null);
		}

		public static DateTime ToDateTime(this string src, DateTime defaultValue, params string[] customFormats)
		{
			if (src.IsEmpty()) return defaultValue;
			return src.ToDateTime(customFormats) ?? defaultValue;
		}

		public static DateTime? ToDateTime(this string src, params string[] customFormats)
		{
			if (src.IsEmpty()) return null;

			src = src.Replace("%20", " ");
			src = src.Replace("%3a", ":");
			src = src.Replace("+", " ");

			var formats = new List<string>(customFormats ?? new string[] { }) { "dd.MM.yyyy HH:mm:ss", "dd.MM.yyyy HH:mm", "dd.MM.yyyy", "yyyy-MM-dd", "yyyyMMdd", "yyyyMMddHHmm" }
					.Where(x => x != null).ToArray();

			if (DateTime.TryParseExact(src, formats, null, DateTimeStyles.None, out DateTime dt))
				return dt;

			return null;
		}

		public static DateTime UnixTimeStampToDateTime(this double unixTimeStamp)
		{
			// Unix timestamp is seconds past epoch
			System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
			return dtDateTime;
		}

		public static DateTime RoundUp(this DateTime dt, TimeSpan d)
		{
			var modTicks = dt.Ticks % d.Ticks;
			var delta = modTicks != 0 ? d.Ticks - modTicks : 0;
			return new DateTime(dt.Ticks + delta, dt.Kind);
		}

		public static DateTime RoundDown(this DateTime dt, TimeSpan d)
		{
			var delta = dt.Ticks % d.Ticks;
			return new DateTime(dt.Ticks - delta, dt.Kind);
		}

		public static DateTime RoundToNearest(this DateTime dt, TimeSpan d)
		{
			var delta = dt.Ticks % d.Ticks;
			bool roundUp = delta > d.Ticks / 2;
			var offset = roundUp ? d.Ticks : 0;

			return new DateTime(dt.Ticks + offset - delta, dt.Kind);
		}


		// From http://www.daimi.au.dk/~ivan/FastExpproject.pdf
		// Left to Right Binary Exponentiation
		public static decimal Pow(this decimal x, uint y)
		{
			decimal A = 1m;
			BitArray e = new BitArray(BitConverter.GetBytes(y));
			int t = e.Count;

			for (int i = t - 1; i >= 0; --i)
			{
				A *= A;
				if (e[i] == true)
				{
					A *= x;
				}
			}
			return A;
		}


		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (var item in source)
				action(item);
		}

		public static string InQuot(this string src)
		{
			return String.Concat("\"", src, "\"");
		}

		public static string InSingleQuot(this string src)
		{
			return String.Concat("'", src, "'");
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

		public static string DateTimeToStringISO8601(this DateTime src)
		{
			return src.ToString("s", CultureInfo.InvariantCulture);
		}

		public static string DateTimeToString(this DateTime? src, string defaultValue)
		{
			if (src.HasValue)
				return src.Value.ToString("dd.MM.yyyy HH:mm");
			return defaultValue;
		}
		public static DateTime EndDay(this DateTime date)
		{
			return date.Date.AddMinutes(1439);
		}
		public static DateTime EndMonth(this DateTime date)
		{
			return date.AddMonths(1).AddDays(-1).EndDay();
		}
		public static DateTime GetLastWeekdayOfMonth(this DateTime date, DayOfWeek day)
		{
			DateTime lastDayOfMonth = new DateTime(date.Year, date.Month, 1)
				.AddMonths(1).AddDays(-1);
			int wantedDay = (int)day;
			int lastDay = (int)lastDayOfMonth.DayOfWeek;
			return lastDayOfMonth.AddDays(
				lastDay >= wantedDay ? wantedDay - lastDay : wantedDay - lastDay - 7);
		}
		public static bool IsInRange(this DateTime dateToCheck, DateTime startDate, DateTime endDate, bool inclusiveEnd = false)
		{
			return dateToCheck >= startDate && dateToCheck.CompareTo(endDate) <= (inclusiveEnd ? 0 : -1);
		}
		public static bool IsInside(this (DateTime start, DateTime finish) dateToCheck, DateTime startDate, DateTime endDate)
		{
			return dateToCheck.start <= startDate && endDate <= dateToCheck.finish;
		}


		static int[] quarters = new int[] { 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4 };
		public static int Quarter(this DateTime date)
		{
			return quarters[date.Month - 1];
		}

		public static string YesNo(this bool src)
		{
			return src ? "да" : "нет";
		}

		public static string MakeValidFileName(this string name)
		{
			string invalidChars = Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
			string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

			return Regex.Replace(name, invalidRegStr, string.Empty);
		}



		public static bool In<T>(this T obj, params T[] values)
		{
			return values.Contains(obj);
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
			return element.Attribute(name)?.Value;
		}

		public static IEnumerable<string> ReadLines(this Stream stream, Encoding encoding = null)
		{
			using (var reader = new StreamReader(stream, encoding ?? Encoding.UTF8))
			{
				List<string> res = new List<string>();
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					res.Add(line);
					//yield return line;
				}
				return res;
			}
		}

		private enum State
		{
			AtBeginningOfToken,
			InNonQuotedToken,
			InQuotedToken,
			ExpectingComma,
			InEscapedCharacter
		};

		public static IEnumerable<string> Lines(this string s)
		{
			using (var tr = new StringReader(s)) while (tr.ReadLine() is string line) yield return line;
		}

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
						string stringWithSlashes = source.Substring(tokenStart, i - tokenStart);
						foreach (int item in slashesToRemove.Reverse<int>())
							stringWithSlashes = stringWithSlashes.Remove(item, 1);
						splitString.Add(stringWithSlashes.Substring(1, stringWithSlashes.Length - 2));
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
					splitString.Add(source.Substring(tokenStart, source.Length - tokenStart));
					return splitString.ToArray();
				case State.InQuotedToken:
					throw new Exception("Expecting ending quote. String: " + source);
				case State.ExpectingComma:
					string stringWithSlashes = source.Substring(tokenStart, source.Length - tokenStart);
					foreach (int item in slashesToRemove.Reverse<int>())
						stringWithSlashes = stringWithSlashes.Remove(item, 1);
					splitString.Add(stringWithSlashes.Substring(1, stringWithSlashes.Length - 2));
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

		//public static void SetPropertyValue(this object obj, string propertyName, object value)
		//{
		//	obj.GetType().GetProperty(propertyName).SetValue(obj, value, null);
		//}

		

		public static string GetFriendlyName(this Type type)
		{
			if (type == typeof(int))
				return "int";
			else if (type == typeof(short))
				return "short";
			else if (type == typeof(byte))
				return "byte";
			else if (type == typeof(bool))
				return "bool";
			else if (type == typeof(long))
				return "long";
			else if (type == typeof(float))
				return "float";
			else if (type == typeof(double))
				return "double";
			else if (type == typeof(decimal))
				return "decimal";
			else if (type == typeof(string))
				return "string";
			else if (type.IsGenericType)
				return type.Name.Split('`')[0] + "<" + string.Join(", ", type.GetGenericArguments().Select(x => GetFriendlyName(x)).ToArray()) + ">";
			else
				return type.Name;
			//string friendlyName = type.Name;
			//if (type.IsGenericType)
			//{
			//	int iBacktick = friendlyName.IndexOf('`');
			//	if (iBacktick > 0)
			//	{
			//		friendlyName = friendlyName.Remove(iBacktick);
			//	}
			//	friendlyName += "<";
			//	Type[] typeParameters = type.GetGenericArguments();
			//	for (int i = 0; i < typeParameters.Length; ++i)
			//	{
			//		string typeParamName = typeParameters[i].Name;
			//		friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
			//	}
			//	friendlyName += ">";
			//}

			//return friendlyName;
		}

		public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
		{
			while (toCheck != null && toCheck != typeof(object))
			{
				var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
				if (generic == cur)
				{
					return true;
				}
				toCheck = toCheck.BaseType;
			}
			return false;
		}

		public static StringBuilder AppendAttributes(this StringBuilder sb, object attributes, string defaultClass)
		{
			bool c = !defaultClass.IsEmpty();
			if (attributes == null)
			{
				if (c) sb.AppendFormat(@" class=""{0}""", defaultClass);
				return sb;
			}

			if (attributes is string)
			{
				if (c) sb.AppendFormat(@" class=""{0}""", defaultClass);
				sb.AppendFormat(@" style=""width:{0}""", attributes);
				return sb;
			}

			bool b = false;
			foreach (var p in attributes.GetType().GetProperties())
			{
				if (p.Name.ToLower() == "class")
				{
					sb.AppendFormat(@" {0}=""{1}""", p.Name, p.GetValue(attributes, null) + " " + defaultClass);
					b = true;
				}
				else
					sb.AppendFormat(@" {0}=""{1}""", p.Name, p.GetValue(attributes, null));
			}
			if (!b && c) sb.AppendFormat(@" class=""{0}""", defaultClass);
			return sb;
		}

		public static string GetMemberString<TIn, TOut>(this Expression<Func<TIn, TOut>> member)
		{
			if (member == null) throw new ArgumentNullException("member");

			var propertyRefExpr = member.Body;
			var memberExpr = propertyRefExpr as MemberExpression;

			if (memberExpr == null)
			{
				var unaryExpr = propertyRefExpr as UnaryExpression;
				if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
				{
					memberExpr = unaryExpr.Operand as MemberExpression;
					if (memberExpr != null) return memberExpr.Member.Name;
				}
			}
			else
			{
				//gets something line "m.Field1.Field2.Field3", from here we just remove the prefix "m."
				string body = member.Body.ToString();
				return body.Substring(body.IndexOf('.') + 1);
			}

			throw new ArgumentException("No property reference expression was found.", "member");
		}

		public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
		{
			StringBuilder sb = new StringBuilder();

			int previousIndex = 0;
			int index = str.IndexOf(oldValue, comparison);
			while (index != -1)
			{
				sb.Append(str.Substring(previousIndex, index - previousIndex));
				sb.Append(newValue);
				index += oldValue.Length;

				previousIndex = index;
				index = str.IndexOf(oldValue, index, comparison);
			}
			sb.Append(str.Substring(previousIndex));

			return sb.ToString();
		}

		public static byte[] ReadAllBytes(this Stream input)
		{
			if (input is MemoryStream)
				return ((MemoryStream)input).ToArray();

			byte[] buffer = new byte[16 * 1024];
			using (MemoryStream ms = new MemoryStream())
			{
				int read;
				while ((read = input.ReadAsync(buffer, 0, buffer.Length).Result) > 0)
				{
					ms.Write(buffer, 0, read);
				}
				return ms.ToArray();
			}
		}

		public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValueProvider)
		{
			return dictionary.TryGetValue(key, out var value) ? value : defaultValueProvider();
		}

		public static string ToBlobLiterals(this byte[] bytes)
		{
			return "0x" + BitConverter.ToString(bytes).Replace("-", string.Empty);
		}
	}
}