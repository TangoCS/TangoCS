using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Nephrite.Meta
{
	public interface ITypedMetaProperty<TClass, TValue>
	{
		Func<TClass, TValue> GetValue { get; set; }
		Action<TClass, TValue> SetValue { get; set; }
	}

	public interface ITypedPersistentMetaProperty<TClass, TValue>
	{
		Expression<Func<TClass, TValue>> GetValueExpression { get; set; }
	}

	public partial class MetaClass<T> : MetaClass
	{
		public IQueryable<T> AllObjects { get; set; }
		public Func<string, Expression<Func<T, bool>>> SearchExpression { get; set; }
		public Func<string, int, int, IEnumerable<T>> SearchQuery { get; set; }
	}

	public partial class MetaReference<TClass, TRefClass, TKey> : MetaReference, 
		ITypedMetaProperty<TClass, TRefClass>, 
		ITypedPersistentMetaProperty<TClass, TRefClass>
	{
		public IQueryable<TRefClass> AllObjects { get; set; }
		public Func<string, Expression<Func<TRefClass, bool>>> SearchExpression { get; set; }
		public Func<string, int, int, IEnumerable<TRefClass>> SearchQuery { get; set; }

		public Expression<Func<TClass, TRefClass>> GetValueExpression { get; set; }
		public Func<TClass, TRefClass> GetValue { get; set; }
		public Action<TClass, TRefClass> SetValue { get; set; }

		public Expression<Func<TClass, TKey>> GetValueIDExpression { get; set; }
		public Func<TClass, TKey> GetValueID { get; set; }
		public Action<TClass, TKey> SetValueID { get; set; }	
	}

	public partial class MetaAttribute<TClass, TValue> : MetaAttribute,
		ITypedMetaProperty<TClass, TValue>,
		ITypedPersistentMetaProperty<TClass, TValue>
	{
		public Expression<Func<TClass, TValue>> GetValueExpression { get; set; }
		public Func<TClass, TValue> GetValue { get; set; }
		public Action<TClass, TValue> SetValue { get; set; }
	}

	public partial class MetaComputedAttribute<TClass, TValue> : MetaProperty, 
		ITypedMetaProperty<TClass, TValue>
	{
		public Func<TClass, TValue> GetValue { get; set; }
		public Action<TClass, TValue> SetValue { get; set; }
	}

	public partial class MetaPersistentComputedAttribute<TClass, TValue> : MetaProperty, 
		ITypedMetaProperty<TClass, TValue>, 
		ITypedPersistentMetaProperty<TClass, TValue>
	{
		public Expression<Func<TClass, TValue>> GetValueExpression { get; set; }
		public Func<TClass, TValue> GetValue { get; set; }
		public Action<TClass, TValue> SetValue { get; set; }
	}
}