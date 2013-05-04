using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Office.Interop.Word;

namespace Nephrite.MSOffice
{
	public class WordApplication : IDisposable
	{
		Application app;
		
		public WordApplication()
		{
			try
			{
				app = new Application();
				app.Visible = false;
			}
			catch (Exception e)
			{
				app = null;
				throw new Exception("Failed to start MS Word: " + e.Message, e);
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (app != null)
			{
				object saveChanges = false;
				object oMissing = Type.Missing;
				try
				{

					app.Quit(ref saveChanges, ref oMissing, ref oMissing);
				}
				catch { }
			}
		}

		#endregion

		public WordDocument Open(string fileName)
		{
			object fn = PDFConverter.GetTempFileName();
			File.Copy(fileName, (string)fn, true);
			object m = Type.Missing;
			Document doc = app.Documents.Open(ref fn, ref m, ref m, ref m, ref m, ref m,
				ref m, ref m, ref m, ref m, ref m, ref m, ref m, ref m, ref m, ref m);
			return new WordDocument(doc, (string)fn);
		}

		public WordDocument Open(byte[] data)
		{
			object fn = PDFConverter.GetTempFileName();
			File.WriteAllBytes((string)fn, data);
			object m = Type.Missing;
			Document doc = app.Documents.Open(ref fn, ref m, ref m, ref m, ref m, ref m,
				ref m, ref m, ref m, ref m, ref m, ref m, ref m, ref m, ref m, ref m);
			return new WordDocument(doc, (string)fn);
		}

		public WordDocument Create()
		{
			object visible = false;
			object doctype = WdNewDocumentType.wdNewBlankDocument;
			object m = Type.Missing;
			var doc = app.Documents.Add(ref m, ref m, ref doctype, ref visible);
			return new WordDocument(doc, null);
		}
	}
}