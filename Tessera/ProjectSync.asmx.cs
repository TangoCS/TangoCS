using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Nephrite.Metamodel;
using Nephrite.Metamodel.Model;
using System.Text;
using System.IO;
using Nephrite.Web;
using Nephrite.Web.FileStorage;

namespace Tessera
{
    /// <summary>
    /// Summary description for ProjectSync
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ProjectSync : System.Web.Services.WebService
    {

        [WebMethod]
        public string[] GetFolders(string path)
        {
            if (String.IsNullOrEmpty(path))
                path = ModelAssemblyGenerator.SourceFolder;
            else
				path = ModelAssemblyGenerator.SourceFolder + "/" + path;

			return FileStorageManager.DbFolders.Where(o => (o.Path.StartsWith(path + "/") && !path.IsEmpty()) || (o.Path.IsEmpty() && o.ParentFolderID == null)).Select(o => o.Title).ToArray();
        }

        [WebMethod]
        public FileInfo[] GetFiles(string path)
        {
            if (String.IsNullOrEmpty(path))
				path = ModelAssemblyGenerator.SourceFolder;
            else
				path = ModelAssemblyGenerator.SourceFolder + "/" + path;

			return FileStorageManager.DbFiles.Where(o => o.Path == path).
                    Select(o => new FileInfo
                    {
                        LastModifiedDate = o.LastModifiedDate,
                        Length = o.Size,
                        Title = o.Title,
                        Guid = o.ID
                    }).ToArray();
        }

        [WebMethod]
        public byte[] GetFileData(Guid fileguid)
        {
            return FileStorageManager.GetFile(fileguid).GetBytes();
        }

        [WebMethod]
        public string[] GetViewContainers(string path)
        {
            if (String.IsNullOrEmpty(path))
                return AppMM.DataContext.MM_Packages.Where(o => !o.ParentPackageID.HasValue).Select(o => o.SysName).ToArray();
            MM_Package package = AppMM.DataContext.MM_Packages.ToList().SingleOrDefault(o => o.ControlPath == path);
            if (package == null)
                return new string[0];
            return AppMM.DataContext.MM_Packages.Where(o => o.ParentPackageID == package.PackageID).
                Select(o => o.SysName).Union(AppMM.DataContext.MM_ObjectTypes.Where(o => o.PackageID == package.PackageID && o.IsSeparateTable && !o.IsTemplate).
                Select(o => o.SysName)).ToArray();
        }

        [WebMethod]
        public FormViewInfo[] GetViews(string path)
        {
            path = Nephrite.Web.Settings.ControlsPath + "/" + AppMM.DBName() + "/" + path;
            var views = AppMM.DataContext.MM_FormViews.ToList();
            return views.Where(o => o.ControlPath.Substring(0, o.ControlPath.LastIndexOf('/')) == path && o.TemplateTypeCode == "A").
                Select(o => new FormViewInfo { Guid = o.Guid, LastModifiedDate = o.LastModifiedDate, Title = o.SysName }).ToArray();
        }

        [WebMethod]
        public string GetFormViewTemplate(Guid fvguid)
        {
            var fv = AppMM.DataContext.MM_FormViews.Single(o => o.Guid == fvguid);
            FormsVirtualFile f = new FormsVirtualFile(fv.ControlPath);
            using (var sr = new System.IO.StreamReader(f.Open(), Encoding.UTF8))
                return sr.ReadToEnd();
        }

        [WebMethod]
        public DateTime SetFormViewTemplate(Guid fvguid, string text)
        {
            var fv = AppMM.DataContext.MM_FormViews.Single(o => o.Guid == fvguid);
            fv.LastModifiedDate = DateTime.Now;
            fv.ViewTemplate = text;
            AppMM.DataContext.SubmitChanges();
            return DateTime.Now;
        }

        [WebMethod]
        public DateTime SetFileData(Guid fileguid, byte[] data)
        {
			var file = FileStorageManager.GetFile(fileguid);
			file.Write(data);
			file.CheckValid();
            A.Model.SubmitChanges();
            return DateTime.Now;
        }

        [WebMethod]
        public string GetNamespace()
        {
            return AppMM.DBName();
        }

        [Serializable]
        public class FileInfo
        {
            public DateTime LastModifiedDate { get; set; }
            public string Title { get; set; }
            public long Length { get; set; }
            public Guid Guid { get; set; }
        }

        [Serializable]
        public class FormViewInfo
        {
            public DateTime LastModifiedDate { get; set; }
            public string Title { get; set; }
            public Guid Guid { get; set; }
        }
    }
}
