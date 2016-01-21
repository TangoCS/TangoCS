using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Nephrite.Multilanguage;

namespace Nephrite.Meta
{
	public partial class MetaClass<T, TKey> : MetaClass
	{
		public MetaClass(ITextResource textResource = null) : base(textResource)
		{
		}

		public new IQueryable<T> AllObjects
		{
			get { return base.AllObjects as IQueryable<T>; }
			set { base.AllObjects = value; }
		}
		public Func<string, Expression<Func<T, bool>>> SearchExpression { get; set; }
		public Func<string, int, int, IEnumerable<T>> SearchQuery { get; set; }

		public Func<IQueryable<T>, IOrderedQueryable<T>> DefaultOrderBy { get; set; }
	}

	public partial class MetaReference<TClass, TValue, TKey> : MetaReference
	{
		public MetaReference(string name, string caption, string refClassName, ITextResource textResource = null, bool isRequired = false,
			int upperBound = 1, AssociationType associationType = AssociationType.Default,
			string inversePropertyName = "", string description = "") : base(name, caption, refClassName, textResource, isRequired,
			upperBound, associationType, inversePropertyName, description)
		{
		}

		public new IQueryable<TValue> AllObjects
		{
			get { return base.AllObjects as IQueryable<TValue>; }
			set { base.AllObjects = value; }
		}
		public Func<string, Expression<Func<TValue, bool>>> SearchExpression { get; set; }
		public Func<string, int, int, IEnumerable<TValue>> SearchQuery { get; set; }

		public new Expression<Func<TClass, TValue>> GetValueExpression
		{
			get { return base.GetValueExpression as Expression<Func<TClass, TValue>>; }
			set { base.GetValueExpression = value; }
		}
		public new Func<TClass, TValue> GetValue
		{
			get { return base.GetValue as Func<TClass, TValue>; }
			set { base.GetValue = value; }
		}
		public new Action<TClass, TValue> SetValue
		{
			get { return base.SetValue as Action<TClass, TValue>; }
			set { base.SetValue = value; }
		}

		public new Expression<Func<TClass, TKey>> GetValueIDExpression
		{
			get { return base.GetValueIDExpression as Expression<Func<TClass, TKey>>; }
			set { base.GetValueIDExpression = value; }
		}
		public new Func<TClass, TKey> GetValueID
		{
			get { return base.GetValueID as Func<TClass, TKey>; }
			set { base.GetValueID = value; }
		}
		public new Action<TClass, TKey> SetValueID
		{
			get { return base.SetValueID as Action<TClass, TKey>; }
			set { base.SetValueID = value; }
		}
	}

	public partial class MetaAttribute<TClass, TValue> : MetaAttribute
	{
		public MetaAttribute(ITextResource textResource = null) : base(textResource)
		{
		}

		public new Expression<Func<TClass, TValue>> GetValueExpression 
		{ 
			get { return base.GetValueExpression as Expression<Func<TClass, TValue>>; }
			set { base.GetValueExpression = value; } 
		}
		public new Func<TClass, TValue> GetValue 
		{
			get { return base.GetValue as Func<TClass, TValue>; }
			set { base.GetValue = value; } 
		}
		public new Action<TClass, TValue> SetValue
		{
			get { return base.SetValue as Action<TClass, TValue>; }
			set { base.SetValue = value; }
		}
	}

	public partial class MetaComputedAttribute<TClass, TValue> : MetaComputedAttribute
	{
		public MetaComputedAttribute(ITextResource textResource = null) : base(textResource)
		{
		}

		public new Func<TClass, TValue> GetValue
		{
			get { return base.GetValue as Func<TClass, TValue>; }
			set { base.GetValue = value; }
		}
		public new Action<TClass, TValue> SetValue
		{
			get { return base.SetValue as Action<TClass, TValue>; }
			set { base.SetValue = value; }
		}

	}

	public partial class MetaPersistentComputedAttribute<TClass, TValue> : MetaPersistentComputedAttribute
	{
		public MetaPersistentComputedAttribute(ITextResource textResource = null) : base(textResource)
		{
		}

		public new Expression<Func<TClass, TValue>> GetValueExpression
		{
			get { return base.GetValueExpression as Expression<Func<TClass, TValue>>; }
			set { base.GetValueExpression = value; }
		}
		public new Func<TClass, TValue> GetValue
		{
			get { return base.GetValue as Func<TClass, TValue>; }
			set { base.GetValue = value; }
		}
		public new Action<TClass, TValue> SetValue
		{
			get { return base.SetValue as Action<TClass, TValue>; }
			set { base.SetValue = value; }
		}
	}

	//public partial class MetaOperation<TDelegate> : MetaOperation
	//{
	//	public TDelegate Delegate { get; set; }
	//}
}