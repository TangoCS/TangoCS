using System;
using System.Collections;
using System.IO;
using ITSPDF = iTextSharp.text.pdf;
using BCX509 = Org.BouncyCastle.X509;
using GostCryptography.Cryptography;
using GostCryptography.Pkcs;

namespace Tango.PDF
{
	public static class SignPdf
	{
		public static byte[] Digest(byte[] pdf, byte[] cert, DateTime date, string reason = "Signing document", string location = "Минпромторг")
		{
			byte[] digestpdf = null;
			try
			{
				using (var memoryStream = new MemoryStream())
				{
					var reader = new ITSPDF.PdfReader(pdf);
					var st = ITSPDF.PdfStamper.CreateSignature(reader, memoryStream, '\0', null, true);
					var sap = st.SignatureAppearance;

					// Загружаем сертификат в объект iTextSharp
					var parser = new BCX509.X509CertificateParser();
					sap.SetCrypto(null, new[] { parser.ReadCertificate(cert) }, null, null);
                    sap.Reason = reason;
					sap.Location = location;
					sap.Acro6Layers = true;
					sap.SignDate = date;
					sap.Render = ITSPDF.PdfSignatureAppearance.SignatureRender.NameAndDescription;
					
					// Устанавливаем подходящий тип фильтра
					var filterName = new ITSPDF.PdfName("CryptoPro PDF");

					// Создаем подпись
					var sig = new ITSPDF.PdfSignature(filterName, ITSPDF.PdfName.ADBE_PKCS7_DETACHED);
					sig.Date = new ITSPDF.PdfDate(sap.SignDate);
					sig.Name = sap.CertChain[0].SubjectDN.ToString();
                    if (sap.Reason != null) sig.Reason = sap.Reason;
					if (sap.Location != null) sig.Location = sap.Location;

					sap.CryptoDictionary = sig;

					int intCSize = 16384;
					var hashtable = new Hashtable();
					hashtable[ITSPDF.PdfName.CONTENTS] = intCSize * 2 + 2;
					sap.PreClose(hashtable);

					using (Stream s = sap.RangeStream)
					{
						using (var hash = new Gost3411HashAlgorithm())
						{
							digestpdf = hash.ComputeHash(s);
						}
					}
				}
			}
			catch (Exception e)
			{
				throw;
			}
			return digestpdf;
		}

		public static byte[] Sign(byte[] pdf, byte[] sign, byte[] cert, DateTime date, string reason = "Signing document", string location = "Минпромторг")
		{
			byte[] newpdf = null;
			try
			{
				using (var memoryStream = new MemoryStream())
				{
					var reader = new ITSPDF.PdfReader(pdf);
					var st = ITSPDF.PdfStamper.CreateSignature(reader, memoryStream, '\0', null, true);
					var sap = st.SignatureAppearance;

					// Загружаем сертификат в объект iTextSharp
					var parser = new BCX509.X509CertificateParser();
					sap.SetCrypto(null, new[] { parser.ReadCertificate(cert) }, null, null);
					sap.Reason = reason;
					sap.Location = location;
					sap.Acro6Layers = true;
					sap.SignDate = date;
					sap.Render = ITSPDF.PdfSignatureAppearance.SignatureRender.NameAndDescription;

					// Устанавливаем подходящий тип фильтра
					var filterName = new ITSPDF.PdfName("CryptoPro PDF");

					// Создаем подпись
					var sig = new ITSPDF.PdfSignature(filterName, ITSPDF.PdfName.ADBE_PKCS7_DETACHED);
					sig.Date = new ITSPDF.PdfDate(sap.SignDate);
					sig.Name = sap.CertChain[0].SubjectDN.ToString();
					if (sap.Reason != null) sig.Reason = sap.Reason;
					if (sap.Location != null) sig.Location = sap.Location;

					sap.CryptoDictionary = sig;

					int intCSize = 16384;
					var hashtable = new Hashtable();
					hashtable[ITSPDF.PdfName.CONTENTS] = intCSize * 2 + 2;
					sap.PreClose(hashtable);

					// Помещаем подпись в документ
					byte[] outc = new byte[intCSize];
					var dic = new ITSPDF.PdfDictionary();
					Array.Copy(sign, 0, outc, 0, sign.Length);
					dic.Put(ITSPDF.PdfName.CONTENTS, new ITSPDF.PdfString(outc).SetHexWriting(true));
					sap.Close(dic);
					reader.Close();
					newpdf = memoryStream.ToArray();
				}
			}
			catch (Exception e)
			{
				throw;
				//return null;
			}
			return newpdf;
		}

