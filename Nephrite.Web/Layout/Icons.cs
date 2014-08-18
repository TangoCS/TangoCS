using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web.Layout
{
	public class Icons
	{
		static Icons _current = new Icons();

		/// <summary>
		/// Current set 16x16
		/// </summary>
		public static Icons X16
		{
			get { return _current; }
			set { _current = value; }
		}

		// Paging
		public virtual string Firstpage { get { return "firstpage.png"; } }
		public virtual string Prevpage { get { return "prevpage.png"; } }
		public virtual string Nextpage { get { return "nextpage.png"; } }
		public virtual string Lastpage { get { return "lastpage.png"; } }

		// Sorting
		public virtual string Sortasc { get { return "up.gif"; } }
		public virtual string Sortdesc { get { return "down.gif"; } }
		
		// Operations
		public virtual string Create { get { return "add.png"; } }
		public virtual string Add { get { return "add.png"; } }
		public virtual string Delete { get { return "delete.gif"; } }
		public virtual string Undelete { get { return "undelete.gif"; } }
		public virtual string Edit { get { return "edititem.gif"; } }
		public virtual string Properties { get { return "edititem.gif"; } }
		public virtual string Save { get { return ""; } }
		public virtual string Print { get { return ""; } }
		public virtual string Download { get { return "save.png"; } }
		public virtual string Upload { get { return ""; } }
		public virtual string Publish { get { return ""; } }
		public virtual string Run { get { return "start_job.gif"; } }
		public virtual string Import { get { return ""; } }
		public virtual string Export { get { return ""; } }
		public virtual string Filter { get { return "filter.gif"; } }
		public virtual string Moveup { get { return "arrow_up.png"; } }
		public virtual string Movedown { get { return "arrow_down.png"; } }

		// Navigation
		public virtual string Prev { get { return ""; } }
		public virtual string Next { get { return ""; } }
		public virtual string Back { get { return "back.png"; } }
		public virtual string Help { get { return ""; } }

		// UI objects
		public virtual string List { get { return ""; } }
		public virtual string Refresh { get { return ""; } }
		public virtual string Undo { get { return ""; } }
		public virtual string Redo { get { return ""; } }
		public virtual string Search { get { return ""; } }
		public virtual string User { get { return ""; } }
		public virtual string Report { get { return ""; } }
		public virtual string Mail { get { return ""; } }
		public virtual string Folder { get { return ""; } }
		public virtual string Attachment { get { return ""; } }
		public virtual string Calendar { get { return ""; } }
		public virtual string Catalog { get { return ""; } }
		public virtual string Document { get { return ""; } }
		public virtual string Setting { get { return ""; } }
		public virtual string Rss { get { return ""; } }
		public virtual string Task { get { return ""; } }
		public virtual string Workspace { get { return ""; } }
		public virtual string Select { get { return ""; } }
		public virtual string BoolTrue { get { return "tick.png"; } }

		// Messages
		public virtual string Success { get { return ""; } }
		public virtual string Error { get { return ""; } }
		public virtual string Warning { get { return ""; } }

		// List views
		public virtual string Createview { get { return "createview.gif"; } }
		public virtual string Modifyview { get { return "modifyview.gif"; } }
	}
}