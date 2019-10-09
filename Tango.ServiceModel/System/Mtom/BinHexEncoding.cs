//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
namespace System.Text
{
	using System.Runtime;
	using System.Security;

	class BinHexEncoding : Encoding
	{
		static byte[] char2val = new byte[128]
		{
                /*    0-15 */ 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                /*   16-31 */ 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                /*   32-47 */ 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                /*   48-63 */ 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                /*   64-79 */ 0xFF, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                /*   80-95 */ 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                /*  96-111 */ 0xFF, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                /* 112-127 */ 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
		};

		static string val2char = "0123456789ABCDEF";


		public override int GetMaxByteCount(int charCount)
		{
			if (charCount < 0)
				throw new ArgumentOutOfRangeException("charCount");
			if ((charCount % 2) != 0)
				throw new FormatException("XmlInvalidBinHexLength");
			return charCount / 2;
		}

		public override int GetByteCount(char[] chars, int index, int count)
		{
			return GetMaxByteCount(count);
		}

		[Fx.Tag.SecurityNote(Critical = "Contains unsafe code.",
			Safe = "Unsafe code is effectively encapsulated, all inputs are validated.")]
		[SecuritySafeCritical]
		unsafe public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			if (chars == null)
				throw new ArgumentNullException("chars");
			if (charIndex < 0)
				throw new ArgumentOutOfRangeException("ValueMustBeNonNegative");
			if (charIndex > chars.Length)
				throw new ArgumentOutOfRangeException("OffsetExceedsBufferSize");
			if (charCount < 0)
				throw new ArgumentOutOfRangeException("ValueMustBeNonNegative");
			if (charCount > chars.Length - charIndex)
				throw new ArgumentOutOfRangeException("SizeExceedsRemainingBufferSpace");
			if (bytes == null)
				throw new ArgumentNullException("bytes");
			if (byteIndex < 0)
				throw new ArgumentOutOfRangeException("ValueMustBeNonNegative");
			if (byteIndex > bytes.Length)
				throw new ArgumentOutOfRangeException("OffsetExceedsBufferSize");
			int byteCount = GetByteCount(chars, charIndex, charCount);
			if (byteCount < 0 || byteCount > bytes.Length - byteIndex)
				throw new ArgumentException("XmlArrayTooSmall");
			if (charCount > 0)
			{
				fixed (byte* _char2val = char2val)
				{
					fixed (byte* _bytes = &bytes[byteIndex])
					{
						fixed (char* _chars = &chars[charIndex])
						{
							char* pch = _chars;
							char* pchMax = _chars + charCount;
							byte* pb = _bytes;
							while (pch < pchMax)
							{
								char pch0 = pch[0];
								char pch1 = pch[1];
								if ((pch0 | pch1) >= 128)
									throw new FormatException("XmlInvalidBinHexSequence");
								byte d1 = _char2val[pch0];
								byte d2 = _char2val[pch1];
								if ((d1 | d2) == 0xFF)
									throw new FormatException("XmlInvalidBinHexSequence");
								pb[0] = (byte)((d1 << 4) + d2);
								pch += 2;
								pb++;
							}
						}
					}
				}
			}
			return byteCount;
		}

		public override int GetMaxCharCount(int byteCount)
		{
			if (byteCount < 0 || byteCount > int.MaxValue / 2)
				throw new ArgumentOutOfRangeException("ValueMustBeInRange");
			return byteCount * 2;
		}

		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			return GetMaxCharCount(count);
		}

		[Fx.Tag.SecurityNote(Critical = "Contains unsafe code.",
			Safe = "Unsafe code is effectively encapsulated, all inputs are validated.")]
		[SecuritySafeCritical]
		unsafe public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			if (bytes == null)
				throw new ArgumentNullException("bytes");
			if (byteIndex < 0)
				throw new ArgumentOutOfRangeException("ValueMustBeNonNegative");
			if (byteIndex > bytes.Length)
				throw new ArgumentOutOfRangeException("OffsetExceedsBufferSize");
			if (byteCount < 0)
				throw new ArgumentOutOfRangeException("ValueMustBeNonNegative");
			if (byteCount > bytes.Length - byteIndex)
				throw new ArgumentOutOfRangeException("SizeExceedsRemainingBufferSpace");
			int charCount = GetCharCount(bytes, byteIndex, byteCount);
			if (chars == null)
				throw new ArgumentNullException("chars");
			if (charIndex < 0)
				throw new ArgumentOutOfRangeException("ValueMustBeNonNegative");
			if (charIndex > chars.Length)
				throw new ArgumentOutOfRangeException("OffsetExceedsBufferSize");
			if (charCount < 0 || charCount > chars.Length - charIndex)
				throw new ArgumentException("XmlArrayTooSmall");
			if (byteCount > 0)
			{
				fixed (char* _val2char = val2char)
				{
					fixed (byte* _bytes = &bytes[byteIndex])
					{
						fixed (char* _chars = &chars[charIndex])
						{
							char* pch = _chars;
							byte* pb = _bytes;
							byte* pbMax = _bytes + byteCount;
							while (pb < pbMax)
							{
								pch[0] = _val2char[pb[0] >> 4];
								pch[1] = _val2char[pb[0] & 0x0F];
								pb++;
								pch += 2;
							}
						}
					}
				}
			}
			return charCount;
		}
	}
}