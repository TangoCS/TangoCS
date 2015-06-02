using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;

namespace Nephrite.MSOffice
{
	public class ExcelWorkbook : IDisposable
	{
		List<string> fileNames = new List<string>();
		Workbook doc;

		internal ExcelWorkbook(Workbook document, string fileName)
		{
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
				doc.Close(saveChanges, m, m);
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
			doc.SaveAs(fn, m, m, m, m, m, XlSaveAsAccessMode.xlShared, m, m, m, m, m);
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
		/// Экспорт в PDF
		/// </summary>
		/// <param name="fileName">Имя файла</param>
		public void ExportToPDF(string fileName)
		{
			object fn = fileName;
			object m = Type.Missing;
			doc.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, fn, Microsoft.Office.Interop.Excel.XlFixedFormatQuality.xlQualityStandard,
				false, false, m, m, false, m);
		}

	}
}
