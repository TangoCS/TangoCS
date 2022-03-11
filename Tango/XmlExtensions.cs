using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Tango
{
	/// <summary>
	/// Useful helpers for the XML parser.
	/// </summary>
	public static class XmlExtensions
	{
		/// <summary>
		/// Determines if the given character is a legal character for the public id field:
		/// http://www.w3.org/TR/REC-xml/#NT-PubidChar
		/// </summary>
		/// <param name="c">The character to examine.</param>
		/// <returns>The result of the test.</returns>
		public static Boolean IsPubidChar(this Char c)
		{
			return c.IsAlphanumericAscii() || c == Symbols.Minus || c == Symbols.SingleQuote || c == Symbols.Plus ||
				   c == Symbols.Comma || c == Symbols.Dot || c == Symbols.Solidus || c == Symbols.Colon ||
				   c == Symbols.QuestionMark || c == Symbols.Equality || c == Symbols.ExclamationMark || c == Symbols.Asterisk ||
				   c == Symbols.Num || c == Symbols.At || c == Symbols.Dollar || c == Symbols.Underscore ||
				   c == Symbols.RoundBracketOpen || c == Symbols.RoundBracketClose || c == Symbols.Semicolon || c == Symbols.Percent ||
				   c.IsSpaceCharacter();
		}

		/// <summary>
		/// Determines if the given character is a legal name start character for XML.
		/// http://www.w3.org/TR/REC-xml/#NT-NameStartChar
		/// </summary>
		/// <param name="c">The character to examine.</param>
		/// <returns>The result of the test.</returns>
		public static Boolean IsXmlNameStart(this Char c)
		{
			return c.IsLetter() || c == Symbols.Colon || c == Symbols.Underscore || c.IsInRange(0xC0, 0xD6) ||
				   c.IsInRange(0xD8, 0xF6) || c.IsInRange(0xF8, 0x2FF) || c.IsInRange(0x370, 0x37D) || c.IsInRange(0x37F, 0x1FFF) ||
				   c.IsInRange(0x200C, 0x200D) || c.IsInRange(0x2070, 0x218F) || c.IsInRange(0x2C00, 0x2FEF) ||
				   c.IsInRange(0x3001, 0xD7FF) || c.IsInRange(0xF900, 0xFDCF) || c.IsInRange(0xFDF0, 0xFFFD) ||
				   c.IsInRange(0x10000, 0xEFFFF);
		}

		/// <summary>
		/// Determines if the given character is a name character for XML.
		/// http://www.w3.org/TR/REC-xml/#NT-NameChar
		/// </summary>
		/// <param name="c">The character to examine.</param>
		/// <returns>The result of the test.</returns>
		public static Boolean IsXmlName(this Char c)
		{
			return c.IsXmlNameStart() || c.IsDigit() || c == Symbols.Minus || c == Symbols.Dot || c == 0xB7 ||
				   c.IsInRange(0x300, 0x36F) || c.IsInRange(0x203F, 0x2040);
		}

		/// <summary>
		/// Determines if the given string is a valid (local or qualified) name.
		/// </summary>
		/// <param name="str">The string to examine.</param>
		/// <returns>The result of the test.</returns>
		public static Boolean IsXmlName(this String str)
		{
			if (str.Length > 0 && str[0].IsXmlNameStart())
			{
				for (int i = 1; i < str.Length; i++)
				{
					if (!str[i].IsXmlName())
						return false;
				}

				return true;
			}

			return false;
		}

		/// <summary>
		/// Determines if the given string is a valid qualified name.
		/// </summary>
		/// <param name="str">The string to examine.</param>
		/// <returns>The result of the test.</returns>
		public static Boolean IsQualifiedName(this String str)
		{
			var colon = str.IndexOf(Symbols.Colon);

			if (colon == -1)
				return str.IsXmlName();

			if (colon > 0 && str[0].IsXmlNameStart())
			{
				for (int i = 1; i < colon; i++)
				{
					if (!str[i].IsXmlName())
						return false;
				}

				colon++;
			}

			if (str.Length > colon && str[colon++].IsXmlNameStart())
			{
				for (int i = colon; i < str.Length; i++)
				{
					if (str[i] == Symbols.Colon || !str[i].IsXmlName())
						return false;
				}

				return true;
			}

			return false;
		}

		/// <summary>
		/// Checks if the given char is a valid character.
		/// </summary>
		/// <param name="chr">The char to examine.</param>
		/// <returns>True if the char would indeed be valid.</returns>
		public static Boolean IsXmlChar(this Char chr)
		{
			return chr == 0x9 || chr == 0xA || chr == 0xD || (chr >= 0x20 && chr <= 0xD7FF) ||
					(chr >= 0xE000 && chr <= 0xFFFD);
		}

		/// <summary>
		/// Checks if the given integer would be a valid character.
		/// </summary>
		/// <param name="chr">The integer to examine.</param>
		/// <returns>True if the integer would indeed be valid.</returns>
		public static Boolean IsValidAsCharRef(this Int32 chr)
		{
			return chr == 0x9 || chr == 0xA || chr == 0xD || (chr >= 0x20 && chr <= 0xD7FF) ||
					(chr >= 0xE000 && chr <= 0xFFFD) || (chr >= 0x10000 && chr <= 0x10FFFF);
		}
	}

    /// <summary>
    /// Contains useful information from the specification.
    /// </summary>
    static class Symbols
    {
        /// <summary>
        /// The end of file marker (Char.MaxValue).
        /// </summary>
        public const Char EndOfFile = Char.MaxValue;

        /// <summary>
        /// The tilde character ( ~ ).
        /// </summary>
        public const Char Tilde = (Char)0x7e;

        /// <summary>
        /// The pipe character ( | ).
        /// </summary>
        public const Char Pipe = (Char)0x7c;

        /// <summary>
        /// The null character.
        /// </summary>
        public const Char Null = (Char)0x0;

        /// <summary>
        /// The ampersand character ( &amp; ).
        /// </summary>
        public const Char Ampersand = (Char)0x26;

        /// <summary>
        /// The number sign character ( # ).
        /// </summary>
        public const Char Num = (Char)0x23;

        /// <summary>
        /// The dollar sign character ( $ ).
        /// </summary>
        public const Char Dollar = (Char)0x24;

        /// <summary>
        /// The semicolon sign ( ; ).
        /// </summary>
        public const Char Semicolon = (Char)0x3b;

        /// <summary>
        /// The asterisk character ( * ).
        /// </summary>
        public const Char Asterisk = (Char)0x2a;

        /// <summary>
        /// The equals sign ( = ).
        /// </summary>
        public const Char Equality = (Char)0x3d;

        /// <summary>
        /// The plus sign ( + ).
        /// </summary>
        public const Char Plus = (Char)0x2b;

        /// <summary>
        /// The dash ( hypen minus, - ) character.
        /// </summary>
        public const Char Minus = (Char)0x2d;

        /// <summary>
        /// The comma character ( , ).
        /// </summary>
        public const Char Comma = (Char)0x2c;

        /// <summary>
        /// The full stop ( . ).
        /// </summary>
        public const Char Dot = (Char)0x2e;

        /// <summary>
        /// The circumflex accent ( ^ ) character.
        /// </summary>
        public const Char Accent = (Char)0x5e;

        /// <summary>
        /// The commercial at ( @ ) character.
        /// </summary>
        public const Char At = (Char)0x40;

        /// <summary>
        /// The opening angle bracket ( LESS-THAN-SIGN ).
        /// </summary>
        public const Char LessThan = (Char)0x3c;

        /// <summary>
        /// The closing angle bracket ( GREATER-THAN-SIGN ).
        /// </summary>
        public const Char GreaterThan = (Char)0x3e;

        /// <summary>
        /// The single quote / quotation mark ( ' ).
        /// </summary>
        public const Char SingleQuote = (Char)0x27;

        /// <summary>
        /// The (double) quotation mark ( " ).
        /// </summary>
        public const Char DoubleQuote = (Char)0x22;

        /// <summary>
        /// The (curved) quotation mark ( ` ).
        /// </summary>
        public const Char CurvedQuote = (Char)0x60;

        /// <summary>
        /// The question mark ( ? ).
        /// </summary>
        public const Char QuestionMark = (Char)0x3f;

        /// <summary>
        /// The tab character.
        /// </summary>
        public const Char Tab = (Char)0x09;

        /// <summary>
        /// The line feed character.
        /// </summary>
        public const Char LineFeed = (Char)0x0a;

        /// <summary>
        /// The carriage return character.
        /// </summary>
        public const Char CarriageReturn = (Char)0x0d;

        /// <summary>
        /// The form feed character.
        /// </summary>
        public const Char FormFeed = (Char)0x0c;

        /// <summary>
        /// The space character.
        /// </summary>
        public const Char Space = (Char)0x20;

        /// <summary>
        /// The slash (solidus, /) character.
        /// </summary>
        public const Char Solidus = (Char)0x2f;

        /// <summary>
        /// The no break space character.
        /// </summary>
        public const Char NoBreakSpace = (Char)0xa0;

        /// <summary>
        /// The backslash ( reverse-solidus, \ ) character.
        /// </summary>
        public const Char ReverseSolidus = (Char)0x5c;

        /// <summary>
        /// The colon ( : ) character.
        /// </summary>
        public const Char Colon = (Char)0x3a;

        /// <summary>
        /// The exclamation mark ( ! ) character.
        /// </summary>
        public const Char ExclamationMark = (Char)0x21;

        /// <summary>
        /// The replacement character in case of errors.
        /// </summary>
        public const Char Replacement = (Char)0xfffd;

        /// <summary>
        /// The low line ( _ ) character.
        /// </summary>
        public const Char Underscore = (Char)0x5f;

        /// <summary>
        /// The round bracket open ( ( ) character.
        /// </summary>
        public const Char RoundBracketOpen = (Char)0x28;

        /// <summary>
        /// The round bracket close ( ) ) character.
        /// </summary>
        public const Char RoundBracketClose = (Char)0x29;

        /// <summary>
        /// The square bracket open ( [ ) character.
        /// </summary>
        public const Char SquareBracketOpen = (Char)0x5b;

        /// <summary>
        /// The square bracket close ( ] ) character.
        /// </summary>
		public const Char SquareBracketClose = (Char)0x5d;

        /// <summary>
        /// The curly bracket open ( { ) character.
        /// </summary>
        public const Char CurlyBracketOpen = (Char)0x7b;

        /// <summary>
        /// The curly bracket close ( } ) character.
        /// </summary>
        public const Char CurlyBracketClose = (Char)0x7d;

        /// <summary>
        /// The percent ( % ) character.
        /// </summary>
        public const Char Percent = (Char)0x25;

        /// <summary>
        /// The maximum allowed codepoint (defined in Unicode).
        /// </summary>
        public const Int32 MaximumCodepoint = 0x10FFFF;

        /// <summary>
        /// A list of available punycode character mappings.
        /// </summary>
        public static Dictionary<Char, Char> Punycode = new Dictionary<Char, Char>
        {
            { '。', '.' },
            { '．', '.' },
            { 'Ｇ', 'g' },
            { 'ｏ', 'o' },
            { 'ｃ', 'c' },
            { 'Ｘ', 'x' },
            { '０', '0' },
            { '１', '1' },
            { '２', '2' },
            { '５', '5' },
        };

        /// <summary>
        /// A list of possible newline characters or character combinations.
        /// </summary>
        public static readonly String[] NewLines = new[] { "\r\n", "\r", "\n" };
    }

    /// <summary>
    /// Useful methods for chars.
    /// </summary>
    static class CharExtensions
    {
        /// <summary>
        /// Converts a given character from the hex representation (0-9A-Fa-f)
        /// to an integer.
        /// </summary>
        /// <param name="c">The character to convert.</param>
        /// <returns>
        /// The integer value or undefined behavior if invalid.
        /// </returns>
        public static Int32 FromHex(this Char c)
        {
            return c.IsDigit() ? c - 0x30 : c - (c.IsLowercaseAscii() ? 0x57 : 0x37);
        }

        /// <summary>
        /// Transforms the given number to a hexadecimal string.
        /// </summary>
        /// <param name="num">The number (0-255).</param>
        /// <returns>A 2 digit upper case hexadecimal string.</returns>
        public static String ToHex(this Byte num)
        {
            var chrs = new Char[2];
            var rem = num >> 4;
            chrs[0] = (Char)(rem + (rem < 10 ? 48 : 55));
            rem = num - 16 * rem;
            chrs[1] = (Char)(rem + (rem < 10 ? 48 : 55));
            return new String(chrs);
        }

        /// <summary>
        /// Transforms the given character to a hexadecimal string.
        /// </summary>
        /// <param name="character">The single character.</param>
        /// <returns>A minimal digit lower case hexadecimal string.</returns>
        public static String ToHex(this Char character)
        {
            return ((Int32)character).ToString("x");
        }

        /// <summary>
        /// Determines if the given character is in the given range.
        /// </summary>
        /// <param name="c">The character to examine.</param>
        /// <param name="lower">The lower bound of the range.</param>
        /// <param name="upper">The upper bound of the range.</param>
        /// <returns>The result of the test.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsInRange(this Char c, Int32 lower, Int32 upper)
        {
            return c >= lower && c <= upper;
        }

        /// <summary>
        /// Determines if the given character is allowed as-it-is in queries.
        /// </summary>
        /// <param name="c">The character to examine.</param>
        /// <returns>The result of the test.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsNormalQueryCharacter(this Char c)
        {
            return c.IsInRange(0x21, 0x7e) && c != Symbols.DoubleQuote &&
                c != Symbols.CurvedQuote && c != Symbols.Num &&
                c != Symbols.LessThan && c != Symbols.GreaterThan;
        }

        /// <summary>
        /// Determines if the given character is allowed as-it-is in paths.
        /// </summary>
        /// <param name="c">The character to examine.</param>
        /// <returns>The result of the test.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsNormalPathCharacter(this Char c)
        {
            return c.IsInRange(0x20, 0x7e) && c != Symbols.DoubleQuote &&
                c != Symbols.CurvedQuote && c != Symbols.Num &&
                c != Symbols.LessThan && c != Symbols.GreaterThan &&
                c != Symbols.Space && c != Symbols.QuestionMark;
        }

        /// <summary>
        /// Determines if the given character is a uppercase character (A-Z) as
        /// specified here:
        /// http://www.whatwg.org/specs/web-apps/current-work/multipage/common-microsyntaxes.html#uppercase-ascii-letters
        /// </summary>
        /// <param name="c">The character to examine.</param>
        /// <returns>The result of the test.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsUppercaseAscii(this Char c)
        {
            return c >= 0x41 && c <= 0x5a;
        }

        /// <summary>
        /// Determines if the given character is a lowercase character (a-z) as
        /// specified here:
        /// http://www.whatwg.org/specs/web-apps/current-work/multipage/common-microsyntaxes.html#lowercase-ascii-letters
        /// </summary>
        /// <param name="c">The character to examine.</param>
        /// <returns>The result of the test.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsLowercaseAscii(this Char c)
        {
            return c >= 0x61 && c <= 0x7a;
        }

        /// <summary>
        /// Determines if the given character is a alphanumeric character
        /// (0-9a-zA-z) as specified here:
        /// http://www.whatwg.org/specs/web-apps/current-work/multipage/common-microsyntaxes.html#alphanumeric-ascii-characters
        /// </summary>
        /// <param name="c">The character to examine.</param>
        /// <returns>The result of the test.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsAlphanumericAscii(this Char c)
        {
            return c.IsDigit() || c.IsUppercaseAscii() || c.IsLowercaseAscii();
        }

        /// <summary>
        /// Determines if the given character is a hexadecimal (0-9a-fA-F) as
        /// specified here:
        /// http://www.whatwg.org/specs/web-apps/current-work/multipage/common-microsyntaxes.html#ascii-hex-digits
        /// </summary>
        /// <param name="c">The character to examine.</param>
        /// <returns>The result of the test.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsHex(this Char c)
        {
            return c.IsDigit() || (c >= 0x41 && c <= 0x46) || (c >= 0x61 && c <= 0x66);
        }

        /// <summary>
        /// Gets if the character is actually a non-ascii character.
        /// </summary>
        /// <param name="c">The character to examine.</param>
        /// <returns>The result of the test.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsNonAscii(this Char c)
        {
            return c != Symbols.EndOfFile && c >= 0x80;
        }

        /// <summary>
        /// Gets if the character is actually a non-printable (special)
        /// character.
        /// </summary>
        /// <param name="c">The character to examine.</param>
        /// <returns>The result of the test.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsNonPrintable(this Char c)
        {
            return (c >= 0x0 && c <= 0x8) || (c >= 0xe && c <= 0x1f) || (c >= 0x7f && c <= 0x9f);
        }

        /// <summary>
        /// Gets if the character is actually a (A-Z,a-z) letter.
        /// </summary>
        /// <param name="c">The character to examine.</param>
        /// <returns>The result of the test.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsLetter(this Char c)
        {
            return IsUppercaseAscii(c) || IsLowercaseAscii(c);
        }

        /// <summary>
        /// Gets if the character is actually a name character.
        /// </summary>
        /// <param name="c">The character to examine.</param>
        /// <returns>The result of the test.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsName(this Char c)
        {
            return c.IsNonAscii() || c.IsLetter() || c == Symbols.Underscore || c == Symbols.Minus || c.IsDigit();
        }

        /// <summary>
        /// Determines if the given character is a valid character for starting
        /// an identifier.
        /// </summary>
        /// <param name="c">The character to examine.</param>
        /// <returns>The result of the test.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsNameStart(this Char c)
        {
            return c.IsNonAscii() || c.IsUppercaseAscii() || c.IsLowercaseAscii() || c == Symbols.Underscore;
        }

        /// <summary>
        /// Determines if the given character is a line break character as
        /// specified here:
        /// http://www.w3.org/TR/html401/struct/text.html#h-9.3.2
        /// </summary>
        /// <param name="c">The character to examine.</param>
        /// <returns>The result of the test.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsLineBreak(this Char c)
        {
            return c == Symbols.LineFeed || c == Symbols.CarriageReturn;
        }

        /// <summary>
        /// Determines if the given character is a space character as specified
        /// here:
        /// http://www.whatwg.org/specs/web-apps/current-work/multipage/common-microsyntaxes.html#space-character
        /// </summary>
        /// <param name="c">The character to examine.</param>
        /// <returns>The result of the test.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsSpaceCharacter(this Char c)
        {
            return c == Symbols.Space || c == Symbols.Tab || c == Symbols.LineFeed || c == Symbols.CarriageReturn || c == Symbols.FormFeed;
        }

        /// <summary>
        /// Determines if the given character is a white-space character as
        /// specified here:
        /// http://www.whatwg.org/specs/web-apps/current-work/multipage/common-microsyntaxes.html#white_space
        /// </summary>
        /// <param name="c">The character to examine.</param>
        /// <returns>The result of the test.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsWhiteSpaceCharacter(this Char c)
        {
            return c.IsInRange(0x0009, 0x000d) || c == 0x0020 || c == 0x0085 || c == 0x00a0 ||
                    c == 0x1680 || c == 0x180e || c.IsInRange(0x2000, 0x200a) || c == 0x2028 ||
                    c == 0x2029 || c == 0x202f || c == 0x205f || c == 0x3000;
        }

        /// <summary>
        /// Determines if the given character is a digit (0-9) as specified
        /// here:
        /// http://www.whatwg.org/specs/web-apps/current-work/multipage/common-microsyntaxes.html#ascii-digits
        /// </summary>
        /// <param name="c">The character to examine.</param>
        /// <returns>The result of the test.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsDigit(this Char c)
        {
            return c >= 0x30 && c <= 0x39;
        }

        /// <summary>
        /// Determines if the given character is a valid url code point as specified here:
        /// http://url.spec.whatwg.org/#url-code-points
        /// </summary>
        /// <param name="c">The character to examine.</param>
        /// <returns>The result of the test.</returns>
        public static Boolean IsUrlCodePoint(this Char c)
        {
            return c.IsAlphanumericAscii() || c == Symbols.ExclamationMark || c == Symbols.Dollar || c == Symbols.Ampersand ||
                   c == Symbols.SingleQuote || c == Symbols.RoundBracketOpen || c == Symbols.RoundBracketClose ||
                   c == Symbols.Asterisk || c == Symbols.Plus || c == Symbols.Minus || c == Symbols.Comma ||
                   c == Symbols.Dot || c == Symbols.Solidus || c == Symbols.Colon || c == Symbols.Semicolon ||
                   c == Symbols.Equality || c == Symbols.QuestionMark || c == Symbols.At || c == Symbols.Underscore ||
                   c == Symbols.Tilde || c.IsInRange(0xa0, 0xd7ff) || c.IsInRange(0xe000, 0xfdcf) || c.IsInRange(0xfdf0, 0xfffd) ||
                   c.IsInRange(0x10000, 0x1FFFD) || c.IsInRange(0x20000, 0x2fffd) || c.IsInRange(0x30000, 0x3fffd) || c.IsInRange(0x40000, 0x4fffd) ||
                   c.IsInRange(0x50000, 0x5fffd) || c.IsInRange(0x60000, 0x6fffd) || c.IsInRange(0x70000, 0x7fffd) || c.IsInRange(0x80000, 0x8fffd) ||
                   c.IsInRange(0x90000, 0x9fffd) || c.IsInRange(0xa0000, 0xafffd) || c.IsInRange(0xb0000, 0xbfffd) || c.IsInRange(0xc0000, 0xcfffd) ||
                   c.IsInRange(0xd0000, 0xdfffd) || c.IsInRange(0xe0000, 0xefffd) || c.IsInRange(0xf0000, 0xffffd) || c.IsInRange(0x100000, 0x10fffd);
        }

        /// <summary>
        /// Determines if the given character is invalid, i.e. zero, above the
        /// max. codepoint or in an invalid range.
        /// </summary>
        /// <param name="c">The character to examine.</param>
        /// <returns>The result of the test.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsInvalid(this Int32 c)
        {
            return c == 0 || c > Symbols.MaximumCodepoint || (c > 0xD800 && c < 0xDFFF);
        }

        /// <summary>
        /// Determines if the given character is one of the two others.
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <param name="a">The first option.</param>
        /// <param name="b">The second option.</param>
        /// <returns>The result of the test.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static Boolean IsOneOf(this Char c, Char a, Char b)
        //{
        //    return a == c || b == c;
        //}

        /// <summary>
        /// Determines if the given character is one of the three others.
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <param name="o1">The first option.</param>
        /// <param name="o2">The second option.</param>
        /// <param name="o3">The third option.</param>
        /// <returns>The result of the test.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static Boolean IsOneOf(this Char c, Char o1, Char o2, Char o3)
        //{
        //    return c == o1 || c == o2 || c == o3;
        //}

        /// <summary>
        /// Determines if the given character is one of the four others.
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <param name="o1">The first option.</param>
        /// <param name="o2">The second option.</param>
        /// <param name="o3">The third option.</param>
        /// <param name="o4">The fourth option.</param>
        /// <returns>The result of the test.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static Boolean IsOneOf(this Char c, Char o1, Char o2, Char o3, Char o4)
        //{
        //    return c == o1 || c == o2 || c == o3 || c == o4;
        //}
    }
}
