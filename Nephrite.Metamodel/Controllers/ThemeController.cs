using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web;
using System.IO;
using Nephrite.Metamodel.Model;

namespace Nephrite.Metamodel.Controllers
{
    [ControllerControlsPath("/_controltemplates/Nephrite.Metamodel/")]
    public class ThemeController : BaseController
    {
        public void View()
        {
            string path = WebPart.Page.Server.MapPath("~/THEMES");
			var di = new DirectoryInfo(path);

            var dirs = di.GetFiles("theme.css", SearchOption.AllDirectories).Select(
                f => f.Directory);
            List<Theme> themes = new List<Theme>();

            foreach (var dir in dirs)
            {
                string inf = Path.Combine(dir.FullName, dir.Name + ".inf");
                if (!File.Exists(inf))
                    continue;
                themes.Add(new Theme
                {
                    Name = dir.Name,
                    Title = dir.Name
                });
            }
            themes.Sort(new Comparison<Theme>((a, b) => a.Title.CompareTo(b.Title)));
            RenderView("view", themes);
        }
    }
}
