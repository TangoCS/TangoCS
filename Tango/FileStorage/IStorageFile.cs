using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Tango.FileStorage
{
	public interface IStorageFile : IWithName
	{
		new string Name { get; set; }
		string Extension { get; set; }
		long Length { get; }
		DateTime LastModifiedDate { get; set; }

		byte[] ReadAllBytes();
        string WriteAllBytesSql(byte[] bytes);
        void WriteAllBytes(byte[] bytes);
		void Delete();
	}

	public interface IStorageFile<TKey> : IStorageFile, IWithKey<TKey>
	{
	}

	public static class IStorageFileExtensions
	{
        public static string ReadAllText(this IStorageFile file)
        {
            var b = file.ReadAllBytes();

            if (b.Length >= 4 && b[0] == 0x00 && b[1] == 0x00 && b[2] == 0xFE && b[3] == 0xFF) return Encoding.GetEncoding("utf-32BE").GetString(b, 4, b.Length - 4); // UTF-32, big-endian 
            if (b.Length >= 4 && b[0] == 0xFF && b[1] == 0xFE && b[2] == 0x00 && b[3] == 0x00) return Encoding.UTF32.GetString(b, 4, b.Length - 4); // UTF-32, little-endian
            if (b.Length >= 3 && b[0] == 0xEF && b[1] == 0xBB && b[2] == 0xBF) return Encoding.UTF8.GetString(b, 3, b.Length - 3); // UTF-8
            if (b.Length >= 3 && b[0] == 0x2b && b[1] == 0x2f && b[2] == 0x76) return Encoding.UTF7.GetString(b, 3, b.Length - 3); // UTF-7
            if (b.Length >= 2 && b[0] == 0xFE && b[1] == 0xFF) return Encoding.BigEndianUnicode.GetString(b, 2, b.Length - 2); // UTF-16, big-endian
            if (b.Length >= 2 && b[0] == 0xFF && b[1] == 0xFE) return Encoding.Unicode.GetString(b, 2, b.Length - 2); // UTF-16, little-endian

            Encoding encoding = null;
            var stream = new MemoryStream(b);
            using (StreamReader reader = new StreamReader(stream, Encoding.ASCII, true))
            {
                reader.Read();
                if (reader.CurrentEncoding != Encoding.ASCII)
                {
                    encoding = reader.CurrentEncoding;
                }
            }
            if (encoding != null)
                return encoding.GetString(b);

            stream = new MemoryStream(b);
            using (StreamReader reader = new StreamReader(stream, new UTF8Encoding(false, true)))
            {
                try
                {
                    reader.Read();
                    encoding = Encoding.UTF8;
                }
                catch { }
            }

            if (encoding != null)
                return encoding.GetString(b);

            var enc = Encoding.GetEncoding(Encoding.ASCII.CodePage, new EncoderExceptionFallback(), new DecoderExceptionFallback());
            try
            {
                var str = enc.GetString(b);
                return str;
            }
            catch { }

            if (encoding == null)
            {
                encoding = Encoding.GetEncoding(1251);
            }

            return encoding.GetString(b);
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