using System;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;

namespace Tango.Razor
{
	public class RazorRenderer
	{
		public string Render(string fileName, object viewData)
		{
			var key = fileName.ToLower() + "?" + File.GetLastWriteTime(fileName).ToString();
			return Render(key, () => File.ReadAllText(fileName), viewData);
		}

		public string Render(string key, Func<string> template, object viewData)
		{
			if (Engine.Razor.IsTemplateCached(key, viewData.GetType()))
				return Engine.Razor.Run(key, viewData.GetType(), viewData);
			else
			{
				return Engine.Razor.RunCompile(template(), key, viewData.GetType(), viewData);
			}
		}
	}
}
