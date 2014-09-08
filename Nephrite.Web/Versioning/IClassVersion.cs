using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Nephrite.Meta;

namespace Nephrite.Web.Versioning
{
	public interface IClassVersion
	{
		int ClassVersionID { get; set; }
		DateTime ClassVersionDate { get; set; }
		int ClassVersionNumber { get; set; }
		int CreateUserID { get; set; }
		DateTime VersionStartDate { get; set; }
		DateTime? VersionEndDate { get; set; }
		bool IsCurrent { get; }
	}

	public interface IWithVersioning<T, TKey>
	{
		bool IsCurrentVersion { get; }
		int VersionNumber { get; }

		Expression<Func<T, bool>> VersionKeySelector(TKey id);
	}

	public interface IWithClassVersioning<T, TKey> : IWithVersioning<T, TKey>
	{
		int ClassVersionID { get; }
	}

	public abstract class EntityControllerWithVersioning<T, TKey> : EntityController<T, TKey>
		where T : IEntity, IWithKey<T, TKey>, IWithVersioning<T, TKey>, new()
	{
		public EntityControllerWithVersioning(string className)
			: base(className)
		{
		}

		protected override IQueryable<T> GetTable()
		{
			var table = base.GetTable();
			table = table.Where(o => o.IsCurrentVersion);
			return table;
		}

		public T GetVersion(TKey id)
		{
			return A.Model.GetTable<T>().Where(_tobj.VersionKeySelector(id)).FirstOrDefault();
		}
	}

	public abstract class EntityControllerWithClassVersioning<T, TKey, TClassVersion> : EntityControllerWithVersioning<T, TKey>
		where T : IEntity, IWithKey<T, TKey>, IWithClassVersioning<T, TKey>, new()
		where TClassVersion : IClassVersion
	{
		public EntityControllerWithClassVersioning(string className)
			: base(className)
		{
		}

		protected override IQueryable<T> GetTable()
		{
			var table = base.GetTable();

			table = table.Where(o => o.IsCurrentVersion && o.ClassVersionID == ClassVersion<TClassVersion>.CurrentVersionID);
			return table;
		}
	}
}
