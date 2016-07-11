using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace Tango.MSOffice
{
	public static class PDFConverter
	{
		static string[] wordExtensions = { "doc", "dot", "docm", "docx", "dotx", "dotm", "htm", "html", "rtf", "mht", "odf", "txt", "wpd", "wps"};
		static string[] excelExtensions = { "xl", "xls", "xlsx", "xlsm", "xlt", "ods", "csv", "dbf", "xlw" };
		static string[] powerPointExtensions = { "ppt", "pptx", "odp", "pptm", "pps", "ppsx", "ppsm" };
		static string[] imageExtensions = { "emf", "wmf", "jpg", "jpeg", "jfif", "jpe", "png", "bmp", "gif", "tif", "tiff" };

		public static string[] GetSupportedExtensions()
		{
			return wordExtensions.Union(excelExtensions).Union(powerPointExtensions).Union(imageExtensions).OrderBy(o => o).ToArray();
		}

		public static byte[] ConvertToPDF(string filename)
		{
			return ConvertToPDF(filename, false);
		}

		public static string GetTempFileName()
		{
			string path;
			do
			{
				path = Path.Combine(Environment.GetEnvironmentVariable("TEMP", EnvironmentVariableTarget.Machine), (new Random()).Next().ToString() + ".tmp");
			} while (File.Exists(path));
			return path;
		}

		public static byte[] ConvertToPDF(string filename, bool addPagesToWordDoc)
		{
			string fn = GetTempFileName();
			string ext = Path.GetExtension(filename).Replace(".", "").ToLower();
			Process pc = new Process();
			if (wordExtensions.Contains(ext))
			{
				pc.StartInfo = new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PdfConvert/PdfConverter.exe"), "word \"" + filename + "\" \"" + fn + "\" " + (addPagesToWordDoc ? "addpages" : "none"));
				
				using (var word = new WordApplication())
				{
					using (var doc = word.Open(filename))
					{
						if (addPagesToWordDoc)
							doc.AddPageNumbers();
						doc.ExportToPDF(fn);
					}
				}
			}
			else if (excelExtensions.Contains(ext))
			{
				pc.StartInfo = new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PdfConvert/PdfConverter.exe"), "excel \"" + filename + "\" \"" + fn + "\" " + (addPagesToWordDoc ? "addpages" : "none"));
				
				using (var excel = new ExcelApplication())
				{
					using (var doc = excel.Open(filename))
						doc.ExportToPDF(fn);
				}
				fn = fn + ".pdf";
				//Thread.Sleep(5000);
			}
			else if (powerPointExtensions.Contains(ext))
			{
				pc.StartInfo = new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PdfConvert/PdfConverter.exe"), "powerpoint \"" + filename + "\" \"" + fn + "\" " + (addPagesToWordDoc ? "addpages" : "none"));
				
				using (var pp = new PowerPointApplication())
				{
					using (var doc = pp.Open(filename))
						doc.ExportToPDF(fn);
				}
				
			}
			else if (imageExtensions.Contains(ext))
			{
				pc.StartInfo = new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PdfConvert/PdfConverter.exe"), "word \"" + filename + "\" \"" + fn + "\" " + (addPagesToWordDoc ? "addpages" : "none"));
				
				using (var word = new WordApplication())
				{
					using (var doc = word.Create())
					{
						doc.InsertPicture(filename);
						doc.ExportToPDF(fn);
					}
				}
				 
			}
			else
				throw new Exception("Формат " + ext + " не поддерживается для экспорта в PDF");
			//pc.Start();
			//pc.WaitForExit();
			//if (pc.ExitCode == -1)
			//	throw new Exception("Ошибка при конвертации в PDF (см. " + Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PdfConvert/PdfConverter.log") + ")");

			var bytes = File.ReadAllBytes(fn);
			File.Delete(fn);
			return bytes;
		}
	}
}
