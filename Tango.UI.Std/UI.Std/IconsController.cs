using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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

				var defs = new XElement(xsvg + "defs");

				XDocument doc = new XDocument(
					new XElement(
						xsvg + "svg",
						new XAttribute("aria-hidden", "true"),
						new XAttribute("style", "position: absolute; width: 0; height: 0; overflow: hidden;"),
						new XAttribute("version", "1.1"),
						new XAttribute(XNamespace.Xmlns + "xlink", xlink.NamespaceName),
						defs
					)
				);

				var theme = Context.PersistentArgs.Get("theme");
				if (theme.IsEmpty()) theme = Settings.Get("theme");

				var path = $"{Env.WebRootPath}{Path.DirectorySeparatorChar}icons";
				var themePath = $"{Env.WebRootPath}{Path.DirectorySeparatorChar}themes{Path.DirectorySeparatorChar}{theme}{Path.DirectorySeparatorChar}icons";

				var files = Directory.EnumerateFiles(path, "*.svg", SearchOption.AllDirectories)
					.ToDictionary(x => Path.GetFileNameWithoutExtension(x), x => x);

				if (Directory.Exists(themePath))
					Directory.EnumerateFiles(themePath, "*.svg", SearchOption.AllDirectories).ForEach(x => files[Path.GetFileNameWithoutExtension(x)] = x);

				foreach (var file in files)
				{
					var webPath = file.Value.Replace(Env.WebRootPath, "").Replace(Path.DirectorySeparatorChar, '/');

					var iconXml = XDocument.Parse(File.ReadAllText(file.Value));

					iconXml.Root.Name = xsvg + "symbol";
					iconXml.Root.Descendants(xsvg + "title").Remove();
					var viewBox = iconXml.Root.Attribute("viewBox");
					iconXml.Root.RemoveAttributes();
					iconXml.Root.Add(new XAttribute("id", "icon-" + file.Key));
					//iconXml.Root.Add(new XAttribute("viewBox", "0 0 32 32"));
					iconXml.Root.Add(viewBox);
					defs.Add(iconXml.Root);
				}
				return doc.ToString();
			});

			return new HttpResult { ContentType = "image/svg+xml", ContentFunc = ctx => Encoding.UTF8.GetBytes(s) };
		}
	}
}
