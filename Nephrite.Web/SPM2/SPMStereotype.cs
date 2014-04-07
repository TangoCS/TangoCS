using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Meta;
using System.Linq.Expressions;
using System.IO;

namespace Nephrite.Web.SPM
{
	public class SSPM : MetaStereotype
	{
		public SSPM()
		{
			//ID = new Guid("8572AC00-42C2-11E1-B555-75714824019B");
			Name = this.GetType().Name;
			Caption = "Для сущности настраиваются права доступа";
		}

		Dictionary<int, ActionType> _types = new Dictionary<int, ActionType>();
		public Dictionary<int, ActionType> ActionTypes { get { return _types; } }

		public Func<int, IQueryable> GetItems { get; set; }
		public string ItemsValueProperty { get; set; }
		public string ItemsTitleProperty { get; set; }
		public string ItemsParentProperty { get; set; }

		public Func<string, IQueryable<ActionInfo>> GetActions { get; set; }
	}

	public class ActionType
	{
		public int ID { get; set; }
		public string Title { get; set; }

		public Func<Guid, IQueryable<PredicateInfo>> GetPredicates { get; set; }
	}

	public class ActionInfo
	{
		public int ID { get; set; }
		public int ActionTypeID { get; set; }
		public Guid ItemGUID { get; set; }
		public string Title { get; set; }
	}

	public class ItemInfo
	{
		public Guid ID { get; set; }
		public Guid? ParentID { get; set; }
		public string Title { get; set; }
	}

	public class PredicateInfo
	{
		public Guid ID { get; set; }
		public string Title { get; set; }
	}
}