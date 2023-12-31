﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Tango.Identity
{
	internal static class Base32
	{
		private const string _base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

		public static string ToBase32(byte[] input)
		{
			StringBuilder sb = new StringBuilder();
			for (int offset = 0; offset < input.Length;)
			{
				byte a, b, c, d, e, f, g, h;
				int numCharsToOutput = GetNextGroup(input, ref offset, out a, out b, out c, out d, out e, out f, out g, out h);

				sb.Append((numCharsToOutput >= 1) ? _base32Chars[a] : '=');
				sb.Append((numCharsToOutput >= 2) ? _base32Chars[b] : '=');
				sb.Append((numCharsToOutput >= 3) ? _base32Chars[c] : '=');
				sb.Append((numCharsToOutput >= 4) ? _base32Chars[d] : '=');
				sb.Append((numCharsToOutput >= 5) ? _base32Chars[e] : '=');
				sb.Append((numCharsToOutput >= 6) ? _base32Chars[f] : '=');
				sb.Append((numCharsToOutput >= 7) ? _base32Chars[g] : '=');
				sb.Append((numCharsToOutput >= 8) ? _base32Chars[h] : '=');
			}

			return sb.ToString();
		}

		// returns the number of bytes that were output
		private static int GetNextGroup(byte[] input, ref int offset, out byte a, out byte b, out byte c, out byte d, out byte e, out byte f, out byte g, out byte h)
		{
			uint b1, b2, b3, b4, b5;

			int retVal;
			switch (input.Length - offset)
			{
				case 1: retVal = 2; break;
				case 2: retVal = 4; break;
				case 3: retVal = 5; break;
				case 4: retVal = 7; break;
				default: retVal = 8; break;
			}

			b1 = (offset < input.Length) ? input[offset++] : 0U;
			b2 = (offset < input.Length) ? input[offset++] : 0U;
			b3 = (offset < input.Length) ? input[offset++] : 0U;
			b4 = (offset < input.Length) ? input[offset++] : 0U;
			b5 = (offset < input.Length) ? input[offset++] : 0U;

			a = (byte)(b1 >> 3);
			b = (byte)(((b1 & 0x07) << 2) | (b2 >> 6));
			c = (byte)((b2 >> 1) & 0x1f);
			d = (byte)(((b2 & 0x01) << 4) | (b3 >> 4));
			e = (byte)(((b3 & 0x0f) << 1) | (b4 >> 7));
			f = (byte)((b4 >> 2) & 0x1f);
			g = (byte)(((b4 & 0x3) << 3) | (b5 >> 5));
			h = (byte)(b5 & 0x1f);

			return retVal;
		}
	}
}
