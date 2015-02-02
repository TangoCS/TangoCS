using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Nephrite
{
	public class ItemIcon
	{
		public string X16 { get; set; }
		public string X32 { get; set; }

		public override string ToString()
		{
			return X16;
		}
	}

	public class IconSet
	{
		[DefaultValue("/i/")]
		public static string RootPath { get; set; }

		// Paging
		static ItemIcon _Firstpage = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Prevpage = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Nextpage = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Lastpage = new ItemIcon { X16 = "", X32 = "" };

		// Sorting
		static ItemIcon _TitleSortasc = new ItemIcon { X16 = "pd/up.png", X32 = "" };
		static ItemIcon _TitleSortdesc = new ItemIcon { X16 = "pd/down.png", X32 = "" };

		// Operations
		static ItemIcon _Create = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Add = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Delete = new ItemIcon { X16 = "fc16/cross.png", X32 = "" };
		static ItemIcon _Undelete = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Edit = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Properties = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Save = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Print = new ItemIcon { X16 = "fc16/printer.png", X32 = "fc32/printer.png" };
		static ItemIcon _Download = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Upload = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Publish = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Run = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Import = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Export = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Filter = new ItemIcon { X16 = "fc16/filter.png", X32 = "" };
		static ItemIcon _Moveup = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Movedown = new ItemIcon { X16 = "", X32 = "" };

		// Navigation
		static ItemIcon _Prev = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Next = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Back = new ItemIcon { X16 = "pd/back.png", X32 = "" };
		static ItemIcon _Help = new ItemIcon { X16 = "", X32 = "" };

		// UI objects
		static ItemIcon _List = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Refresh = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Undo = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Redo = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Search = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _User = new ItemIcon { X16 = "fc16/users_men_women.png", X32 = "" };
		static ItemIcon _Report = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Mail = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Folder = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Attachment = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Calendar = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Catalog = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Document = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Setting = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Rss = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Task = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Workspace = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Select = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _BoolTrue = new ItemIcon { X16 = "", X32 = "" };

		// Messages
		static ItemIcon _Success = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Error = new ItemIcon { X16 = "", X32 = "" };
		static ItemIcon _Warning = new ItemIcon { X16 = "", X32 = "" };

		// List views
		static ItemIcon _Createview = new ItemIcon { X16 = "fc16/table_add.png", X32 = "" };
		static ItemIcon _Modifyview = new ItemIcon { X16 = "fc16/table_edit.png", X32 = "" };

		#region Properties
		// Paging
		public static ItemIcon Firstpage { get { return _Firstpage; } }
		public static ItemIcon Prevpage { get { return _Prevpage; } }
		public static ItemIcon Nextpage { get { return _Nextpage; } }
		public static ItemIcon Lastpage { get { return _Lastpage; } }

		// Sorting
		public static ItemIcon TitleSortasc { get { return _TitleSortasc; } }
		public static ItemIcon TitleSortdesc { get { return _TitleSortdesc; } }
		
		// Operations
		public static ItemIcon Create { get { return _Create; } }
		public static ItemIcon Add { get { return _Add; } }
		public static ItemIcon Delete { get { return _Delete; } }
		public static ItemIcon Undelete { get { return _Undelete; } }
		public static ItemIcon Edit { get { return _Edit; } }
		public static ItemIcon Properties { get { return _Properties; } }
		public static ItemIcon Save { get { return _Save; } }
		public static ItemIcon Print { get { return _Print; } }
		public static ItemIcon Download { get { return _Download; } }
		public static ItemIcon Upload { get { return _Upload; } }
		public static ItemIcon Publish { get { return _Publish; } }
		public static ItemIcon Run { get { return _Run; } }
		public static ItemIcon Import { get { return _Import; } }
		public static ItemIcon Export { get { return _Export; } }
		public static ItemIcon Filter { get { return _Filter; } }
		public static ItemIcon Moveup { get { return _Moveup; } }
		public static ItemIcon Movedown { get { return _Movedown; } }

		// Navigation
		public static ItemIcon Prev { get { return _Prev; } }
		public static ItemIcon Next { get { return _Next; } }
		public static ItemIcon Back { get { return _Back; } }
		public static ItemIcon Help { get { return _Help; } }

		// UI objects
		public static ItemIcon List { get { return _List; } }
		public static ItemIcon Refresh { get { return _Refresh; } }
		public static ItemIcon Undo { get { return _Undo; } }
		public static ItemIcon Redo { get { return _Redo; } }
		public static ItemIcon Search { get { return _Search; } }
		public static ItemIcon User { get { return _User; } }
		public static ItemIcon Report { get { return _Report; } }
		public static ItemIcon Mail { get { return _Mail; } }
		public static ItemIcon Folder { get { return _Folder; } }
		public static ItemIcon Attachment { get { return _Attachment; } }
		public static ItemIcon Calendar { get { return _Calendar; } }
		public static ItemIcon Catalog { get { return _Catalog; } }
		public static ItemIcon Document { get { return _Document; } }
		public static ItemIcon Setting { get { return _Setting; } }
		public static ItemIcon Rss { get { return _Rss; } }
		public static ItemIcon Task { get { return _Task; } }
		public static ItemIcon Workspace { get { return _Workspace; } }
		public static ItemIcon Select { get { return _Select; } }
		public static ItemIcon BoolTrue { get { return _BoolTrue; } }

		// Messages
		public static ItemIcon Success { get { return _Success; } }
		public static ItemIcon Error { get { return _Error; } }
		public static ItemIcon Warning { get { return _Warning; } }

		// List views
		public static ItemIcon Createview { get { return _Createview; } }
		public static ItemIcon Modifyview { get { return _Modifyview; } }
		#endregion
	}
}