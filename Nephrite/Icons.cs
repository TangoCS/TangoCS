using System;
using System.Configuration;

namespace Nephrite
{
	public class IconSet
	{
		static Lazy<string> _rootPath = new Lazy<string>(() => ConfigurationManager.AppSettings["ImagesPath"] ?? "/i/");
		public static string RootPath { get { return _rootPath.Value; } }

		// Paging
		public const string Firstpage = "Firstpage";
		public const string Prevpage = "Prevpage";
		public const string Nextpage = "Nextpage";
		public const string Lastpage = "Lastpage";

		// Sorting
		public const string TitleSortasc = "TitleSortasc";
		public const string TitleSortdesc = "TitleSortdesc";

		// Operations
		public const string Create = "Create";
		public const string Add = "Add";
		public const string Delete = "Delete";
		public const string Undelete = "Undelete";
		public const string Edit = "Edit";
		public const string Properties = "Properties";
		public const string Save = "Save";
		public const string Print = "Print";
		public const string Download = "Download";
		public const string Upload = "Upload";
		public const string Publish = "Publish";
		public const string Run = "Run";
		public const string Import = "Import";
		public const string Export = "Export";
		public const string Filter = "Filter";
		public const string MoveUp = "MoveUp";
		public const string MoveDown = "MoveDown";

		// Navigation
		public const string Prev = "Prev";
		public const string Next = "Next";
		public const string Back = "Back";
		public const string Help = "Help";

		// UI objects
		public const string List = "List";
		public const string Refresh = "Refresh";
		public const string Undo = "Undo";
		public const string Redo = "Redo";
		public const string Search = "Search";
		public const string User = "User";
		public const string Report = "Report";
		public const string Mail = "Mail";
		public const string Folder = "Folder";
		public const string Attachment = "Attachment";
		public const string Calendar = "Calendar";
		public const string Catalog = "Catalog";
		public const string Document = "Document";
		public const string Setting = "Setting";
		public const string Rss = "Rss";
		public const string Task = "Task";
		public const string Workspace = "Workspace";
		public const string Select = "Select";
		public const string BoolTrue = "BoolTrue";

		// Messages
		public const string Success = "Success";
		public const string Error = "Error";
		public const string Warning = "Warning";

		// List views
		public const string Createview = "Createview";
		public const string Modifyview = "Modifyview";
	}
}