		/// <summary>
		/// Подпись pdf на серверной стороне
		/// </summary>
		/// <param name="pdf"></param>
		/// <param name="reason"></param>
		/// <param name="location"></param>
		/// <returns></returns>
		public static byte[] Sign(byte[] pdf, string reason = "Signing document", string location = "Минпромторг")
		{
			byte[] newpdf = null;
			try
			{
				using (var memoryStream = new MemoryStream())
				{
					var reader = new ITSPDF.PdfReader(pdf);

					using (var alg = GostCryptoConfig.CreateGost3410AsymmetricAlgorithm())
					{
						var st = ITSPDF.PdfStamper.CreateSignature(reader, memoryStream, '\0', null, true);
						var sap = st.SignatureAppearance;

						// Загружаем сертификат в объект iTextSharp
						var parser = new BCX509.X509CertificateParser();
						sap.SetCrypto(null, new[] { parser.ReadCertificate(alg.ContainerCertificate.RawData) }, null, null);
						sap.Reason = reason;
						sap.Location = location;
						sap.Acro6Layers = true;
						sap.SignDate = DateTime.Now;
						sap.Render = ITSPDF.PdfSignatureAppearance.SignatureRender.NameAndDescription;

						// Устанавливаем подходящий тип фильтра
						var filterName = new ITSPDF.PdfName("CryptoPro PDF");

						// Создаем подпись
						var sig = new ITSPDF.PdfSignature(filterName, ITSPDF.PdfName.ADBE_PKCS7_DETACHED);
						sig.Date = new ITSPDF.PdfDate(sap.SignDate);
						sig.Name = sap.CertChain[0].SubjectDN.ToString();
						if (sap.Reason != null) sig.Reason = sap.Reason;
						if (sap.Location != null) sig.Location = sap.Location;

						sap.CryptoDictionary = sig;

						int intCSize = 16384;
						var hashtable = new Hashtable();
						hashtable[ITSPDF.PdfName.CONTENTS] = intCSize * 2 + 2;
						sap.PreClose(hashtable);

						byte[] contentInfo;
						using (Stream s = sap.RangeStream)
						{
							using (var ss = new MemoryStream())
							{
								int read = 0;
								byte[] buff = new byte[8192];
								while ((read = s.Read(buff, 0, 8192)) > 0)
								{
									ss.Write(buff, 0, read);
								}
								//contentInfo = new ContentInfo(ss.ToArray());
								contentInfo = ss.ToArray();
							}
						}
						// Вычисляем подпись
						/*SignedCms signedCms = new SignedCms(contentInfo, true);
						CmsSigner cmsSigner = new CmsSigner(GostCryptoConfig.KeyContainerParameters);
						signedCms.ComputeSignature(cmsSigner, true);
						byte[] sign = signedCms.Encode();*/
						byte[] sign = SignedPkcs7.ComputeSignature(contentInfo);

						// Помещаем подпись в документ
						byte[] outc = new byte[intCSize];

						var dic = new ITSPDF.PdfDictionary();
						Array.Copy(sign, 0, outc, 0, sign.Length);
						dic.Put(ITSPDF.PdfName.CONTENTS, new ITSPDF.PdfString(outc).SetHexWriting(true));

						sap.Close(dic);
					}
					reader.Close();
					newpdf = memoryStream.ToArray();
				}
			}
			catch (Exception e)
			{
				throw;
				//return null;
			}
			return newpdf;
		}

		public static bool Verify(byte[] pdf)
		{
			bool? verify = null;
			try
			{
				bool valid = true;
				var reader = new ITSPDF.PdfReader(pdf);

				// Получаем подписи из документа
				var af = reader.AcroFields;
				var names = af.GetSignatureNames();

				foreach (string name in names)
				{
					// Проверяем подпись
					var pk = af.VerifySignature(name);
					valid &= pk.Verify();

					// Проверим сертификат
					//var cert = new X509Certificate2(pk.SigningCertificate.GetEncoded());
					//verify &= cert.Verify();
					verify = valid;
				}
			}
			catch (Exception e)
			{
				return false;
			}
			return verify == true;
		}
	}
}
