using System;
using System.Collections.Generic;
using Nephrite.EntityAudit;
using Nephrite.Identity;
using NHibernate;
using NHibernate.Event;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace Nephrite.Hibernate
{
	public class AuditEventListener : IPreInsertEventListener, IPostInsertEventListener, IPostUpdateEventListener, IPostDeleteEventListener
	{
		List<object> toInsert = new List<object>();
		Func<IdentityUser<int>> subject;
		Func<IDC_EntityAudit> auditDc;

		public AuditEventListener(Func<IdentityUser<int>> getCurrentSubject, Func<IDC_EntityAudit> getDc)
		{
			subject = getCurrentSubject;
			auditDc = getDc;
		}

		public bool OnPreInsert(PreInsertEvent e)
		{
			if (e.Entity is IWithTimeStamp)
			{
				var obj2 = e.Entity as IWithTimeStamp;
				var subj = subject();

				obj2.LastModifiedDate = DateTime.Now;
				obj2.LastModifiedUserID = subj.Id;

				Set(e.Persister, e.State, "LastModifiedDate", DateTime.Now);
				Set(e.Persister, e.State, "LastModifiedUserID", subj.Id);
				RefreshSet(e.Persister, e.State, "LastModifiedUser", obj2);
			}
			return false;
		}

		private void Set(IEntityPersister persister, object[] state, string propertyName, object value)
		{
			var index = Array.IndexOf(persister.PropertyNames, propertyName);
			if (index == -1)
				return;
			state[index] = value;
		}

		private void RefreshSet(IEntityPersister persister, object[] state, string propertyName, object obj)
		{
			var index = Array.IndexOf(persister.PropertyNames, propertyName);
			if (index == -1)
				return;
			state[index] = persister.GetPropertyValue(obj, index, EntityMode.Poco);
		}

		ISession newSession = null;
		ISession GetSession(ISession parent)
		{
			if (newSession == null || !newSession.IsOpen) newSession = parent.GetSession(EntityMode.Poco);
			return newSession;
		}

		public void OnPostInsert(PostInsertEvent e)
		{
			if (!(e.Entity is IWithoutEntityAudit))
			{
				List<object> toInsert = new List<object>();
				List<IN_ObjectPropertyChange> readOnlyColumns = new List<IN_ObjectPropertyChange>();

				var dc = auditDc();
				string title = e.Entity is IWithTitle ? (e.Entity as IWithTitle).Title : "";
				var oc = dc.NewIN_ObjectChange(subject(), "Creation", e.Id != null ? e.Id.ToString() : "", e.Entity.GetType().Name, title);
				toInsert.Add(oc);

				if (e.Entity is IWithPropertyAudit)
				{
					for (int i = 0; i < e.Persister.PropertyNames.Length; i++)
					{
						if (e.Persister.PropertyTypes[i] is ManyToOneType)
						{
							foreach (var opc in readOnlyColumns)
								if (opc.PropertySysName == e.Persister.PropertyNames[i] + "ID" || opc.PropertySysName == e.Persister.PropertyNames[i] + "GUID")
								{
									opc.PropertySysName = e.Persister.PropertyNames[i];
									break;
								}
						}
						else
						{
							var opc = dc.NewIN_ObjectPropertyChange();
							opc.ObjectChange = oc;

							opc.PropertySysName = e.Persister.PropertyNames[i];
							opc.Title = "";
							opc.NewValue = e.State[i].ToString();
							opc.NewValueTitle = e.State[i].ToString();

							if (!e.Persister.EntityMetamodel.PropertyUpdateability[i])
								readOnlyColumns.Add(opc);

							toInsert.Add(opc);
						}
					}
				}

				foreach (object obj in toInsert)
					GetSession(e.Session).SaveOrUpdate(obj);
			}
		}

		public void OnPostUpdate(PostUpdateEvent e)
		{
			if (!(e.Entity is IWithoutEntityAudit))
			{
				List<IN_ObjectPropertyChange> readOnlyColumns = new List<IN_ObjectPropertyChange>();
				var dc = auditDc();
				string title = e.Entity is IWithTitle ? (e.Entity as IWithTitle).Title : "";
				var oc = dc.NewIN_ObjectChange(subject(), "Modification", e.Id != null ? e.Id.ToString() : "", e.Entity.GetType().Name, title);
				toInsert.Add(oc);

				if (e.Entity is IWithPropertyAudit)
				{
					for (int i = 0; i < e.Persister.PropertyNames.Length; i++)
					{
						if (!object.Equals(e.OldState[i], e.State[i]))
						{
							if (e.Persister.PropertyTypes[i] is ManyToOneType)
							{
								foreach (var opc in readOnlyColumns)
									if (opc.PropertySysName == e.Persister.PropertyNames[i] + "ID" || opc.PropertySysName == e.Persister.PropertyNames[i] + "GUID")
									{
										opc.PropertySysName = e.Persister.PropertyNames[i];
										break;
									}
							}
							else
							{
								var opc = dc.NewIN_ObjectPropertyChange();
								opc.ObjectChange = oc;

								opc.PropertySysName = e.Persister.PropertyNames[i];
								opc.Title = "";
								opc.OldValue = e.OldState[i].ToString();
								opc.NewValue = e.State[i].ToString();
								opc.OldValueTitle = e.OldState[i].ToString();
								opc.NewValueTitle = e.State[i].ToString();

								if (!e.Persister.EntityMetamodel.PropertyUpdateability[i])
									readOnlyColumns.Add(opc);

								toInsert.Add(opc);
							}
						}
					}
				}

				foreach (object obj in toInsert)
					GetSession(e.Session).SaveOrUpdate(obj);
			}
		}

		public void OnPostDelete(PostDeleteEvent e)
		{
			if (!(e.Entity is IWithoutEntityAudit))
			{
				List<IN_ObjectPropertyChange> readOnlyColumns = new List<IN_ObjectPropertyChange>();

				var dc = auditDc();
				string title = e.Entity is IWithTitle ? (e.Entity as IWithTitle).Title : "";
				var oc = dc.NewIN_ObjectChange(subject(), "Deletion", e.Id != null ? e.Id.ToString() : "", e.Entity.GetType().Name, title);
				toInsert.Add(oc);

				foreach (object obj in toInsert)
					GetSession(e.Session).SaveOrUpdate(obj);
			}
		}
	}

    public class HDataContextInterceptor : EmptyInterceptor
    {
        public override SqlString OnPrepareStatement(SqlString sql)
        {
			//var hdc = A.Model as HDataContext;
			//hdc.Log.WriteLine(sql.ToString());
			//foreach (Parameter p in sql.GetParameters())
			//{
			//	hdc.Log.Write("--");
			//	hdc.Log.WriteLine(p.ToString());
			//}
			//hdc.Log.WriteLine();
            return sql;
        }

    }



}