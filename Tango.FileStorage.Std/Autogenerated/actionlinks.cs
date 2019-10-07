using System;
using Tango.AccessControl;
using Tango.UI;
using Tango.UI.Std;

namespace Tango.FileStorage.Std.Model
{
    public static class LinksExtensions
    {
		public static ActionLink ToN_Folder_CreateNew(this ActionLink link, IAccessControl ac, int parentid, string returnurl = "this", object predicateContext = null)
		{
			link.WithArg("parentid", parentid);
			link.To<N_Folder>("CreateNew", ac, predicateContext, returnurl);
			return link;
		}
		public static ActionLink ToN_Folder_ViewList(this ActionLink link, IAccessControl ac, object predicateContext = null)
		{
			link.To<N_Folder>("ViewList", ac, predicateContext);
			return link;
		}
		public static ActionLink ToN_Folder_Upload(this ActionLink link, IAccessControl ac, string returnurl = "this", object predicateContext = null)
		{
			link.To<N_Folder>("Upload", ac, predicateContext, returnurl);
			return link;
		}
		public static ActionLink ToN_Folder_Edit(this ActionLink link, IAccessControl ac, int oid, string returnurl = "this", object predicateContext = null)
		{
			link.WithArg("oid", oid);
			link.To<N_Folder>("Edit", ac, predicateContext, returnurl);
			return link;
		}
		public static ActionLink ToN_Folder_View(this ActionLink link, IAccessControl ac, int oid, string returnurl = "this", object predicateContext = null)
		{
			link.WithArg("oid", oid);
			link.To<N_Folder>("View", ac, predicateContext, returnurl);
			return link;
		}
		public static ActionLink ToN_Folder_Delete(this ActionLink link, IAccessControl ac, int oid, string returnurl = "this", object predicateContext = null)
		{
			link.WithArg("oid", oid);
			link.To<N_Folder>("Delete", ac, predicateContext, returnurl);
			return link;
		}
		public static ActionLink ToN_Folder_PackAndDownload(this ActionLink link, IAccessControl ac, int oid, object predicateContext = null)
		{
			link.WithArg("oid", oid);
			link.To<N_Folder>("PackAndDownload", ac, predicateContext);
			return link;
		}
		public static ActionLink ToN_File_Delete(this ActionLink link, IAccessControl ac, Guid folderid, string filekey, string returnurl = "this", object predicateContext = null)
		{
			link.WithArg("folderid", folderid);
			link.WithArg("filekey", filekey);
			link.To<N_File>("Delete", ac, predicateContext, returnurl);
			return link;
		}
		public static ActionLink ToN_File_CreateNew(this ActionLink link, IAccessControl ac, int folderid, string returnurl = "this", object predicateContext = null)
		{
			link.WithArg("folderid", folderid);
			link.To<N_File>("CreateNew", ac, predicateContext, returnurl);
			return link;
		}
		public static ActionLink ToN_File_Edit(this ActionLink link, IAccessControl ac, Guid folderid, string filekey, string returnurl = "this", object predicateContext = null)
		{
			link.WithArg("folderid", folderid);
			link.WithArg("filekey", filekey);
			link.To<N_File>("Edit", ac, predicateContext, returnurl);
			return link;
		}
		public static ActionLink ToN_FileData_CreateNew(this ActionLink link, IAccessControl ac, string returnurl = "this", object predicateContext = null)
		{
			link.To<N_FileData>("CreateNew", ac, predicateContext, returnurl);
			return link;
		}
		public static ActionLink ToN_FileData_Delete(this ActionLink link, IAccessControl ac, Guid oid, string returnurl = "this", object predicateContext = null)
		{
			link.WithArg("oid", oid);
			link.To<N_FileData>("Delete", ac, predicateContext, returnurl);
			return link;
		}
		public static ActionLink ToN_FileData_ViewList(this ActionLink link, IAccessControl ac, object predicateContext = null)
		{
			link.To<N_FileData>("ViewList", ac, predicateContext);
			return link;
		}
		public static ActionLink ToN_FileData_Edit(this ActionLink link, IAccessControl ac, Guid oid, string returnurl = "this", object predicateContext = null)
		{
			link.WithArg("oid", oid);
			link.To<N_FileData>("Edit", ac, predicateContext, returnurl);
			return link;
		}
		public static ActionLink ToN_FileData_Upload(this ActionLink link, IAccessControl ac, string returnurl = "this", object predicateContext = null)
		{
			link.To<N_FileData>("Upload", ac, predicateContext, returnurl);
			return link;
		}
		public static ActionLink ToN_FileLibraryType_CreateNew(this ActionLink link, IAccessControl ac, string returnurl = "this", object predicateContext = null)
		{
			link.To<N_FileLibraryType>("CreateNew", ac, predicateContext, returnurl);
			return link;
		}
		public static ActionLink ToN_FileLibraryType_Edit(this ActionLink link, IAccessControl ac, int oid, string returnurl = "this", object predicateContext = null)
		{
			link.WithArg("oid", oid);
			link.To<N_FileLibraryType>("Edit", ac, predicateContext, returnurl);
			return link;
		}
		public static ActionLink ToN_FileLibraryType_Delete(this ActionLink link, IAccessControl ac, int oid, string returnurl = "this", object predicateContext = null)
		{
			link.WithArg("oid", oid);
			link.To<N_FileLibraryType>("Delete", ac, predicateContext, returnurl);
			return link;
		}
		public static ActionLink ToN_FileLibraryType_ViewList(this ActionLink link, IAccessControl ac, object predicateContext = null)
		{
			link.To<N_FileLibraryType>("ViewList", ac, predicateContext);
			return link;
		}
		public static ActionLink ToN_FileLibraryType_UnDelete(this ActionLink link, IAccessControl ac, int oid, string returnurl = "this", object predicateContext = null)
		{
			link.WithArg("oid", oid);
			link.To<N_FileLibraryType>("UnDelete", ac, predicateContext, returnurl);
			return link;
		}
		public static ActionLink ToN_FileLibraryType_View(this ActionLink link, IAccessControl ac, int oid, string returnurl = "this", object predicateContext = null)
		{
			link.WithArg("oid", oid);
			link.To<N_FileLibraryType>("View", ac, predicateContext, returnurl);
			return link;
		}
		public static ActionLink ToN_DownloadLog_ViewList(this ActionLink link, IAccessControl ac, object predicateContext = null)
		{
			link.To<N_DownloadLog>("ViewList", ac, predicateContext);
			return link;
		}
	}
	
}
