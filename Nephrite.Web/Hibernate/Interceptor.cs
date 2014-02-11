using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web.SPM;
using NHibernate;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace Nephrite.Web.Hibernate
{
	public class HDataContextSqlStatementInterceptor : EmptyInterceptor
	{
		HDataContext _dataContext;

		public HDataContextSqlStatementInterceptor(HDataContext dc)
		{
			_dataContext = dc;
		}

		public override void PreFlush(ICollection entitites)
		{
			int sid = -1;

			foreach (object obj in entitites)
			{
				if (obj is IWithTimeStamp)
				{
					if (sid == -1) sid = Subject.Current.ID;

					var obj2 = obj as IWithTimeStamp;
					obj2.LastModifiedDate = DateTime.Now;
					obj2.LastModifiedUserID = sid;
				}
			}
		}

		public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
		{
			if (!(entity is IWithoutEntityAudit))
			{
				string title = entity is IWithTitle ? (entity as IWithTitle).GetTitle() : "";
				var ua = (A.Model as IDC_EntityAudit).NewUserActivity("Создание", id.ToString(), entity.GetType().Name, title);
				(A.Model as IDC_EntityAudit).IN_ObjectChange.InsertOnSubmit(ua);
			}

			return base.OnSave(entity, id, state, propertyNames, types);
		}

		public override void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types)
		{
			if (!(entity is IWithoutEntityAudit))
			{
				string title = entity is IWithTitle ? (entity as IWithTitle).GetTitle() : "";
				var ua = (A.Model as IDC_EntityAudit).NewUserActivity("Удаление", id.ToString(), entity.GetType().Name, title);
				(A.Model as IDC_EntityAudit).IN_ObjectChange.InsertOnSubmit(ua);
			}

			base.OnDelete(entity, id, state, propertyNames, types);
		}

		public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
		{
			if (!(entity is IWithoutEntityAudit))
			{
				string title = entity is IWithTitle ? (entity as IWithTitle).GetTitle() : "";
				var ua = (A.Model as IDC_EntityAudit).NewUserActivity("Редактирование", id.ToString(), entity.GetType().Name, title);
				(A.Model as IDC_EntityAudit).IN_ObjectChange.InsertOnSubmit(ua);
			}

			return base.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types);
		}

		public override SqlString OnPrepareStatement(SqlString sql)
		{
			_dataContext.Log.WriteLine(sql.ToString());
			_dataContext.Log.WriteLine();
			return sql;
		}
	}
}