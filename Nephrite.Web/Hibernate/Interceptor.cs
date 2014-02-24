using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web.SPM;
using NHibernate;
using NHibernate.Event;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace Nephrite.Web.Hibernate
{
    public class AuditEventListener : IPreUpdateEventListener, IPreInsertEventListener, IPreDeleteEventListener
    {
        public bool OnPreUpdate(PreUpdateEvent e)
        {
            if (!(e.Entity is IWithoutEntityAudit))
            {
                List<object> toInsert = new List<object>();
                List<IN_ObjectPropertyChange> readOnlyColumns = new List<IN_ObjectPropertyChange>();

                var dc = A.Model as IDC_EntityAudit;
                string title = e.Entity is IWithTitle ? (e.Entity as IWithTitle).GetTitle() : "";
                var oc = dc.NewIN_ObjectChange("Редактирование", e.Id != null ? e.Id.ToString() : "", e.Entity.GetType().Name, title);
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
                                opc.IObjectChange = oc;

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
                    e.Session.SaveOrUpdate(obj);
            }
            return false;
        }

        public bool OnPreInsert(PreInsertEvent e)
        {
            if (!(e.Entity is IWithoutEntityAudit))
            {
                List<object> toInsert = new List<object>();
                List<IN_ObjectPropertyChange> readOnlyColumns = new List<IN_ObjectPropertyChange>();

                var dc = A.Model as IDC_EntityAudit;
                string title = e.Entity is IWithTitle ? (e.Entity as IWithTitle).GetTitle() : "";
                var oc = dc.NewIN_ObjectChange("Создание", e.Id != null ? e.Id.ToString() : "", e.Entity.GetType().Name, title);
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
                            opc.IObjectChange = oc;

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
                    e.Session.SaveOrUpdate(obj);
            }
            return false;
        }

        public bool OnPreDelete(PreDeleteEvent e)
        {
            if (!(e.Entity is IWithoutEntityAudit))
            {
                List<object> toInsert = new List<object>();
                List<IN_ObjectPropertyChange> readOnlyColumns = new List<IN_ObjectPropertyChange>();

                var dc = A.Model as IDC_EntityAudit;
                string title = e.Entity is IWithTitle ? (e.Entity as IWithTitle).GetTitle() : "";
                var oc = dc.NewIN_ObjectChange("Удаление", e.Id != null ? e.Id.ToString() : "", e.Entity.GetType().Name, title);
                toInsert.Add(oc);

                foreach (object obj in toInsert)
                    e.Session.SaveOrUpdate(obj);
            }
            return false;
        }
    }

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

        public override SqlString OnPrepareStatement(SqlString sql)
        {
            _dataContext.Log.WriteLine(sql.ToString());
            _dataContext.Log.WriteLine();
            return sql;
        }
    }


}