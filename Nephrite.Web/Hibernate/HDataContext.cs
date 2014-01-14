using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Linq;
using Nephrite.Web.CoreDataContext;

namespace Nephrite.Web.Hibernate
{
	public abstract partial class HDataContext
	{
		public IQueryable<MM_FormView> MM_FormView
		{
			get { return new HTable<MM_FormView>(this, Session.Query<MM_FormView>()); }
		}

		public IQueryable<MM_Package> MM_Package
		{
			get { return new HTable<MM_Package>(this, Session.Query<MM_Package>()); }
		}
	}
}