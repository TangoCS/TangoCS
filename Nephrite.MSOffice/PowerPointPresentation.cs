using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Office.Interop.PowerPoint;

namespace Nephrite.MSOffice
{
	public class PowerPointPresentation : IDisposable
	{
		List<string> fileNames = new List<string>();
		Presentation doc;

		internal PowerPointPresentation(Presentation document, string fileName)
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
				doc.Close();
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
			doc.SaveAs(fileName, PpSaveAsFileType.ppSaveAsDefault, Microsoft.Office.Core.MsoTriState.msoTrue);
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
			doc.SaveAs(fileName, PpSaveAsFileType.ppSaveAsPDF, Microsoft.Office.Core.MsoTriState.msoTrue);
		}

	}
}
