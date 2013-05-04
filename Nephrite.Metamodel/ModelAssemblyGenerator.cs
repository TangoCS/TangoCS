using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using Nephrite.Core;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Security;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using Nephrite.Metamodel.Model;
using System.Xml;
using System.Text;
using Microsoft.VisualStudio.TextTemplating;
using System.Reflection;
using Nephrite.Web;
using Nephrite.Web.SPM;
using System.Xml.Xsl;
using Nephrite.Web.FileStorage;

namespace Nephrite.Metamodel
{
	public class ModelAssemblyGenerator
	{
		public static string Output = "";
		public static string SourceFolder
		{
			get
			{
				if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["SourceFolder"]))
					return ConfigurationManager.AppSettings["SourceFolder"];
				if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["ModelNamespace"]))
					return ConfigurationManager.AppSettings["ModelNamespace"] + ".Model.dll";
				if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["ModelAssembly"]))
					return ConfigurationManager.AppSettings["ModelNamespace"] + ".dll";
				return AppMM.DBName() + ".Model.dll";
			}
		}

		public static string DllName
		{
			get
			{
				if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["ModelAssembly"]))
					return ConfigurationManager.AppSettings["ModelAssembly"] + ".dll";
				return AppMM.DBName() + ".Model.dll";
			}
		}

		static string TempDir
		{
			get
			{
				string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_temp");
				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);
				return path;
			}
		}

		public static string CreateModelDll()
		{
			return CreateModelDll(true);
		}

		public static string CreateModelDll(bool debug)
		{
			var objectTypes = AppMM.DataContext.MM_ObjectTypes.Where(o => o.IsSeparateTable && !o.IsTemplate).OrderBy(o => o.SysName).ToList();
			var otherfiles = FileStorageManager.DbFiles.Where(o => o.Path.StartsWith(SourceFolder) && !o.IsDeleted).ToList();
			
			// Создать временный каталог
			if (Directory.Exists(TempDir))
			{
				(new DirectoryInfo(TempDir)).Delete(true);
			}
			Directory.CreateDirectory(TempDir);

			Output = "";

			// Сгенерировать DLL
			// Create the C# compiler
			CSharpCodeProvider csCompiler = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", Config.CompilerVersion } });

			// input params for the compiler
			CompilerParameters cp = new CompilerParameters();
			cp.OutputAssembly = TempDir + "\\" + DllName;
			cp.ReferencedAssemblies.Add("mscorlib.dll");
			cp.ReferencedAssemblies.Add("System.dll");
			cp.ReferencedAssemblies.Add("System.Core.dll");
			cp.ReferencedAssemblies.Add("System.Configuration.dll");
			cp.ReferencedAssemblies.Add("System.Data.dll");
			cp.ReferencedAssemblies.Add("System.Data.Linq.dll");
			cp.ReferencedAssemblies.Add("System.Xml.dll");
			cp.ReferencedAssemblies.Add("System.Xml.Linq.dll");
			cp.ReferencedAssemblies.Add("System.Web.dll");
			cp.ReferencedAssemblies.Add("System.Web.Extensions.dll");
			cp.ReferencedAssemblies.Add("System.Web.Services.dll");
			cp.ReferencedAssemblies.Add("System.Transactions.dll");
						
			string binPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
			DirectoryInfo binDir = new DirectoryInfo(binPath);
			foreach (var dllFile in binDir.GetFiles("*.dll"))
				if (dllFile.Name != DllName && dllFile.Name != Url.Current.GetString("skipdll"))
					cp.ReferencedAssemblies.Add(dllFile.FullName);
			cp.ReferencedAssemblies.AddRange(Config.ReferencedAssembly.ToArray());

			// generate the DLL
			cp.GenerateExecutable = false;
			
			cp.CompilerOptions = "/target:library /optimize";

			string res = " /resource:\"" + TempDir + "\\model.dbml\"," + AppMM.DBName() + ".Model.model.dbml";
			cp.CompilerOptions += res;
			if (File.Exists(TempDir + "\\..\\" + AppMM.DBName() + ".snk"))
				cp.CompilerOptions += " /keyfile:\"" + TempDir + "\\..\\..\\" + AppMM.DBName() + ".snk\"";
			cp.IncludeDebugInformation = debug;

			if (Url.Current.GetString("platform") != "")
				cp.CompilerOptions += "/platform:" + Url.Current.GetString("platform");

			// Прочие файлы
			List<string> otherfileNames = new List<string>();
			foreach (var f in otherfiles)
			{
				string path = f.Path.Replace("/", "\\").Substring(SourceFolder.Length);
				if (path.StartsWith("\\"))
					path = path.Substring(1);
				path = TempDir + "\\" + path;
				Directory.CreateDirectory(path);
				if (f.Title == "model.dbml")
					File.WriteAllBytes(TempDir + "\\model.dbml", f.GetBytes());
				else if (f.Extension == ".cs")
				{
					var ofn = Path.Combine(path, f.Title);
					otherfileNames.Add(ofn);
					File.WriteAllBytes(ofn, f.GetBytes());
				}
			}

			// Run the compiler and build the assembly
			CompilerResults cr = null;
			try
			{
				cr = csCompiler.CompileAssemblyFromFile(cp, otherfileNames.Where(s => s.ToLower().EndsWith(".cs")).ToArray());

				if (cr.Errors.HasErrors)
				{
					StringBuilder error = new StringBuilder();
					error.AppendFormat("Всего ошибок и предупреждений: {0}<br />", cr.Errors.Count);
					foreach (var errinfo in cr.Errors.Cast<CompilerError>().OrderBy(o => o.IsWarning).Take(20))
					{
						int line = errinfo.Line;
						error.AppendFormat("<br /><br /><b>{0} {1} at {2} line {3} column {4} </b><br /><br />", errinfo.IsWarning ? "WARNING" : "ERROR", errinfo.ErrorText, errinfo.FileName, line, errinfo.Column);
						int minline = Math.Max(1, line - 30);
						if (!String.IsNullOrEmpty(errinfo.FileName))
						{
							using (var file = new StreamReader(errinfo.FileName))
							{
								int lineNo = 0;
								while (!file.EndOfStream)
								{
									lineNo++;
									string lineText = file.ReadLine();
									if (lineNo == line)
										error.Append("<span style=\"color:Red; font-size:13px; font-weight:bold\">");
									if (lineNo >= minline && lineNo < line + 20)
										error.Append(HttpUtility.HtmlEncode(lineText)).Append("<br />");
									if (lineNo == line)
										error.Append("</span>");
								}
							}
						}
					}
					return error.ToString();
				}
			}
			finally
			{
				File.WriteAllText(TempDir + "\\compileroutput.txt", String.Join("\r\n", cr.Output.Cast<string>().ToArray()));
			}
			if (File.Exists(TempDir + "\\" + DllName))
			{
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "bin\\" + DllName))
					File.Delete(AppDomain.CurrentDomain.BaseDirectory + "bin\\" + DllName);
				File.Move(TempDir + "\\" + DllName, AppDomain.CurrentDomain.BaseDirectory + "bin\\" + DllName);
			}
			var PdbName = DllName.Replace(".dll", ".pdb");
			if (File.Exists(TempDir + "\\" + PdbName))
			{
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "bin\\" + PdbName))
					File.Delete(AppDomain.CurrentDomain.BaseDirectory + "bin\\" + PdbName);
				File.Move(TempDir + "\\" + PdbName, AppDomain.CurrentDomain.BaseDirectory + "bin\\" + PdbName);
			}
			if (Config.PlaceAssemblyToGAC)
			{
				string gacutilpath = Config.GacutilPath;
				using (new Impersonation(Config.ElevatedAccessAccountDomain,
					Config.ElevatedAccessAccountUsername,
					Config.ElevatedAccessAccountPassword))
				{

					string args = String.Format("/u \"{0}\"", String.IsNullOrEmpty(ConfigurationManager.AppSettings["ModelAssembly"]) ? AppMM.DBName() + ".Model" : ConfigurationManager.AppSettings["ModelAssembly"]);
					Process gu = new Process();
					gu.StartInfo = new ProcessStartInfo(gacutilpath, args);
					gu.StartInfo.UseShellExecute = false;
					gu.StartInfo.RedirectStandardOutput = true;
					gu.Start();
					gu.WaitForExit();

					Output += "\r\n\r\n";
					Output += gu.StandardOutput.ReadToEnd();

					Output += String.Format("{0} {1}\r\n", gacutilpath, args);

					args = String.Format("/i \"{0}\"", AppDomain.CurrentDomain.BaseDirectory + "bin\\" + DllName);
					gu = new Process();
					gu.StartInfo = new ProcessStartInfo(gacutilpath, args);
					gu.StartInfo.UseShellExecute = false;
					gu.StartInfo.RedirectStandardOutput = true;
					gu.Start();
					gu.WaitForExit();

					Output += "\r\n\r\n";
					Output += gu.StandardOutput.ReadToEnd();

					Output += String.Format("{0} {1}\r\n", gacutilpath, args);

					args = String.Format("/noforce");
					gu = new Process();
					gu.StartInfo = new ProcessStartInfo("iisreset", args);
					gu.StartInfo.UseShellExecute = true;
					gu.Start();
				}
			}
			// Сохранить все файлы из папки bin в папку bin
			foreach (var dllFile in binDir.GetFiles("*.*"))
			{
				SaveFile(dllFile.Name, File.ReadAllBytes(dllFile.FullName), "bin");
			}
			WebSiteCache.Reset();
			return String.Empty;
		}

		static string GenerateFromTemplate(string tempdir, string templateName)
		{
			Engine eng = new Engine();
			Nephrite.Metamodel.TextTransform.Host host = new Nephrite.Metamodel.TextTransform.Host();
			host.CompilerVersion = "v4.0";// Config.CompilerVersion;
			host.TemplateFile = templateName + ".tt";
			File.WriteAllText(tempdir + "\\" + templateName + ".cs",
				eng.ProcessTemplate(host.GetTemplateContent(templateName + ".tt"), host),
				Encoding.UTF8);

			if (host.Errors.Count > 0)
			{
				string error = "";
				for (int err = 0; err < host.Errors.Count; err++)
				{
					var errinfo = host.Errors[err];
					int line = errinfo.Line;
					error += "<br /><br />" + errinfo.ErrorText + " at " + errinfo.FileName + " line " + line.ToString() + " column " + errinfo.Column.ToString() + "<br /><br />";
					int minline = Math.Max(1, line - 30);
					if (!String.IsNullOrEmpty(errinfo.FileName))
					{
						using (TextReader file = errinfo.FileName.EndsWith(".tt") ? (TextReader)(new StringReader(host.GetTemplateContent(errinfo.FileName))) : new StreamReader(errinfo.FileName))
						{
							int lineNo = 0;
							string lineText = file.ReadLine();
							do
							{
								lineNo++;

								if (lineNo == line)
									error += "<span style=\"color:Red; font-size:13px; font-weight:bold\">";
								if (lineNo >= minline && lineNo < line + 20)
									error += HttpUtility.HtmlEncode(lineText) + "<br />";
								if (lineNo == line)
									error += "</span>";
								lineText = file.ReadLine();
							} while (lineText != null);
						}
					}
				}
				return error;
			}
			return String.Empty;
		}

		public static string CreateLinq2SqlFiles()
		{
			var objectTypes = AppMM.DataContext.MM_ObjectTypes.Where(o => o.IsSeparateTable && !o.IsTemplate).OrderBy(o => o.SysName).ToList();

			// Создать временный каталог
			if (Directory.Exists(TempDir))
				Array.ForEach((new DirectoryInfo(TempDir)).GetFiles(), o => { o.Delete(); });

			Output = "";

			// Сгенерировать DML и CS
			string smname = Config.SqlMetalPath;
			string smargs = String.Format("/conn:\"{0}\" /sprocs /functions /views /dbml:\"{1}\\model.dbml\" /namespace:{2}.Model /context:modelDataContext",
				ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString, TempDir, AppMM.DBName());
			Process sqlmetal = new Process();
			sqlmetal.StartInfo = new ProcessStartInfo(smname, smargs);
			sqlmetal.Start();
			sqlmetal.WaitForExit();
			Output += String.Format("{0} {1}\r\n", smname, smargs);

			// Отредактировать dbml-файл
			XmlDocument doc = new XmlDocument();
			doc.Load(TempDir + "\\model.dbml");
			var mgrt = new XmlNamespaceManager(doc.NameTable);
			mgrt.AddNamespace("w", doc.DocumentElement.NamespaceURI);

			foreach (XmlElement column in doc.DocumentElement.SelectNodes("//w:Column[(@DbType='Char(1) NOT NULL' or @DbType='Char(1)')and @Type='System.String']", mgrt))
			{
				column.SetAttribute("Type", "System.Char");
			}
			foreach (XmlElement column in doc.DocumentElement.SelectNodes("//w:Column", mgrt))
			{
				if (column.Attributes.GetNamedItem("IsPrimaryKey") == null && column.GetAttribute("Name") != "LastModifiedDate")
					column.SetAttribute("UpdateCheck", "Never");
			}
			foreach (var prop in objectTypes.SelectMany(o => o.MM_ObjectProperties).Where(o => o.TypeCode == ObjectPropertyType.FileEx))
			{
				XmlElement type = (XmlElement)doc.DocumentElement.SelectSingleNode("//w:Type[@Name='" + prop.MM_ObjectType.SysName + (prop.IsMultilingual ? "Data" : "") + "']", mgrt);

				XmlElement asso = doc.CreateElement("Association", doc.DocumentElement.NamespaceURI);
				asso.SetAttribute("Name", "FK_" + prop.MM_ObjectType.SysName + (prop.IsMultilingual ? "Data" : "") + "_" + prop.SysName);
				asso.SetAttribute("Member", prop.SysName);
				asso.SetAttribute("IsForeignKey", "true");
				asso.SetAttribute("Type", "DbFile");
				asso.SetAttribute("ThisKey", prop.ColumnName);
				asso.SetAttribute("OtherKey", "ID");
				type.AppendChild(asso);
			}
			foreach (XmlElement association in doc.DocumentElement.SelectNodes("//w:Association[@IsForeignKey='true']", mgrt))
			{
				string typeName = ((XmlElement)association.ParentNode).GetAttribute("Name");
				association.SetAttribute("Member", association.GetAttribute("Name").Replace("FK_" + typeName + "_", ""));
			}
			foreach (XmlElement association in doc.DocumentElement.SelectNodes("//w:Association", mgrt).Cast<XmlElement>().ToList())
			{
				if (association.GetAttribute("IsForeignKey") == "true")
				{
					//if (association.GetAttribute("Type") == "N_File" && association.GetAttribute("OtherKey") == "Guid")
					//	association.ParentNode.RemoveChild(association);
					continue;
				}
				// Найти ассоциацию с таким же именем, являющуюся внешним ключом
				var otherAsso = doc.DocumentElement.SelectSingleNode("//w:Association[@IsForeignKey='true' and @Name='" + association.GetAttribute("Name") + "']", mgrt) as XmlElement;
				if (otherAsso != null)
				{
					string typeName = ((XmlElement)otherAsso.ParentNode).GetAttribute("Name");
					string property = otherAsso.GetAttribute("Member");
					// Найти свойство
					var t1 = objectTypes.SingleOrDefault(o => o.SysName == typeName);
					if (t1 != null)
					{
						var p = t1.MM_ObjectProperties.SingleOrDefault(o => o.SysName == property);
						if (p != null)
						{
							var otherProp = objectTypes.SelectMany(o => o.MM_ObjectProperties.Where(o1 => o1.RefObjectPropertyID.HasValue)).FirstOrDefault(o => o.RefObjectPropertyID == p.ObjectPropertyID);
							if (otherProp != null)
							{
								association.SetAttribute("Member", otherProp.SysName);
								if (otherProp.UpperBound == 1)
									association.SetAttribute("Cardinality", "One");
								else
									association.RemoveAttribute("Cardinality");
							}
						}
					}
				}
			}
			// Для представлений надо создать ассоциации
			foreach (XmlElement type in doc.DocumentElement.SelectNodes("//w:Type", mgrt))
			{
				MM_ObjectType t = objectTypes.SingleOrDefault(o => "V_" + o.SysName == type.GetAttribute("Name"));
				if (t != null)
				{
					foreach (var tpk in t.PrimaryKey)
					{
						XmlElement pk = (XmlElement)type.SelectSingleNode("w:Column[@Name='" + tpk.SysName + "']", mgrt);
						if (pk == null)
							throw new Exception("В таблице " + type.GetAttribute("Name") + " нет столбца " + tpk.SysName + "!");
						pk.SetAttribute("IsPrimaryKey", "true");
						pk = (XmlElement)type.SelectSingleNode("w:Column[@Name='LanguageCode']", mgrt);
						pk.SetAttribute("IsPrimaryKey", "true");
					}
					foreach (var p in t.MM_ObjectProperties.Where(o => o.TypeCode == ObjectPropertyType.Object && o.UpperBound == 1))
					{
						XmlElement asso = doc.CreateElement("Association", doc.DocumentElement.NamespaceURI);
						asso.SetAttribute("Name", "FK_V_" + t.SysName + "_" + p.SysName);
						asso.SetAttribute("Member", p.SysName);
						asso.SetAttribute("IsForeignKey", "true");
						if (p.RefObjectType.MM_ObjectProperties.Any(o => o.IsMultilingual))
						{
							asso.SetAttribute("Type", "V_" + p.RefObjectType.SysName);
							asso.SetAttribute("ThisKey", p.ColumnName + ",LanguageCode");
							asso.SetAttribute("OtherKey", p.RefObjectType.PrimaryKey.Single().ColumnName + ",LanguageCode");
						}
						else
						{
							asso.SetAttribute("Type", p.RefObjectType.SysName);
							asso.SetAttribute("ThisKey", p.ColumnName);
							asso.SetAttribute("OtherKey", p.RefObjectType.PrimaryKey.Single().ColumnName);
						}
						type.AppendChild(asso);
					}
					foreach (var p in t.MM_ObjectProperties.Where(o => o.TypeCode == ObjectPropertyType.Object && o.UpperBound == -1 && o.RefObjectPropertyID.HasValue))
					{
						XmlElement asso = doc.CreateElement("Association", doc.DocumentElement.NamespaceURI);
						asso.SetAttribute("Name", "FK_V_" + t.SysName + "_" + p.SysName);
						asso.SetAttribute("Member", p.SysName);
						asso.SetAttribute("IsForeignKey", "false");
						if (p.RefObjectType.MM_ObjectProperties.Any(o => o.IsMultilingual))
						{
							asso.SetAttribute("Type", "V_" + p.RefObjectType.SysName);
							asso.SetAttribute("ThisKey", t.PrimaryKey[0].ColumnName + ",LanguageCode");
							asso.SetAttribute("OtherKey", p.RefObjectProperty.ColumnName + ",LanguageCode");
						}
						else
						{
							asso.SetAttribute("Type", p.RefObjectType.SysName);
							asso.SetAttribute("ThisKey", t.PrimaryKey[0].ColumnName);
							asso.SetAttribute("OtherKey", p.RefObjectProperty.ColumnName);
						}
						type.AppendChild(asso);
					}
					// Ссылка на материнский объект
					if (t.PrimaryKey.Length == 1)
					{
						XmlElement assoM = doc.CreateElement("Association", doc.DocumentElement.NamespaceURI);
						assoM.SetAttribute("Name", "FK_V_" + t.SysName + "_MainObject");
						assoM.SetAttribute("Member", "MainObject");
						assoM.SetAttribute("IsForeignKey", "true");
						assoM.SetAttribute("Type", t.SysName);
						assoM.SetAttribute("ThisKey", t.PrimaryKey[0].SysName);
						assoM.SetAttribute("OtherKey", t.PrimaryKey[0].SysName);
						type.AppendChild(assoM);
					}
				}

				t = objectTypes.SingleOrDefault(o => "V_HST_" + o.SysName == type.GetAttribute("Name"));
				if (t != null)
				{
					XmlElement pk = (XmlElement)type.SelectSingleNode("w:Column[@Name='" + t.PrimaryKey.Single().ColumnName.Replace("ID", "VersionID") + "']", mgrt);
					pk.SetAttribute("IsPrimaryKey", "true");
					pk = (XmlElement)type.SelectSingleNode("w:Column[@Name='LanguageCode']", mgrt);
					pk.SetAttribute("IsPrimaryKey", "true");
					foreach (var p in t.MM_ObjectProperties.Where(o => o.TypeCode == ObjectPropertyType.Object && o.UpperBound == 1))
					{
						XmlElement asso = doc.CreateElement("Association", doc.DocumentElement.NamespaceURI);
						asso.SetAttribute("Name", "FK_V_" + t.SysName + "_" + p.SysName);
						asso.SetAttribute("Member", p.SysName);
						asso.SetAttribute("IsForeignKey", "true");
						if (p.RefObjectType.MM_ObjectProperties.Any(o => o.IsMultilingual))
						{
							asso.SetAttribute("Type", "V_" + p.RefObjectType.SysName);
							asso.SetAttribute("ThisKey", p.ColumnName + ",LanguageCode");
							asso.SetAttribute("OtherKey", p.RefObjectType.PrimaryKey.Single().ColumnName + ",LanguageCode");
						}
						else
						{
							asso.SetAttribute("Type", p.RefObjectType.SysName);
							asso.SetAttribute("ThisKey", p.ColumnName);
							asso.SetAttribute("OtherKey", p.RefObjectType.PrimaryKey.Single().ColumnName);
						}
						type.AppendChild(asso);
					}
					XmlElement asso1 = doc.CreateElement("Association", doc.DocumentElement.NamespaceURI);
					asso1.SetAttribute("Name", "FK_V_HST_" + t.SysName + "_CHST_" + t.SysName);
					asso1.SetAttribute("Member", "CHST_" + t.SysName);
					asso1.SetAttribute("IsForeignKey", "true");
					asso1.SetAttribute("Type", "CHST_" + t.SysName);
					asso1.SetAttribute("ThisKey", "ClassVersionID");
					asso1.SetAttribute("OtherKey", "ClassVersionID");
					type.AppendChild(asso1);
				}
			}
			doc.Save(TempDir + "\\model.dbml");

			smargs = String.Format("/code:\"{0}\\model.cs\" /namespace:{1}.Model /context:modelDataContext \"{0}\\model.dbml\"",
				TempDir, AppMM.DBName());

			sqlmetal = new Process();
			sqlmetal.StartInfo = new ProcessStartInfo(smname, smargs);
			sqlmetal.StartInfo.UseShellExecute = false;
			sqlmetal.StartInfo.RedirectStandardOutput = true;
			sqlmetal.Start();
			sqlmetal.WaitForExit();

			Output += "\r\n\r\n";
			Output += sqlmetal.StandardOutput.ReadToEnd();

			// Полученные файлы надо залить в БД
			string res = SaveFile("model.dbml", File.ReadAllBytes(TempDir + "\\model.dbml"), "AutoGenerate");
			if (!String.IsNullOrEmpty(res))
				return res;
			res = SaveFile("model.cs", File.ReadAllBytes(TempDir + "\\model.cs"), "AutoGenerate");
			if (!String.IsNullOrEmpty(res))
				return res;

			return string.Empty;
		}

		public static string CreateTemplateCodeFiles()
		{
			// Создать временный каталог
			if (Directory.Exists(TempDir))
				Array.ForEach((new DirectoryInfo(TempDir)).GetFiles(), o => { o.Delete(); });
			
			var res = AppMM.DataContext.usp_model();
			var res1 = res.FirstOrDefault();
			var xe = res1.Column1;
			xe.Save(TempDir + "\\model.xml");

			string[] templates = new string[] 
                {
                    "datacontextPartial",
                    "ModelInterfaces", 
                    "codifiers", 
                    "collections",
                    "hstcollections",
                    "app",
                    "chst",
                    "mlview",
                    "classes",
                    "class1",
                    "hstclasses",
                    "controllers",
                    "controllers1",
                    "controllers2",
                    "controllers3",
                    "controllers4",
                    "controllers5",
                    "classes_inhprops",
                    "pckcontrollers",
					"controllersredirect",
					"folderpredicates",
					"predicates"
                };

			foreach (var templateName in templates)
			{
				string errors = GenerateFromTemplate(TempDir, templateName);
				if (errors != String.Empty)
					return errors;
				errors = SaveFile(templateName + ".cs", File.ReadAllBytes(TempDir + "\\" + templateName + ".cs"), "AutoGenerate");
				if (errors != String.Empty)
					return errors;
			}

			return String.Empty;
		}

		public static string CreateXsltTemplateCodeFiles()
		{
			var res = AppMM.DataContext.usp_model();
			var res1 = res.FirstOrDefault();
			var xe = res1.Column1;
			xe.Save(TempDir + "\\model.xml");

			// Генерация по шаблонам XSLT
			string xsltPath = "~/CodeTemplates";
			if (!ConfigurationManager.AppSettings["CodeTemplatesPath"].IsEmpty())
				xsltPath = ConfigurationManager.AppSettings["CodeTemplatesPath"];
			xsltPath = HttpContext.Current.Server.MapPath(xsltPath);
			var xsltFiles = Directory.GetFiles(xsltPath);
			if (xsltFiles.Length > 0)
			{
				foreach (var xsltFile in xsltFiles)
				{
					XslCompiledTransform tr = new XslCompiledTransform(true);
					tr.Load(xsltFile);
					var targetPath = TempDir + "\\" + Path.GetFileNameWithoutExtension(xsltFile) + ".cs";
					tr.Transform(TempDir + "\\model.xml", targetPath);
					string errors = SaveFile(Path.GetFileNameWithoutExtension(xsltFile) + ".cs", File.ReadAllBytes(targetPath), "AutoGenerate");
					if (errors != String.Empty)
						return errors;
				}
			}

			return String.Empty;
		}

		static string SaveFile(string name, byte[] data, string subFolder)
		{
			var folder = FileStorageManager.GetFolder(SourceFolder);
			if (folder == null)
				return "Папка " + SourceFolder + " не существует, некуда сохранить файл!";
			else
			{
				var AutoGenerateFolder = FileStorageManager.GetFolder(SourceFolder + "/" + subFolder);
				if (AutoGenerateFolder == null)
				{
					AutoGenerateFolder = FileStorageManager.CreateFolder(subFolder, SourceFolder);
					AutoGenerateFolder.CheckValid();
					Base.Model.SubmitChanges();
				}

				var file = FileStorageManager.GetFile(SourceFolder + "/" + subFolder + "/" + name);
				if (file == null)
				{
					file = FileStorageManager.CreateFile(name, SourceFolder + "/" + subFolder);
				}

				file.Write(data);
				if (!file.CheckValid())
					return file.GetValidationMessages().Select(o => o.Message).Join("; ");
				Base.Model.SubmitChanges();
			}
			return String.Empty;
		}

		public static CompileConfig Config
		{
			get
			{
				CompileConfig config = HttpContext.Current.Items["CompileConfig"] as CompileConfig;
				if (config == null)
				{
					string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CompileConfig.xml");
					if (File.Exists(path))
						config = CompileConfig.LoadFromFile(path);
					else
					{
						config = new CompileConfig();
						config.CompilerVersion = "v4.0";
						config.SqlMetalPath = AppDomain.CurrentDomain.BaseDirectory + "Tools\\sqlmetal.exe";
						config.SaveToFile(path);
					}
				}
				HttpContext.Current.Items["CompileConfig"] = config;
				return config;
			}
		}

		public static string CompileModelDll()
		{
			var otherfiles = FileStorageManager.DbFiles.Where(o => o.Path.StartsWith(SourceFolder) && !o.IsDeleted).ToList();
					
			Output = "";

			// Сгенерировать DLL
			// Create the C# compiler
			CSharpCodeProvider csCompiler = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", Config.CompilerVersion } });

			// input params for the compiler
			CompilerParameters cp = new CompilerParameters();
			cp.ReferencedAssemblies.Add("mscorlib.dll");
			cp.ReferencedAssemblies.Add("System.dll");
			cp.ReferencedAssemblies.Add("System.Core.dll");
			cp.ReferencedAssemblies.Add("System.Configuration.dll");
			cp.ReferencedAssemblies.Add("System.Data.dll");
			cp.ReferencedAssemblies.Add("System.Data.Linq.dll");
			cp.ReferencedAssemblies.Add("System.Xml.dll");
			cp.ReferencedAssemblies.Add("System.Xml.Linq.dll");
			cp.ReferencedAssemblies.Add("System.Web.dll");
			cp.ReferencedAssemblies.Add("System.Web.Extensions.dll");
			cp.ReferencedAssemblies.Add("System.Web.Services.dll");
			cp.ReferencedAssemblies.Add("System.Transactions.dll");
			
			string binPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
			DirectoryInfo binDir = new DirectoryInfo(binPath);
			foreach (var dllFile in binDir.GetFiles("*.dll"))
				if (dllFile.Name != DllName)
					cp.ReferencedAssemblies.Add(dllFile.FullName);

			cp.ReferencedAssemblies.AddRange(Config.ReferencedAssembly.ToArray());

			cp.GenerateExecutable = false;
			cp.GenerateInMemory = true;

			cp.CompilerOptions = "/target:library";

			cp.IncludeDebugInformation = true;

			List<string> otherFiles = new List<string>();
			// Прочие файлы
			foreach (var f in otherfiles.Where(o => o.Extension == ".cs"))
			{
				otherFiles.Add(Encoding.UTF8.GetString(f.GetBytes()));
			}

			// Run the compiler and build the assembly
			CompilerResults cr = null;
			try
			{
				cr = csCompiler.CompileAssemblyFromSource(cp, otherFiles.ToArray());
				
				if (cr.Errors.HasErrors)
				{
					string error = "Всего ошибок и предупреждений: " + cr.Errors.Count.ToString() + "<br />";
					foreach (var errinfo in cr.Errors.Cast<CompilerError>().OrderBy(o => o.IsWarning))
					{
						int line = errinfo.Line;
						error += "<br /><br /><b>" + (errinfo.IsWarning ? "WARNING" : "ERROR") + " " + errinfo.ErrorText + " at " + errinfo.FileName + " line " + line.ToString() + " column " + errinfo.Column.ToString() + "</b><br /><br />";
						int minline = Math.Max(1, line - 30);
					}
					return error;
				}
			}
			finally
			{
				File.WriteAllText(TempDir + "\\compileroutput.txt", String.Join("\r\n", cr.Output.Cast<string>().ToArray()));
			}
			return String.Empty;
		}
	}
}