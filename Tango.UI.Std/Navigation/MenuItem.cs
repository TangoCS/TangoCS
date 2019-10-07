using System.Collections.Generic;

namespace Tango.UI.Navigation
{
	public class MenuItem
	{
		public string ResourceKey { get; set; }
		public string SecurableObjectKey { get; set; }

		public string MenuItemType { get; set; }

		public ActionTarget Target { get; set; }

		public string Title { get; set; }
		public string Url { get; set; }
		public string Image { get; set; }
		public int SeqNo { get; set; }

		public bool Enabled { get; set; } = true;
		public string Description { get; set; }

		public List<MenuItem> Children { get; } = new List<MenuItem>();
	}
}
