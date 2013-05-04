using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Nephrite.Metamodel.Model;

namespace Nephrite.Metamodel
{
	internal class FileUploadCode : ControlCode
	{
		string controlID = "";
		public FileUploadCode(MM_FormField field)
			: base(field)
		{
			controlID = "fu" + field.MM_ObjectProperty.SysName;
		}
		public FileUploadCode(MM_FormField field, string prefix)
			: base(field, prefix)
		{
			controlID = "fu" + prefix + field.MM_ObjectProperty.SysName;
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
			return "if (" + var + "." + _field.MM_ObjectProperty.SysName + " != null) \r\n\t" + controlID + ".SetFile(" +
				var + "." + _field.MM_ObjectProperty.SysName + ".FileID, " + var + "." + _field.MM_ObjectProperty.SysName + ".Title);";
		}
		public override string Save(string var)
		{
			StringBuilder s = new StringBuilder();
			
			string sv = "sfile" + _field.MM_ObjectProperty.SysName;
			string fv = "file" + _field.MM_ObjectProperty.SysName;
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

			s.AppendLine("\tif (" + var + "." + _field.MM_ObjectProperty.SysName + " != null)");
			s.AppendLine("\t{");
			s.AppendLine("\t\tApp.DataContext.N_File.DeleteOnSubmit(App.DataContext.N_File.Single(o => o.FileID == " + var + "." + _field.MM_ObjectProperty.SysName + "FileID));");
			s.AppendLine("\t}");
			s.AppendLine("\t" + var + "." + _field.MM_ObjectProperty.SysName + " = " + fv + ";");
			s.AppendLine("\t" + controlID + ".CleanTemp();");
			s.AppendLine("}");
			s.AppendLine("else if (" + controlID + ".DeleteFlag())");
			s.AppendLine("{");
			s.AppendLine("\tApp.DataContext.N_File.DeleteOnSubmit(App.DataContext.N_File.Single(o => o.FileID == " + var + "." + _field.MM_ObjectProperty.SysName + "FileID));");
			s.AppendLine("\t" + var + "." + _field.MM_ObjectProperty.SysName + " = null;");
			s.AppendLine("}");
			return s.ToString();
		}

		public override string SetDefaultOrEmpty()
		{
			return controlID + @".Clear();";
		}
	}

	internal class FileUploadSimpleCode : ControlCode
	{
		string controlID = "";
		public FileUploadSimpleCode(MM_FormField field)
			: base(field)
		{
			controlID = "fu" + field.MM_ObjectProperty.SysName;
		}
		public FileUploadSimpleCode(MM_FormField field, string prefix)
			: base(field, prefix)
		{
			controlID = "fu" + prefix + field.MM_ObjectProperty.SysName;
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
			return "if (" + var + "." + _field.MM_ObjectProperty.SysName + " != null) \r\n\t" + controlID + ".SetFile(" +
				var + "." + _field.MM_ObjectProperty.SysName + ".FileID, " + var + "." + _field.MM_ObjectProperty.SysName + ".Title);";
		}

		public override string Save(string var)
		{
			StringBuilder s = new StringBuilder();

			string sv = "sfile" + _field.MM_ObjectProperty.SysName;
			string fv = "file" + _field.MM_ObjectProperty.SysName;
			s.AppendLine("if (" + controlID + ".GetFile().HasFile)");
			s.AppendLine("{");
			s.AppendLine("\tSystem.Web.UI.WebControls.FileUpload " + sv + " = " + controlID + ".GetFile();");
            s.AppendLine("\tstring vc = VirusChecker.Check(" + sv + ");");
            s.AppendLine("\tif (vc != \"\")");
            s.AppendLine("\t{");
            var p = _field.MM_ObjectProperty.MM_ObjectType.MM_ObjectProperties1.FirstOrDefault(o => o.IsAggregate &&
                o.MM_FormField.ControlName.HasValue && o.MM_FormField.ControlName.Value == (int)FormControl.Modal);
            if (p != null)
            {
                s.AppendLine("\t\tlMsg" + p.SysName + ".Text = \"<div class='savewarning'>\" + vc + \"</div>\";");
                s.AppendLine("\t\tm" + p.SysName + ".Reopen();");
            }
            else
            {
                s.AppendLine("\t\tlMsg.Text = \"" + _field.MM_ObjectProperty.Title + ": \" + vc;");
            }
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
			s.AppendLine("\tif (" + var + "." + _field.MM_ObjectProperty.SysName + " != null)");
			s.AppendLine("\t{");
			s.AppendLine("\t\tApp.DataContext.N_File.DeleteOnSubmit(App.DataContext.N_File.Single(o => o.FileID == " + var + "." + _field.MM_ObjectProperty.SysName + "FileID));");
			s.AppendLine("\t}");
			s.AppendLine("\t" + var + "." + _field.MM_ObjectProperty.SysName + " = " + fv + ";");
			s.AppendLine("}");
			s.AppendLine("else if (" + controlID + ".DeleteFlag())");
			s.AppendLine("{");
			s.AppendLine("\tApp.DataContext.N_File.DeleteOnSubmit(App.DataContext.N_File.Single(o => o.FileID == " + var + "." + _field.MM_ObjectProperty.SysName + "FileID));");
			s.AppendLine("\t" + var + "." + _field.MM_ObjectProperty.SysName + " = null;");
			s.AppendLine("}");
			return s.ToString();
		}



