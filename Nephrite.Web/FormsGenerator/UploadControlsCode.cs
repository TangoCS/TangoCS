using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Nephrite.Meta;

namespace Nephrite.Web.FormsGenerator
{
	internal class FileUploadCode : AttributeControlCode
	{
		string controlID = "";
		public FileUploadCode(FormElement field)
			: base(field)
		{
			controlID = "fu" + field.Name;
		}
		public FileUploadCode(FormElement field, string prefix)
			: base(field, prefix)
		{
			controlID = "fu" + prefix + field.Name;
		}
		public override string Control()
		{
			return @"<nw:FileUpload ID=""" + controlID + @""" runat=""server"" />";
		}

		public override string SetValue(string value)
		{
			throw new NotImplementedException();
		}

		public override string Load(string var)
		{
			return "if (" + var + "." + _field.Name + " != null) \r\n\t" + controlID + ".SetFile(" +
				var + "." + _field.Name + ".FileID, " + var + "." + _field.Name + ".Title);";
		}
		public override string Save(string var)
		{
			StringBuilder s = new StringBuilder();

			string sv = "sfile" + _field.Name;
			string fv = "file" + _field.Name;
			s.AppendLine("if (!String.IsNullOrEmpty(" + controlID + ".GetFile()))");
			s.AppendLine("{");
			s.AppendLine("\tstring " + sv + " = " + controlID + ".GetFile();");
			s.AppendLine("\tN_File " + fv + " = new N_File();");
			s.AppendLine("\t" + fv + ".Title = Path.GetFileName(" + sv + ");");
			s.AppendLine("\t" + fv + ".Extension = Path.GetExtension(" + sv + ");");
			s.AppendLine("\t" + fv + ".Data = File.ReadAllBytes(" + sv + ");");
			s.AppendLine("\t" + fv + ".IsDiskStorage = false;");
			s.AppendLine("\t" + fv + ".Guid = Guid.NewGuid();");
			s.AppendLine("\t" + fv + ".LastModifiedDate = DateTime.Now;");
			s.AppendLine("\t" + fv + ".LastModifiedUserID = Nephrite.Web.SPM.AppSPM.GetCurrentSubjectID();");
			s.AppendLine("\t" + fv + ".Length = " + fv + ".Data.Length;");
			s.AppendLine("\t" + fv + ".FolderID = App.DataContext.N_Folder.Single(o => o.FullPath == Nephrite.Web.App.AppSettings.Get(\"filefolder\")).FolderID;");

			s.AppendLine("\tif (" + var + "." + _field.Name + " != null)");
			s.AppendLine("\t{");
			s.AppendLine("\t\tApp.DataContext.N_File.DeleteOnSubmit(App.DataContext.N_File.Single(o => o.FileID == " + var + "." + _field.Name + "FileID));");
			s.AppendLine("\t}");
			s.AppendLine("\t" + var + "." + _field.Name + " = " + fv + ";");
			s.AppendLine("\t" + controlID + ".CleanTemp();");
			s.AppendLine("}");
			s.AppendLine("else if (" + controlID + ".DeleteFlag())");
			s.AppendLine("{");
			s.AppendLine("\tApp.DataContext.N_File.DeleteOnSubmit(App.DataContext.N_File.Single(o => o.FileID == " + var + "." + _field.Name + "FileID));");
			s.AppendLine("\t" + var + "." + _field.Name + " = null;");
			s.AppendLine("}");
			return s.ToString();
		}

		public override string SetDefaultOrEmpty()
		{
			return controlID + @".Clear();";
		}
	}

	internal class FileUploadSimpleCode : AttributeControlCode
	{
		string controlID = "";
		public FileUploadSimpleCode(FormElement field)
			: base(field)
		{
			controlID = "fu" + field.Name;
		}
		public FileUploadSimpleCode(FormElement field, string prefix)
			: base(field, prefix)
		{
			controlID = "fu" + prefix + field.Name;
		}

		public override string Control()
		{
			return @"<nw:FileUploadSimple ID=""" + controlID + @""" runat=""server"" />";
		}

		public override string SetValue(string value)
		{
			throw new NotImplementedException();
		}

		public override string Load(string var)
		{
			return "if (" + var + "." + _field.Name + " != null) \r\n\t" + controlID + ".SetFile(" +
				var + "." + _field.Name + ".FileID, " + var + "." + _field.Name + ".Title);";
		}

		public override string Save(string var)
		{
			StringBuilder s = new StringBuilder();

			string sv = "sfile" + _field.Name;
			string fv = "file" + _field.Name;
			s.AppendLine("if (" + controlID + ".GetFile().HasFile)");
			s.AppendLine("{");
			s.AppendLine("\tSystem.Web.UI.WebControls.FileUpload " + sv + " = " + controlID + ".GetFile();");
			s.AppendLine("\tstring vc = VirusChecker.Check(" + sv + ");");
			s.AppendLine("\tif (vc != \"\")");
			s.AppendLine("\t{");
			s.AppendLine("\t\tlMsg.Text = \"" + _field.Caption + ": \" + vc;");

			s.AppendLine("\t\treturn;");
			s.AppendLine("\t}");
			s.AppendLine("\tN_File " + fv + " = new N_File();");
			s.AppendLine("\t" + fv + ".Title = Path.GetFileName(" + sv + ".FileName);");
			s.AppendLine("\t" + fv + ".Extension = Path.GetExtension(" + sv + ".FileName);");
			s.AppendLine("\t" + fv + ".Data = " + sv + ".FileBytes;");
			s.AppendLine("\t" + fv + ".IsDiskStorage = false;");
			s.AppendLine("\t" + fv + ".Guid = Guid.NewGuid();");
			s.AppendLine("\t" + fv + ".LastModifiedDate = DateTime.Now;");
			s.AppendLine("\t" + fv + ".LastModifiedUserID = Nephrite.Web.SPM.AppSPM.GetCurrentSubjectID();");
			s.AppendLine("\t" + fv + ".Length = " + fv + ".Data.Length;");
			s.AppendLine("\t" + fv + ".FolderID = App.DataContext.N_Folder.Single(o => o.FullPath == Nephrite.Web.App.AppSettings.Get(\"filefolder\")).FolderID;");
			s.AppendLine("\tif (" + var + "." + _field.Name + " != null)");
			s.AppendLine("\t{");
			s.AppendLine("\t\tApp.DataContext.N_File.DeleteOnSubmit(App.DataContext.N_File.Single(o => o.FileID == " + var + "." + _field.Name + "FileID));");
			s.AppendLine("\t}");
			s.AppendLine("\t" + var + "." + _field.Name + " = " + fv + ";");
			s.AppendLine("}");
			s.AppendLine("else if (" + controlID + ".DeleteFlag())");
			s.AppendLine("{");
			s.AppendLine("\tApp.DataContext.N_File.DeleteOnSubmit(App.DataContext.N_File.Single(o => o.FileID == " + var + "." + _field.Name + "FileID));");
			s.AppendLine("\t" + var + "." + _field.Name + " = null;");
			s.AppendLine("}");
			return s.ToString();
		}



		public override string SetDefaultOrEmpty()
		{
			return controlID + @".Clear();";
		}


	}

	internal class ImageUploadCode : AttributeControlCode
	{
		string controlID = "";
		public ImageUploadCode(FormElement field)
			: base(field)
		{
			controlID = "iu" + field.Name;
		}
		public ImageUploadCode(FormElement field, string prefix)
			: base(field, prefix)
		{
			controlID = "iu" + prefix + field.Name;
		}
		public override string Control()
		{
			return @"<nw:ImageUploadSimple ID=""" + controlID + @""" runat=""server"" />";
		}
		public override string SetValue(string value)
		{
			throw new NotImplementedException();
		}
		public override string Load(string var)
		{
			return "if (" + var + " ." + _field.Name + " != null) \r\n\t" + controlID + ".SetImage(" +
				var + "." + _field.Name + ".FileID, " + var + "." + _field.Name + ".Title);";
			//var + "." + _field.MM_ObjectProperty.SysName + "ImageID, " + var + "." + _field.MM_ObjectProperty.SysName + ".Name);";
		}
		public override string Save(string var)
		{
			StringBuilder s = new StringBuilder();
			string sv = "simage" + _field.Name;
			string iv = "image" + _field.Name;
			s.AppendLine("if (" + controlID + ".GetImage().HasFile)");
			s.AppendLine("{");
			s.AppendLine("\tSystem.Web.UI.WebControls.FileUpload " + sv + " = " + controlID + ".GetImage();");

			s.AppendLine("\tN_File " + iv + " = new N_File();");
			s.AppendLine("\t" + iv + ".Title = Path.GetFileName(" + sv + ".FileName);");
			s.AppendLine("\t" + iv + ".Extension = Path.GetExtension(" + sv + ".FileName);");
			s.AppendLine("\t" + iv + ".Data = " + sv + ".FileBytes;");
			s.AppendLine("\t" + iv + ".IsDiskStorage = false;");
			s.AppendLine("\t" + iv + ".Guid = Guid.NewGuid();");
			s.AppendLine("\t" + iv + ".LastModifiedDate = DateTime.Now;");
			s.AppendLine("\t" + iv + ".LastModifiedUserID = Nephrite.Web.SPM.AppSPM.GetCurrentSubjectID();");
			s.AppendLine("\t" + iv + ".Length = " + iv + ".Data.Length;");
			s.AppendLine("\t" + iv + ".FolderID = App.DataContext.N_Folder.Single(o => o.FullPath == Nephrite.Web.App.AppSettings.Get(\"imagefolder\")).FolderID;");
			s.AppendLine("\tif (" + var + "." + _field.Name + " != null)");
			s.AppendLine("\t{");
			s.AppendLine("\t\tApp.DataContext.N_File.DeleteOnSubmit(App.DataContext.N_File.Single(o => o.FileID == " + var + "." + _field.Name + "FileID));");
			s.AppendLine("\t}");
			s.AppendLine("\t" + var + "." + _field.Name + " = " + iv + ";");
			s.AppendLine("}");
			s.AppendLine("else if (" + controlID + ".DeleteFlag())");
			s.AppendLine("{");
			s.AppendLine("\tApp.DataContext.N_File.DeleteOnSubmit(App.DataContext.N_File.Single(o => o.FileID == " + var + "." + _field.Name + "FileID));");
			s.AppendLine("\t" + var + "." + _field.Name + " = null;");
			s.AppendLine("}");
			return s.ToString();
		}

		public override string SetDefaultOrEmpty()
		{
			return controlID + @".Clear();";
		}
	}
}