using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.IO;

namespace Nephrite.Web.FileStorage
{
    public static class FileTypeIcon
    {
        static string defaultImage;
        static Dictionary<string, string> imageNames;

        public static string GetImageName(string extension)
        {
            if (imageNames == null)
            {
                XDocument xd = XDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DOCICON.XML"));
                defaultImage = xd.Descendants("Default").Single().Element("Mapping").Attribute("Value").Value;
                imageNames = new Dictionary<string, string>();
                foreach (var xe in xd.Descendants("ByExtension").Single().Elements("Mapping"))
                {
                    imageNames.Add(xe.Attribute("Key").Value.ToLower(), xe.Attribute("Value").Value);
                }
            }

            if (extension.StartsWith("."))
                extension = extension.Substring(1);
            extension = extension.ToLower();

            if (imageNames.ContainsKey(extension))
                return imageNames[extension];
            return defaultImage;
        }
    }
}
