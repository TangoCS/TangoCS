using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Office.Interop.Excel;

namespace Tango.MSOffice
{
	public class ExcelApplication : IDisposable
	{
		Application app;
		
		public ExcelApplication()
		{
			try
			{
				app = new Application();
			}
			catch (Exception e)
			{
				app = null;
				throw new Exception("Failed to start MS Excel: " + e.Message, e);
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (app != null)
			{
				try
				{
					app.Quit();
				}
				catch { }
			}
		}

		#endregion

		public ExcelWorkbook Open(string fileName)
		{
			try
			{
				string fn = PDFConverter.GetTempFileName();// +Path.GetExtension(fileName);
				File.Copy(fileName, (string)fn, true);
				Workbook doc = app.Workbooks.Open(fn, 0, false, 5, "", "", false, XlPlatform.xlWindows, "", true, false, 0, false, false, false);
				return new ExcelWorkbook(doc, fn);
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public ExcelWorkbook Open(byte[] data)
		{
			string fn = PDFConverter.GetTempFileName();
			File.WriteAllBytes((string)fn, data);
			object m = Type.Missing;
			Workbook doc = app.Workbooks.Open(fn, m, m, m, m, m, m, m, m, m, m, m, m, m, m);
			return new ExcelWorkbook(doc, fn);
		}

		public ExcelWorkbook Create()
		{
			object visible = false;
			object m = Type.Missing;
			Workbook doc = app.Workbooks.Add(m);
			return new ExcelWorkbook(doc, null);
		}
	}
}
