using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Tango.FileStorage
{
	public interface IStorageFile
	{
		string Name { get; set; }
		string Extension { get; set; }
		long Length { get; }
		DateTime LastModifiedDate { get; set; }

		byte[] ReadAllBytes();
		void WriteAllBytes(byte[] bytes);
		void Delete();
	}

	public interface IStorageFile<TKey> : IStorageFile
	{
		TKey ID { get; }
	}

	public static class IStorageFileExtensions
	{
		public static string ReadAllText(this IStorageFile file)
		{
			var bytes = file.ReadAllBytes();

			if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
				return Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);

			if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
				return Encoding.BigEndianUnicode.GetString(bytes, 2, bytes.Length - 2);

			if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
				return Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2);

			return Encoding.UTF8.GetString(bytes);
		}

		public static XDocument ReadAllXml(this IStorageFile file)
		{
			return XDocument.Parse(file.ReadAllText());
		}


		public static void WriteAllText(this IStorageFile file, string text, Encoding encoding = null)
		{
			if (encoding == null) encoding = Encoding.UTF8;
			using (var ms = new MemoryStream())
			{
				using (var br = new BinaryWriter(ms))
				{
					br.Write(encoding.GetPreamble());
					br.Write(encoding.GetBytes(text));
					br.Close();
					file.WriteAllBytes(ms.ToArray());
				}
			}
		}

		public static void WriteAllXml(this IStorageFile file, XDocument xml)
		{
			StringWriter sw = new StringWriter();
			xml.Save(sw);
			file.WriteAllText(sw.ToString());
		}
	}
}