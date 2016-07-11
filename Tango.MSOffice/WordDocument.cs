using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Word;

namespace Tango.MSOffice
{
	public class WordDocument : IDisposable
	{
		List<string> fileNames = new List<string>();
		Document doc;

		internal WordDocument(Document document, string fileName)
		{
			if (document == null)
				throw new Exception("Ошибка открытия документа " + fileName + @". При работе в Windows Server 2008 проверить наличие каталога C:\Windows\SysWOW64\config\systemprofile\Desktop.");
			doc = document;
			if (fileName != null)
				fileNames.Add(fileName);
		}

		public void Dispose()
		{
			if (doc != null)
			{
				object saveChanges = false;
				object m = Type.Missing;
				doc.Close(ref saveChanges, ref m, ref m);
				foreach (string openFileName in fileNames)
					if (File.Exists(openFileName))
						File.Delete(openFileName);
			}
		}

		/// <summary>
		/// Сохранить в файл
		/// </summary>
		/// <param name="fileName">Имя файла</param>
		public void Save(string fileName)
		{
			object fn = fileName;
			object m = Type.Missing;
			doc.SaveAs(ref fn, ref m, ref m, ref m, ref m, ref m, ref m, ref m,
						ref m, ref m, ref m, ref m, ref m, ref m, ref m, ref m);
		}
		public void SaveAs(string fileName, WdSaveFormat format)
		{
			object fn = fileName;
			object m = Type.Missing;
			object f = format;
			doc.SaveAs(ref fn, ref f, ref m, ref m, ref m, ref m, ref m, ref m,
						ref m, ref m, ref m, ref m, ref m, ref m, ref m, ref m);
		}

		/// <summary>
		/// Получить данные документа
		/// </summary>
		/// <returns></returns>
		public byte[] GetData()
		{
			string fn = Path.GetTempFileName();
			Save(fn);
			fileNames.Add(fn);
			using (FileStream fs = File.Open(fn, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				byte[] b = new byte[fs.Length];
				fs.Read(b, 0, (int)fs.Length);
				fs.Close();
				return b;
			}
		}

		/// <summary>
		/// Поиск текста
		/// </summary>
		/// <param name="text">Строка для поиса</param>
		/// <param name="startPosition">Стартовая позиция</param>
		/// <returns>Диапазон документа, или null, если не найдено</returns>
		public Range FindText(string text, int startPosition)
		{
			object m = Type.Missing;
			Range r = doc.Range(ref m, ref m);
			Find f = r.Find;
			object t = text;
			object b = false;
			if (f.Execute(ref t, ref b, ref b, ref b, ref b, ref b, ref m, ref m, ref m,
				ref m, ref m, ref m, ref m, ref m, ref m))
				return r;
			else
				return null;
		}

		/// <summary>
		/// Поиск текста
		/// </summary>
		/// <param name="text">Строка для поиса</param>
		/// <returns>Диапазон документа, или null, если не найдено</returns>
		public Range FindText(string text)
		{
			return FindText(text, 0);
		}

		/// <summary>
		/// Экспорт в PDF
		/// </summary>
		/// <param name="fileName">Имя файла</param>
		public void ExportToPDF(string fileName)
		{
			object fn = fileName;
			object m = Type.Missing;
			//object format = WdSaveFormat.wdFormatDocument;
			//doc.SaveAs(ref fn, ref format, ref m, ref m, ref m, ref m, ref m, ref m, ref m, ref m, ref m, ref m, ref m, ref m, ref m, ref m);
			doc.ExportAsFixedFormat(fileName, WdExportFormat.wdExportFormatPDF, false, WdExportOptimizeFor.wdExportOptimizeForPrint,
				 WdExportRange.wdExportAllDocument, 0, 0, WdExportItem.wdExportDocumentWithMarkup, true,
				 true, WdExportCreateBookmarks.wdExportCreateWordBookmarks, true, true, false, ref m);
		}

		/// <summary>
		/// Вставить картинку
		/// </summary>
		/// <param name="fileName">Файл</param>
		public void InsertPicture(string fileName)
		{
			object m = Type.Missing;
			doc.InlineShapes.AddPicture(fileName, ref m, ref m, ref m);
		}

		/// <summary>
		/// Вставить картинку
		/// </summary>
		/// <param name="data">Данные</param>
		public void InsertPicture(byte[] data)
		{
			object m = Type.Missing;
			string fn = Path.GetTempFileName();
			File.WriteAllBytes(fn, data);
			doc.InlineShapes.AddPicture(fn, ref m, ref m, ref m);
			File.Delete(fn);
		}

		public void AddPageNumbers()
		{
			object t = WdFieldType.wdFieldEmpty;
			object text = "PAGE   ";
			object preserveFormatting = true;
			foreach (Section wordSection in doc.Sections)
			{
				var r = wordSection.Footers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
				doc.Fields.Add(r, ref t, ref text, ref preserveFormatting);
				wordSection.Footers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			}			
		}

		public Document Document { get { return doc; } }

		public string GetTableCellText(int tableNumber, int row, int col)
		{
			try
			{
				return doc.Tables[tableNumber].Cell(row, col).Range.Text.TrimEnd('\r', '\a');
			}
			catch
			{
				return String.Empty;
			}
		}
	}
}

