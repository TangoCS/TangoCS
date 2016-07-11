using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nephrite.Meta;
using Nephrite.Web;

namespace Nephrite.Web.LinkedObjects
{
	public class ObjectLinks
	{
		public MetaClass Class { get; set; } // Класс, на который ссылается свойство
		public MetaProperty Property { get; set; } // Свойство
		public LinkType Type { get; set; }
		public bool Aggregation { get; set; }
		public IQueryable<IModelObject> VisibleObjects { get; set; }
		public Func<int> TotalCount { get; set; }
	}

	[Flags]
	public enum LinkType
	{
		ReferencedBy = 1,	// текущий объект зависит от связанного
		References = 2,		// Связанный объект зависит от текущего
		All = 3
	}
}
