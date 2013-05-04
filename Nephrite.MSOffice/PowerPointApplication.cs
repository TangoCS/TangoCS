using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Office.Interop.PowerPoint;

namespace Nephrite.MSOffice
{
	public class PowerPointApplication : IDisposable
	{
		Application app;
		
		public PowerPointApplication()
		{
			try
			{
				app = new Application();
				app.Visible = Microsoft.Office.Core.MsoTriState.msoFalse;
			}
			catch (Exception e)
			{
				app = null;
				throw new Exception("Failed to start MS PowerPoint: " + e.Message, e);
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

		public PowerPointPresentation Open(string fileName)
		{
			string fn = PDFConverter.GetTempFileName();
			File.Copy(fileName, (string)fn, true);
			object m = Type.Missing;
			Presentation doc = app.Presentations.Open(fn, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoFalse);
			return new PowerPointPresentation(doc, fn);
		}

		public PowerPointPresentation Open(byte[] data)
		{
			string fn = PDFConverter.GetTempFileName();
			File.WriteAllBytes((string)fn, data);
			object m = Type.Missing;
			Presentation doc = app.Presentations.Open(fn, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoFalse);
			return new PowerPointPresentation(doc, fn);
		}

		public PowerPointPresentation Create()
		{
			Presentation doc = app.Presentations.Add(Microsoft.Office.Core.MsoTriState.msoFalse);
			return new PowerPointPresentation(doc, null);
		}
	}
}
