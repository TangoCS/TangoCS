using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nephrite.FileStorage
{
	public interface IStorageProvider
	{
		void SetData(IStorageFile file, byte[] bytes);
		byte[] GetData(IStorageFile file);
		void DeleteData(IStorageFile file);
		IStorageFile GetMetadata(IStorageFile file);
		IQueryable<IStorageFile> GetAllMetadata(IStorageFolder folder);
	}

	public static class IFileStorageProviderExtensions
	{
		public static string GetText(this IStorageProvider provider, IStorageFile file)
		{
			var bytes = provider.GetData(file);

			if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
				return Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);

			if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
				return Encoding.BigEndianUnicode.GetString(bytes, 2, bytes.Length - 2);

			if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
				return Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2);

			return Encoding.UTF8.GetString(bytes);
		}

		public static XDocument GetXml(this IStorageProvider provider, IStorageFile file)
		{
			return XDocument.Parse(provider.GetText(file));
		}

		public static void SetText(this IStorageProvider provider, IStorageFile file, string text, Encoding encoding = null)
		{
			if (encoding == null) encoding = Encoding.UTF8;
			using (var ms = new MemoryStream())
			{
				using (var br = new BinaryWriter(ms))
				{
					br.Write(encoding.GetPreamble());
					br.Write(encoding.GetBytes(text));
					br.Close();
					provider.SetData(file, ms.ToArray());
				}
			}
		}

		public static void SetXml(this IStorageProvider provider, IStorageFile file, XDocument document)
		{
			StringWriter sw = new StringWriter();
			document.Save(sw);
			provider.SetText(file, sw.ToString());
		}	
	}
}