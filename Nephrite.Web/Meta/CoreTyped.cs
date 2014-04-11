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
		public IQueryable<T> AllObjects { get; set; }
		public Func<string, Expression<Func<T, bool>>> SearchExpression { get; set; }
		public Func<string, int, int, IEnumerable<T>> SearchQuery { get; set; }
	}

	public partial class MetaReference<T> : MetaReference
	{
		public IQueryable<T> AllObjects { get; set; }
		public Func<string, Expression<Func<T, bool>>> SearchExpression { get; set; }
		public Func<string, int, int, IEnumerable<T>> SearchQuery { get; set; }
	}
}