		public override string SetDefaultOrEmpty()
		{
			return controlID + @".Clear();";
		}


	}

	internal class ImageUploadCode : ControlCode
	{
		string controlID = "";
		public ImageUploadCode(MM_FormField field)
			: base(field)
		{
			controlID = "iu" + field.MM_ObjectProperty.SysName;
		}
		public ImageUploadCode(MM_FormField field, string prefix)
			: base(field, prefix)
		{
			controlID = "iu" + prefix + field.MM_ObjectProperty.SysName;
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
			return "if (" + var + " ." + _field.MM_ObjectProperty.SysName + " != null) \r\n\t" + controlID + ".SetImage(" +
				var + "." + _field.MM_ObjectProperty.SysName + ".FileID, " + var + "." + _field.MM_ObjectProperty.SysName + ".Title);";
				//var + "." + _field.MM_ObjectProperty.SysName + "ImageID, " + var + "." + _field.MM_ObjectProperty.SysName + ".Name);";
		}
		public override string Save(string var)
		{
			StringBuilder s = new StringBuilder();
			string sv = "simage" + _field.MM_ObjectProperty.SysName;
			string iv = "image" + _field.MM_ObjectProperty.SysName;
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
			s.AppendLine("\tif (" + var + "." + _field.MM_ObjectProperty.SysName + " != null)");
			s.AppendLine("\t{");
			s.AppendLine("\t\tApp.DataContext.N_File.DeleteOnSubmit(App.DataContext.N_File.Single(o => o.FileID == " + var + "." + _field.MM_ObjectProperty.SysName + "FileID));");
			s.AppendLine("\t}");
			s.AppendLine("\t" + var + "." + _field.MM_ObjectProperty.SysName + " = " + iv + ";");
			s.AppendLine("}");
			s.AppendLine("else if (" + controlID + ".DeleteFlag())");
			s.AppendLine("{");
			s.AppendLine("\tApp.DataContext.N_File.DeleteOnSubmit(App.DataContext.N_File.Single(o => o.FileID == " + var + "." + _field.MM_ObjectProperty.SysName + "FileID));");
			s.AppendLine("\t" + var + "." + _field.MM_ObjectProperty.SysName + " = null;");
			s.AppendLine("}");
			
			/*s.AppendLine("\tN_Image " + iv + " = new N_Image();");
			s.AppendLine("\t" + iv + ".Title = Path.GetFileName(" + sv + ");");
			s.AppendLine("\t" + iv + ".Extension = Path.GetExtension(" + sv + ");");
			s.AppendLine("\t" + iv + ".Guid = Guid.NewGuid();");

			s.AppendLine("\tif (ViewData." + _field.MM_ObjectProperty.SysName + " != null)");
			s.AppendLine("\t{");
			s.AppendLine("\t\tApp.DataContext.N_Image.DeleteOnSubmit(App.DataContext.N_Image.Single(o => o.ImageID == " + var + "." + _field.MM_ObjectProperty.SysName + "ImageID));");
			s.AppendLine("\t}");
			s.AppendLine("\t" + var + "." + _field.MM_ObjectProperty.SysName + " = " + iv + ";");
			s.AppendLine("\t" + controlID + ".CleanTemp();");
			s.AppendLine("}");
			s.AppendLine("if (" + controlID + ".DeleteFlag())");
			s.AppendLine("{");
			s.AppendLine("\tApp.DataContext.N_Image.DeleteOnSubmit(App.DataContext.N_Image.Single(o => o.ImageID == " + var + "." + _field.MM_ObjectProperty.SysName + "ImageID));");
			s.AppendLine("\t" + var + "." + _field.MM_ObjectProperty.SysName + " = null;");
			s.AppendLine("}");*/
			return s.ToString();
		}

		/*public override string AfterSubmit()
		{
			return controlID + ".ApproveImage(image" + _field.MM_ObjectProperty.SysName + ".Guid, image" + _field.MM_ObjectProperty.SysName + ".Title);";
		}*/

		public override string SetDefaultOrEmpty()
		{
			return controlID + @".Clear();";
		}
	}
}
