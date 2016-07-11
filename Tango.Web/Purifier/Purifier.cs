using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using Sgml;

namespace Nephrite.HtmlPurifier
{
    public static class Purifier
    {
        class AttrList
        {
            public string[] Allow;
            public string[] Deny;
        }

        static Dictionary<string, AttrList> tags;
		static Dictionary<string, List<string>> allowStyles;
		static Dictionary<string, List<string>> denyStyles;

        static void InitTags()
        {
            tags = new Dictionary<string, AttrList>();

            XmlDocument xd = new XmlDocument();
            if (tagsxml != null)
                xd.LoadXml(tagsxml);
            else if (tagsfile != null)
            {
                if (Path.IsPathRooted(tagsfile))
                    xd.Load(tagsfile);
                else
                    xd.Load(AppDomain.CurrentDomain.BaseDirectory + tagsfile);
            }
            else
                throw new Exception("Call SetTagsXml or SetTagsXmlFile before calling Purify");

            foreach (XmlElement xe in xd.DocumentElement.SelectNodes("//tag"))
            {
                AttrList l = new AttrList();
                l.Allow = xe.GetAttribute("allow").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                l.Deny = xe.GetAttribute("deny").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                tags.Add(xe.GetAttribute("id"), l);
            }

            allowStyles = new Dictionary<string, List<string>>();
			denyStyles = new Dictionary<string, List<string>>();
            foreach (XmlElement xe in xd.DocumentElement.SelectNodes("//style"))
            {
				List<string> styles = null;
				if (String.IsNullOrEmpty(xe.GetAttribute("tag")))
				{
					styles = new List<string>();
					foreach (string s in xe.GetAttribute("allow").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
						styles.Add(s.Trim());
					allowStyles.Add("*", styles);

					styles = new List<string>();
					foreach (string s in xe.GetAttribute("deny").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
						styles.Add(s.Trim());
					denyStyles.Add("*", styles);
				}
				else
				{
					styles = new List<string>();
					foreach (string s in xe.GetAttribute("allow").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
						styles.Add(s.Trim());
					allowStyles.Add(xe.GetAttribute("tag"), styles);

					styles = new List<string>();
					foreach (string s in xe.GetAttribute("deny").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
						styles.Add(s.Trim());
					denyStyles.Add(xe.GetAttribute("tag"), styles);
				}
                //allowStyles.AddRange();
                //denyStyles.AddRange(xe.GetAttribute("deny").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }
			/*for (int i = 0; i < allowStyles.Count; i++)
				allowStyles[i] = allowStyles[i].Trim();

			for (int i = 0; i < denyStyles.Count; i++)
				denyStyles[i] = denyStyles[i].Trim();*/

			
				
        }

		static string tagsxml = null;
        static string tagsfile = null;

        public static void SetTagsXml(string xml)
        {
            tagsxml = xml;
        }

        public static void SetTagsXmlFile(string fileName)
        {
            tagsfile = fileName;
        }

        public static string Purify(string source)
        {
            return Purify(source, true);
        }

        public static string Purify(string source, bool formatParagraphs)
        {
            XmlDocument xd = TextToXml(source);
            InitTags();
            ProcessNode(xd.DocumentElement);
            // Заменить переводы строк на <br> в текстовых узлах
            ReplaceCaret(xd.DocumentElement);
            // Обработка переводов строк
            RemoveBrSequence(xd.DocumentElement);
            if (formatParagraphs)
                WrapText(xd.DocumentElement);
            StringWriter sw = new StringWriter();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.OmitXmlDeclaration = true;
            xws.ConformanceLevel = ConformanceLevel.Fragment;
            XmlWriter xw = XmlWriter.Create(sw, xws);
            xd.DocumentElement.WriteContentTo(xw);
            xw.Close();
            return sw.ToString();
        }

        public static void ReplaceCaret(XmlNode xn1)
        {
            for (int i = 0; i < xn1.ChildNodes.Count; i++)
            {
                XmlNode xn = xn1.ChildNodes[i];

                if (xn.NodeType == XmlNodeType.Text)
                {
                    // Замена переводов строк на br
                    string[] texts = xn.Value.Replace("\r", "").Split('\n');
                    if (texts.Length > 1)
                    {
                        int st = 0;
                        while (texts[st] == String.Empty && st < texts.Length - 1) st++;
                        xn.Value = texts[st];
                        for (int j = st + 1; j < texts.Length; j++)
                        {
                            xn = xn.ParentNode.InsertAfter(xn.OwnerDocument.CreateElement("br"), xn);
                            i++;
                            if (texts[j] != String.Empty)
                            {
                                xn = xn.ParentNode.InsertAfter(xn.OwnerDocument.CreateTextNode(texts[j]), xn);
                                i++;
                            }
                        }
                    }
                }
            }
        }

        public static XmlDocument TextToXml(string html)
        {
            html = "<html>" + html + "</html>";
            SgmlReader sr = new SgmlReader();
            sr.InputStream = new StringReader(html);
            sr.DocType = "HTML";
            StringWriter sw = new StringWriter();
            XmlTextWriter w = new XmlTextWriter(sw);
            while (sr.Read())
                w.WriteNode(sr, true);
            w.Flush();
            w.Close();
            sw.Close();
            sr.Close();
            sr.InputStream.Close();

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(sw.ToString());
            return xd;
        }

        static void RemoveBrSequence(XmlNode xn1)
        {
            // Удалить все повторяющиеся более двух раз подряд переводы строки
            for (int i = 0; i < xn1.ChildNodes.Count; i++)
            {
                XmlNode xn = xn1.ChildNodes[i];
                
                if (IsBr(xn) && IsBr(xn.NextSibling) && IsBr(xn.NextSibling.NextSibling))
                {
                    i--;
                    xn1.RemoveChild(xn);
                }
                RemoveBrSequence(xn);
            }
        }

        static void ProcessNode(XmlNode xn1)
        {
            for (int i = 0; i < xn1.ChildNodes.Count; i++)
            {
                XmlNode xn = xn1.ChildNodes[i];
                // Удалим элементы
                if (xn.NodeType == XmlNodeType.Element && !tags.ContainsKey(xn.Name.ToLower()))
                {
                    xn.ParentNode.RemoveChild(xn);
                    i--;
                    continue;
                }

                if (xn.NodeType == XmlNodeType.Element)
                {
                    AttrList l = tags[xn.Name.ToLower()];
                    for (int j = 0; j < xn.Attributes.Count; j++)
                    {
                        bool remove = false;
                        if (l.Deny.Length > 0)
                        {
                            if (l.Deny[0] == "*")
                                remove = true;
                            if (Array.IndexOf<string>(l.Deny, xn.Attributes[j].Name.ToLower()) >= 0)
                                remove = true;
                        }
                        if (l.Allow.Length > 0)
                        {
                            if (Array.IndexOf<string>(l.Allow, xn.Attributes[j].Name.ToLower()) < 0)
                                remove = true;
                        }
                        if (remove)
                        {
                            xn.Attributes.RemoveAt(j);
                            j--;
                            continue;
                        }


						List<string> nodeAllowStyles = allowStyles.ContainsKey("*") ? allowStyles["*"] : null;
						List<string> nodeDenyStyles = denyStyles.ContainsKey("*") ? denyStyles["*"] : null;

						List<string> nodeTagAllowStyles = allowStyles.ContainsKey(xn.Name.ToLower()) ? allowStyles[xn.Name.ToLower()] : null;
						List<string> nodeTagDenyStyles = denyStyles.ContainsKey(xn.Name.ToLower()) ? denyStyles[xn.Name.ToLower()] : null;

						if (xn.Attributes[j].Name.ToLower() == "style" && (nodeAllowStyles != null || nodeDenyStyles != null || nodeTagAllowStyles != null || nodeTagDenyStyles != null))
                        {
                            string val = xn.Attributes[j].Value;
                            string[] items = val.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                            val = String.Empty;
                            for (int k = 0; k < items.Length; k++)
                            {
                                remove = false;
                                string stylename = (items[k].IndexOf(':') > 0 ? items[k].Substring(0, items[k].IndexOf(':')) : items[k]).ToLower().Trim();
								if (nodeDenyStyles != null && nodeDenyStyles.Count > 0)
                                {
									if (nodeDenyStyles[0] == "*")
                                        remove = true;
									else if (nodeDenyStyles.Contains(stylename))
                                        remove = true;
								}
								if (nodeTagDenyStyles != null && nodeTagDenyStyles.Count > 0)
								{
									if (nodeTagDenyStyles[0] == "*")
										remove = true;
									else if (nodeTagDenyStyles.Contains(stylename))
										remove = true;
                                }
								if (nodeAllowStyles != null && nodeAllowStyles.Count > 0)
                                {
									if (!nodeAllowStyles.Contains(stylename))
										if (nodeTagAllowStyles != null && nodeTagAllowStyles.Count > 0)
										{
											if (!nodeTagAllowStyles.Contains(stylename))
												remove = true;
										}
                                        else 
											remove = true;
                                }
								
                                if (!remove)
                                {
                                    val = val.Length == 0 ? items[k] : (val + ";" + items[k]);
                                }
                            }
                            xn.Attributes[j].Value = val;
                        }
                    }
                }
                ProcessNode(xn);
            }
        }
    
        static void WrapText(XmlNode xn1)
        {
            string[] breakTags = { "div", "p", "body", "h1", "h2", "h3", "h4", "h5", "h6", "pre", "code", "table", "ul", "ol", "hr" };
            for (int i = 0; i < xn1.ChildNodes.Count; i++)
            {
                XmlNode xn = xn1.ChildNodes[i];

                if (xn.NodeType != XmlNodeType.Element ||
                    Array.IndexOf<string>(breakTags, xn.Name.ToLower()) < 0 )
                {
                    XmlElement p1 = xn1.OwnerDocument.CreateElement("p");
                    xn1.InsertBefore(p1, xn);
                    i++;
                    do
                    {
                        if (IsBr(xn) && IsBr(xn.NextSibling))
                        {
                            xn1.RemoveChild(xn.NextSibling);
                            xn1.RemoveChild(xn);
                            i -= 2;
                            break;
                        }
                        
                        xn1.RemoveChild(xn);
                        p1.AppendChild(xn);
                        if (i >= xn1.ChildNodes.Count)
                            break;
                        xn = xn1.ChildNodes[i];
                    }
                    while (xn.NodeType != XmlNodeType.Element ||
                        Array.IndexOf<string>(breakTags, xn.Name.ToLower()) < 0);
                    if (IsBr(p1.LastChild))
                        p1.RemoveChild(p1.LastChild);
                }
            }
        }

        static bool Clone1(XmlNode target, XmlNode source)
        {
            foreach (XmlNode child in source.ChildNodes)
            {
                if ((IsBr(child) && IsBr(child.NextSibling)) || child.Name.ToLower() == "p")
                    return true;
                if (Clone1(target.AppendChild(child.CloneNode(false)), child))
                    return true;
            }
            return false;
        }

        static void Clone2(XmlNode target, XmlNode source, XmlNode brNode)
        {
            foreach (XmlNode child in source.ChildNodes)
            {
                if (child == brNode.NextSibling)
                {
                    target.RemoveAll();
                    continue;
                }
                Clone2(target.AppendChild(child.CloneNode(false)), child, brNode);
            }
        }

        static bool IsBr(XmlNode xn)
        {
            return xn != null ? xn.Name.ToLower() == "br" && xn.NodeType == XmlNodeType.Element : false;
        }
    }
}
