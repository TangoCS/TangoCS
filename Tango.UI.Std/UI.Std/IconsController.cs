using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tango;
using Tango.Cache;
using Tango.UI;

namespace Tango.UI.Std
{
	public class IconsController : BaseController
	{
		[Inject]
		public IHostingEnvironment Env { get; set; }
		[Inject]
		public IPersistentSettings Settings { get; set; }
		[Inject]
		public ICache Cache { get; set; }

		[AllowAnonymous]
		[HttpGet]
		public ActionResult List(string theme)
		{
			var sb = new StringBuilder();

			var path = $"{Env.WebRootPath}{Path.DirectorySeparatorChar}icons";

			foreach (string file in Directory.EnumerateFiles(path, "*.svg", SearchOption.AllDirectories))
			{
				var name = Path.GetFileNameWithoutExtension(file);
				var webPath = file.Replace(Env.WebRootPath, "").Replace(Path.DirectorySeparatorChar, '/');

				sb.AppendLine($".icon-{name} {{ background-image: url({webPath}); }}");
				sb.AppendLine();
			}

			return new ContentResult { ContentType = "text/css", Content = sb.ToString() };
		}

		[AllowAnonymous]
		[HttpGet]
		public ActionResult Svg()
		{
			var s = Cache.GetOrAdd("svg", () =>	{
				XNamespace xsvg = XNamespace.Get("http://www.w3.org/2000/svg");
				XNamespace xlink = XNamespace.Get("http://www.w3.org/1999/xlink");

				var svg = new XElement(
					xsvg + "svg",
					new XAttribute("id", "icons"),
					new XAttribute("aria-hidden", "true"),
					new XAttribute("style", "position: absolute; width: 0; height: 0; overflow: hidden;"),
					new XAttribute("version", "1.1"),
					new XAttribute(XNamespace.Xmlns + "xlink", xlink.NamespaceName)
				);
				//var defs = new XElement(xsvg + "defs");
				//svg.Add(defs);

				var theme = Context.PersistentArgs.Get("theme");
				if (theme.IsEmpty()) theme = Settings.Get("theme");
				theme = theme.ToLower();

				var path = $"{Env.WebRootPath}{Path.DirectorySeparatorChar}icons";
				var themePath = $"{Env.WebRootPath}{Path.DirectorySeparatorChar}themes{Path.DirectorySeparatorChar}{theme}{Path.DirectorySeparatorChar}icons";

				var files = Directory.EnumerateFiles(path, "*.svg", SearchOption.AllDirectories)
					.ToDictionary(x => Path.GetFileNameWithoutExtension(x), x => x);

				if (Directory.Exists(themePath))
					Directory.EnumerateFiles(themePath, "*.svg", SearchOption.AllDirectories)
						.ForEach(x => files[Path.GetFileNameWithoutExtension(x)] = x);

				foreach (var file in files)
				{
					var webPath = file.Value.Replace(Env.WebRootPath, "").Replace(Path.DirectorySeparatorChar, '/');

					var iconXml = XDocument.Parse(File.ReadAllText(file.Value));

					iconXml.Root.Name = xsvg + "symbol";
					iconXml.Root.Descendants(xsvg + "title").Remove();
					var viewBox = iconXml.Root.Attribute("viewBox");
					iconXml.Root.RemoveAttributes();
					iconXml.Root.Add(new XAttribute("id", "icon-" + file.Key.ToLower()));
					iconXml.Root.Add(viewBox);
					/*var iconDefs = iconXml.Root.Descendants(xsvg + "defs");
					if (iconDefs.Any())
					{
						foreach (var iconDef in iconDefs)
							foreach (var el in iconDef.Elements())
								defs.AddFirst(el); 

						iconDefs.Remove();
					}*/
					var iconStyle = iconXml.Root.Descendants(xsvg + "style")?.FirstOrDefault();
					if (iconStyle != null)
					{
						var classStyles = new Dictionary<string, string>();
						var cssContent = iconStyle.Value;

						// Regular expression to match CSS classes and their styles
						var pattern = @"\.(?<className>[a-zA-Z0-9_-]+)\s*\{\s*(?<style>[^}]+)\s*\}";
						MatchCollection matches = Regex.Matches(cssContent, pattern);

						foreach (Match match in matches)
						{
							var className = match.Groups["className"].Value.Trim();
							var styleRules = match.Groups["style"].Value.Trim();
							classStyles[className] = styleRules;
						}

						foreach (var el in iconXml.Root.Descendants())
						{
							var className = el.Attribute("class");
							if (className != null && classStyles.ContainsKey(className.Value))
							{
								className.Remove();
								el.Add(new XAttribute("style", classStyles[className.Value]));
							}
						}
						iconStyle.Remove();
					}
					svg.Add(iconXml.Root);
				}
				return svg.ToString();
			});

			return new HttpResult { ContentType = "image/svg+xml", ContentFunc = ctx => Encoding.UTF8.GetBytes(s) };
		}
	}
}
