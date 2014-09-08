using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Nephrite.Meta
{
	public partial class MetaClass<T> : MetaClass
	{
		public new IQueryable<T> AllObjects
		{
			get { return base.AllObjects as IQueryable<T>; }
			set { base.AllObjects = value; }
		}
		public Func<string, Expression<Func<T, bool>>> SearchExpression { get; set; }
		public Func<string, int, int, IEnumerable<T>> SearchQuery { get; set; }

		public Func<IQueryable<T>, IOrderedQueryable<T>> DefaultOrderBy { get; set; }
	}

	public partial class MetaReference<TClass, TValue> : MetaReference
	{
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

		public new Expression<Func<TClass, TValue>> GetValueIDExpression
		{
			get { return base.GetValueIDExpression as Expression<Func<TClass, TValue>>; }
			set { base.GetValueIDExpression = value; }
		}
		public new Func<TClass, TValue> GetValueID
		{
			get { return base.GetValueID as Func<TClass, TValue>; }
			set { base.GetValueID = value; }
		}
		public new Action<TClass, TValue> SetValueID
		{
			get { return base.SetValueID as Action<TClass, TValue>; }
			set { base.SetValueID = value; }
		}
	}

	public partial class MetaAttribute<TClass, TValue> : MetaAttribute
	{
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

	public partial class MetaOperation<TClass> : MetaOperation
	{
		public Func<TClass, bool> Predicate { get; set; }
	}
}