using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using Nephrite.Web.Hibernate;

namespace Nephrite.Metamodel.Model
{
	public interface IDC_MM_ObjectType<T> where T : IQueryable<MM_ObjectType>, ITable
	{
		T MM_ObjectType { get; }
	}

	public interface IDC_MM_ObjectProperty<T> where T : IQueryable<MM_ObjectProperty>, ITable
	{
		T MM_ObjectProperty { get; }
	}


}