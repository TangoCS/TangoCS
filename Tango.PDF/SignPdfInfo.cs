using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using iTextSharp.text.pdf;
using GostCryptography.Cryptography;
using GostCryptography.X509Certificates;

namespace Tango.PDF
{
	public static class SignPdfInfo
	{
		public static byte[] GetSignature(byte[] pdf, out byte[] hash, int numbersignature = 0)
		{
			byte[] bytes = null;
			hash = null;
			try
			{
				var reader = new PdfReader(pdf);

				// Получаем подпись из документа
				var af = reader.AcroFields;
				var signs = af.GetSignatureNames();

				if (signs.Count == 0) return bytes;

				string signame = signs[numbersignature].ToString();
				var dic = af.GetSignatureDictionary(signame);
				var cont = dic.GetAsString(PdfName.CONTENTS);
				bytes = cont.GetOriginalBytes();

				byte[] data;
				using (var s = af.ExtractRevision(signame))
				{
					using (var ms = new MemoryStream())
					{
						int read = 0;
						byte[] buff = new byte[8192];
						while ((read = s.Read(buff, 0, 8192)) > 0)
						{
							ms.Write(buff, 0, read);
						}
						data = ms.ToArray();
					}
				}
				var str = data.GetString();
				int startindex = str.IndexOf(signame);

				int start = str.IndexOf("/Contents <", startindex) + 10;
				if (start < 0) start = str.IndexOf("/Contents<", startindex) + 9;
				int end = str.IndexOf(">", start) + 1;

				if (start < 0 || start >= end) return bytes;

				str = str.Remove(start, end - start);
				data = str.GetBytes();

				using (var alg = new Gost3411HashAlgorithm())
				{
					hash = alg.ComputeHash(data);
				}
			}
			catch (Exception e)
			{
				throw;
			}
			return bytes;
		}

		public static int GetNumberSign(byte[] pdf)
		{
			int number = 0;
			try
			{
				var reader = new PdfReader(pdf);

				// Получаем подписи из документа
				var af = reader.AcroFields;
				foreach (string name in af.GetSignatureNames())
				{
					number++;
				}
			}
			catch (Exception e)
			{
				return -1;
			}
			return number;
		}

		public static X509Certificate GetCertificate(byte[] pdf, int numbersignature)
		{
			X509Certificate cert = null;
			if (pdf == null) return null;
			try
			{
				var reader = new PdfReader(pdf);

				// Получаем подписи из документа
				var af = reader.AcroFields;
				var signs = af.GetSignatureNames();
				if (signs.Count == 0) return null;
				string signame = signs[numbersignature].ToString();

				var p = af.VerifySignature(signame);
				cert = new X509Certificate(p.SigningCertificate.GetEncoded());
			}
			catch (Exception e)
			{
				return null;
			}
			return cert;
		}

		public static X509Certificate[] GetCertificate(byte[] pdf)
		{
			var certs = new List<X509Certificate>();
			if (pdf == null) return null;
			try
			{
				var reader = new PdfReader(pdf);

				// Получаем подписи из документа
				var af = reader.AcroFields;
				var signs = af.GetSignatureNames();
				if (signs.Count == 0) return certs.ToArray();

				foreach (string signame in signs)
				{
					var p = af.VerifySignature(signame);
					var cert = new X509Certificate(p.SigningCertificate.GetEncoded());
					certs.Add(cert);
				}
			}
			catch (Exception e)
			{
				return null;
			}
			return certs.ToArray();
		}

		public static int PageCount(byte[] pdf)
		{
			if (pdf == null)
				return 0;
			var pdfreader = new PdfReader(pdf);
			return pdfreader.NumberOfPages;
		}
	}

	public static class ConvertExtentions
	{
		public static byte[] GetBytes(this string bytes)
		{
			var encoder = new byte[bytes.Length];
			for (int i = 0; i < bytes.Length; i++)
			{
				encoder[i] = (byte)bytes[i];
			}
			return encoder;
		}

		public static byte[] GetBytes(this string bytes, int start, int size)
		{
			var encoder = new byte[size];
			int count = start + size;
			for (int i = 0; i < count; i++)
			{
				encoder[i] = (byte)bytes[i];
			}
			return encoder;
		}

		public static string GetString(this byte[] bytes)
		{
			var encoder = new StringBuilder();
			foreach (byte t in bytes)
			{
				encoder.Append((char)t);
			}
			return encoder.ToString();
		}

		public static string GetString(this byte[] bytes, int start, int size)
		{
			var encoder = new StringBuilder();
			int count = start + size;
			for (int i = start; i < count; i++)
			{
				encoder.Append((char)bytes[i]);
			}
			return encoder.ToString();
		}
	}
}
