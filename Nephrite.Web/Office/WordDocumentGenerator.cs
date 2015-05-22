using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.CustomProperties;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
using Nephrite.FileStorage;
using Microsoft.Framework.DependencyInjection;

namespace Nephrite.Web.Office
{
	public class WordDocumentGenerator
	{
		public byte[] DocTemplate { get; set; }
		public object DataSource { get; set; }
		public bool MacroEnabled { get; set; }
		public CustomProperty[] CustomProperties { get; set; }

		public byte[] Generate()
		{
			byte[] output = null;

			using (MemoryStream ms = new MemoryStream())
			{
				ms.Write(DocTemplate, 0, DocTemplate.Length);

				using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(ms, true))
				{
					wordDocument.ChangeDocumentType(MacroEnabled ? WordprocessingDocumentType.MacroEnabledDocument : WordprocessingDocumentType.Document);
					var mainDocumentPart = wordDocument.MainDocumentPart;

					foreach (var hp in mainDocumentPart.HeaderParts)
					{
						SetContentInElement(hp.Header, DataSource);
						Clean(hp.Header, null);
					}
					SetContentInElement(mainDocumentPart.Document, DataSource);
					Clean(mainDocumentPart.Document, null);
					foreach (var fp in mainDocumentPart.FooterParts)
					{
						SetContentInElement(fp.Footer, DataSource);
						Clean(fp.Footer, null);
					}
					mainDocumentPart.Document.Save();
					if (CustomProperties != null)
						foreach (var customProperty in CustomProperties)
							SetCustomProperty(wordDocument, customProperty);
				}

				ms.Position = 0;
				output = new byte[ms.Length];
				ms.Read(output, 0, output.Length);
			}

			return output;
		}

		private void SetCustomProperty(WordprocessingDocument document, CustomProperty property)
		{
			var customProps = document.CustomFilePropertiesPart;
			if (customProps == null) return;
			var props = customProps.Properties;
			if (props == null) return;

			var prop = props.Where(p => ((CustomDocumentProperty)p).Name.Value == property.PropertyName).FirstOrDefault();
			if (prop == null) return;
			var custProp = prop as CustomDocumentProperty;

			switch (property.PropertyType)
			{
				case PropertyTypes.DateTime:
					custProp.VTFileTime = new VTFileTime(string.Format("{0:s}Z", Convert.ToDateTime(property.PropertyValue)));
					break;
				case PropertyTypes.NumberInteger:
					custProp.VTInt32 = new VTInt32(property.PropertyValue.ToString());
					break;
				case PropertyTypes.NumberDouble:
					custProp.VTFloat = new VTFloat(property.PropertyValue.ToString());
					break;
				case PropertyTypes.Text:
					custProp.VTLPWSTR = new VTLPWSTR(property.PropertyValue.ToString());
					break;
				case PropertyTypes.YesNo:
					custProp.VTBool = new VTBool(Convert.ToBoolean(property.PropertyValue).ToString().ToLower());
					break;
			}
			props.Save();
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

			var storage = DI.RequestServices.GetService<IStorage<string>>();
			var file = storage.GetFile(templateFullPath);
			if (file == null)
				throw new Exception("В файловом хранилище отсутствует файл " + templateFullPath);
			gen.DocTemplate = file.ReadAllBytes();
			return gen.Generate();
		}

		public static byte[] CreateFromTemplate(string templateFullPath, object data, bool ismacroenabled, CustomProperty[] custom = null)
		{
			WordDocumentGenerator gen = new WordDocumentGenerator();
			gen.DataSource = data;
			gen.MacroEnabled = ismacroenabled;
			gen.CustomProperties = custom;
			var storage = DI.RequestServices.GetService<IStorage<string>>();
			var file = storage.GetFile(templateFullPath);
			if (file == null)
				throw new Exception("В файловом хранилище отсутствует файл " + templateFullPath);
			gen.DocTemplate = file.ReadAllBytes();
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
				var content = e.ChildElements.SingleOrDefault(o => o is SdtContentBlock || o is SdtContentRun || o is SdtContentRow || o is SdtContentCell);
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

		public static Result SetCustomProperty(string fileName, string propertyName, PropertyTypes propertyType, object propertyValue)
		{
			using (var document = WordprocessingDocument.Open(fileName, true))
			{
				var customProps = document.CustomFilePropertiesPart;
				if (customProps == null) return new Result(-1, "В документе отсутствуют расширенные свойства");
				var props = customProps.Properties;
				if (props == null) return new Result(-1, "В документе отсутствуют расширенные свойства");

				var prop = props.Where(p => ((CustomDocumentProperty)p).Name.Value == propertyName).FirstOrDefault();
				if (prop == null) return new Result(-1, "В документе отсутствует свойство " + propertyName);
				var custProp = prop as CustomDocumentProperty;
				//if (prop != null) prop.Remove();

				// Append the new property, and 
				// fix up all the property ID values. 
				// The PropertyId value must start at 2.
				//props.AppendChild(newProp);
				//int pid = 2;
				//foreach (CustomDocumentProperty item in props)
				//{
				//	item.PropertyId = pid++;
				//}

				switch (propertyType)
				{
					case PropertyTypes.DateTime:
						custProp.VTFileTime = new VTFileTime(string.Format("{0:s}Z", Convert.ToDateTime(propertyValue)));
						break;
					case PropertyTypes.NumberInteger:
						custProp.VTInt32 = new VTInt32(propertyValue.ToString());
						break;
					case PropertyTypes.NumberDouble:
						custProp.VTFloat = new VTFloat(propertyValue.ToString());
						break;
					case PropertyTypes.Text:
						custProp.VTLPWSTR = new VTLPWSTR(propertyValue.ToString());
						break;
					case PropertyTypes.YesNo:
						custProp.VTBool = new VTBool(Convert.ToBoolean(propertyValue).ToString().ToLower());
						break;
				}
				props.Save();
			}
			return new Result(0, "");
		}
	}

	public enum PropertyTypes : int
	{
		YesNo,
		Text,
		DateTime,
		NumberInteger,
		NumberDouble
	}

	public class CustomProperty
	{
		public string PropertyName { get; set; }
		public PropertyTypes PropertyType { get; set; }
		public object PropertyValue { get; set; }
	}
}