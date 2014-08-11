using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Nephrite.Web.FileStorage;

namespace Nephrite.Web.Office
{
	public class WordDocumentGenerator
	{
		public byte[] DocTemplate { get; set; }
		public object DataSource { get; set; }

		public byte[] Generate()
		{
			byte[] output = null;

			using (MemoryStream ms = new MemoryStream())
			{
				ms.Write(DocTemplate, 0, DocTemplate.Length);

				using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(ms, true))
				{
					wordDocument.ChangeDocumentType(WordprocessingDocumentType.Document);
					var mainDocumentPart = wordDocument.MainDocumentPart;

					foreach (var hp in mainDocumentPart.HeaderParts)
						SetContentInElement(hp.Header, DataSource);
					SetContentInElement(mainDocumentPart.Document, DataSource);
					foreach (var fp in mainDocumentPart.FooterParts)
						SetContentInElement(fp.Footer, DataSource);
					Clean(mainDocumentPart.Document, null);
					mainDocumentPart.Document.Save();
				}

				ms.Position = 0;
				output = new byte[ms.Length];
				ms.Read(output, 0, output.Length);
			}

			return output;
		}

		void SetContentInElements(List<OpenXmlElement> elements, object data, string propertyName)
		{
			var p = data.GetType().GetProperty(propertyName);
			var value = p != null ? p.GetValue(data, null) : null;
			if (!(value is IEnumerable))
				return;
			foreach (var obj in value as IEnumerable)
			{
				foreach (var element in elements)
				{
					var tag = getTag(element);
					tag = tag.Substring(tag.IndexOf(".") + 1);
					p = obj.GetType().GetProperty(tag);
					if (p != null && p.GetValue(obj, null) is IEnumerable)
					{
						foreach (var obj1 in p.GetValue(obj, null) as IEnumerable)
						{
							var node = element.CloneNode(true);
							SetContentInElement(node, obj1);
							elements.First().InsertBeforeSelf(node);
						}
					}
					else
					{
						var node = element.CloneNode(true);
						SetContentInElement(node, obj);
						elements.First().InsertBeforeSelf(node);
					}
				}
			}
			foreach (var element in elements)
				element.Remove();
		}

		void SetContentInElement(OpenXmlElement element, object data)
		{
			if (element is SdtElement)
			{
				string propertyName = getTag(element);
				if (propertyName != null)
				{
					var p = data.GetType().GetProperty(propertyName);
					var value = p != null ? p.GetValue(data, null) : null;
					var ap = data.GetType().GetProperty(propertyName + "Action");
					Action<OpenXmlElement> a = ap == null ? null : ap.GetValue(data, null) as Action<OpenXmlElement>;
					if (value != null)
					{
						if (value is ValueType || value is string)
						{
							SetContent(element, value.ToString());
							if (a != null)
								a(element);
						}
						else if (value is IEnumerable)
						{
							foreach (object obj in (value as IEnumerable))
							{
								var node = element.CloneNode(true);
								PopulateOtherOpenXmlElements(node, obj);
								element.InsertBeforeSelf(node);
							}
							element.Remove();
						}
						else
						{
							PopulateOtherOpenXmlElements(element, value);
						}
						return;
					}
				}
			}
			PopulateOtherOpenXmlElements(element, data);
		}

		void PopulateOtherOpenXmlElements(OpenXmlElement element, object data)
		{
			var elements = new List<OpenXmlElement>();
			string tag = null;
			if (element is OpenXmlCompositeElement && element.HasChildren)
			{
				foreach (var childelement in element.Elements().OfType<OpenXmlCompositeElement>().ToList())
				{
					string t = getTag(childelement);
					if (t != null && t.Contains("."))
					{
						if (t.Substring(0, t.IndexOf(".")) == tag)
							elements.Add(childelement);
						else
						{
							if (elements.Count > 0)
								SetContentInElements(elements, data, tag);
							elements.Clear();
							elements.Add(childelement);
							tag = t.Substring(0, t.IndexOf("."));
						}
					}
					else
					{
						if (elements.Count > 0)
							SetContentInElements(elements, data, tag);
						elements.Clear();
						SetContentInElement(childelement, data);
					}
				}
				if (elements.Count > 0)
					SetContentInElements(elements, data, tag);
			}
		}

		string getTag(OpenXmlElement element)
		{
			if (element is SdtElement)
			{
				var sdtElement = element as SdtElement;
				var tag = sdtElement.SdtProperties.OfType<Tag>().SingleOrDefault();
				if (tag != null)
					return tag.Val.Value;
			}
			return null;
		}

		bool SetContent(OpenXmlElement element, string content)
		{
			if (element is Text)
			{
				if (content != null)
				{
					string[] lines = content.Split('\n');
					for (int i = 0; i < lines.Length - 1; i++)
					{
						var e = element.InsertBeforeSelf(new Text(lines[i]));
						e.InsertAfterSelf<Break>(new Break());
					}
					((Text)element).Text = lines[lines.Length - 1];
				}
				return true;
			}
			else
			{
				foreach (var c in element.ChildElements)
				{
					if (SetContent(c, content))
						content = "";
				}
				return content == "";
			}
		}

		public static byte[] CreateFromTemplate(string templateFullPath, object data)
		{
			WordDocumentGenerator gen = new WordDocumentGenerator();
			gen.DataSource = data;
			var file = FileStorageManager.DbFiles.FirstOrDefault(o => o.FullPath == templateFullPath && !o.MainID.HasValue);
			if (file == null)
				throw new Exception("В файловом хранилище отсутствует файл " + templateFullPath);
			gen.DocTemplate = file.GetBytes();
			return gen.Generate();
		}

		public static void Download(string fileName, byte[] bytes)
		{
			var ctx = System.Web.HttpContext.Current;
			ctx.Response.ContentType = fileName.EndsWith("xlsx") ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/msword";

			if (ctx.Request.Browser.Browser == "IE" || ctx.Request.Browser.Browser == "InternetExplorer")
			{
				ctx.Response.HeaderEncoding = System.Text.Encoding.Default;
				ctx.Response.AppendHeader("content-disposition", "Attachment; FileName=\"" + System.Web.HttpUtility.UrlPathEncode(fileName) + "\"");
			}
			else
			{
				ctx.Response.HeaderEncoding = System.Text.Encoding.UTF8;
				ctx.Response.AppendHeader("content-disposition", "Attachment; FileName=\"" + fileName + "\"");
			}

			ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
			ctx.Response.End();
		}

		void Clean(OpenXmlElement e, OpenXmlElement p)
		{
			if (e is SdtElement)
			{
				var content = e.ChildElements.SingleOrDefault(o => o is SdtContentBlock || o is SdtContentRun || o is SdtContentRow);
				if (content != null && content.HasChildren)
				{
					foreach (var c in content.ChildElements.ToList())
					{
						var n = c.CloneNode(true);
						p.InsertBefore(n, e);
						Clean(n, p);
					}
					p.RemoveChild(e);
				}
			}
			else
			{
				foreach (var childelement in e.ChildElements.ToList())
					Clean(childelement, e);
			}
		}
	}
}