using Microsoft.AspNetCore.DataProtection;
using System;
using System.IO;
using System.Text;
using Tango.Identity;

namespace Tango.AspNetCore
{
	public class AspNetTokenProvider : ITokenProvider
	{
		public AspNetTokenProvider(IDataProtectionProvider dataProtectionProvider)
		{
			Protector = dataProtectionProvider.CreateProtector("DataProtectorTokenProvider");
		}
		protected IDataProtector Protector { get; private set; }

		public string Generate(string purpose)
		{
			var ms = new MemoryStream();
			using (var writer = ms.CreateWriter())
			{
				writer.Write(DateTimeOffset.UtcNow);
				writer.Write(purpose ?? "");
			}
			var protectedBytes = Protector.Protect(ms.ToArray());
			return Convert.ToBase64String(protectedBytes);
		}

		public bool Validate(string purpose, string token)
		{
			var unprotectedData = Protector.Unprotect(Convert.FromBase64String(token));
			var ms = new MemoryStream(unprotectedData);
			using (var reader = ms.CreateReader())
			{
				var creationTime = reader.ReadDateTimeOffset();
				var expirationTime = creationTime + TimeSpan.FromMinutes(10);
				if (expirationTime < DateTimeOffset.UtcNow)
				{
					return false;
				}

				var purp = reader.ReadString();
				if (!string.Equals(purp, purpose))
				{
					return false;
				}

				return true;
			}
		}
	}

	internal static class StreamExtensions
	{
		internal static readonly Encoding DefaultEncoding = new UTF8Encoding(false, true);

		public static BinaryReader CreateReader(this Stream stream)
			=> new BinaryReader(stream, DefaultEncoding, true);

		public static BinaryWriter CreateWriter(this Stream stream)
			=> new BinaryWriter(stream, DefaultEncoding, true);

		public static DateTimeOffset ReadDateTimeOffset(this BinaryReader reader)
			=> new DateTimeOffset(reader.ReadInt64(), TimeSpan.Zero);

		public static void Write(this BinaryWriter writer, DateTimeOffset value)
		{
			writer.Write(value.UtcTicks);
		}
	}
}
