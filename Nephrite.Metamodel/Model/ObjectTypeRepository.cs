using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using System.Transactions;
using Nephrite.Web;
using Nephrite.Web.SPM;
using System.Configuration;
using Microsoft.SqlServer.Management.Common;

namespace Nephrite.Metamodel.Model
{
    public class ObjectTypeRepository
    {
        modelDataContext db;

        List<Table> dropTables = new List<Table>();

        public ObjectTypeRepository()
        {
            db = AppMM.DataContext;
        }
        public static void Init(MM_ObjectType[] otlist)
        {
			objectTypes = otlist.ToDictionary(o => o.SysName.ToUpper());
        }

		static Dictionary<string, MM_ObjectType> objectTypes;
        static object locker = new Object();
        public static MM_ObjectType Get(string sysName)
        {
            if (objectTypes == null)
            {
                lock (locker)
                {
                    if (objectTypes == null)
                    {
                        objectTypes = new Dictionary<string, MM_ObjectType>();
                    }
                }
            }
			if (!objectTypes.ContainsKey(sysName.ToUpper()))
			{
				lock (locker)
				{
					if (!objectTypes.ContainsKey(sysName.ToUpper()))
					{
						using (var db = new modelDataContext(Nephrite.Web.ConnectionManager.Connection))
						{
							// Отложенная загрузка кэша приводит к ошибкам, если она осуществляется внутри транзакции.
							// Поэтому пока грузим всю метамодель...
							foreach (var ot in db.MM_ObjectTypes)
							{
								if (!objectTypes.ContainsKey(ot.SysName.ToUpper()))
								{
									objectTypes.Add(ot.SysName.ToUpper(), ot);
									var t = ot.TrackHistory;
									var m = ot.MM_Methods.Count;
									var p = ot.MM_Package.Title;
									var pp = ot.MM_ObjectProperties.Count;
									var fv = ot.MM_FormViews.Count;
									var cp = ot.ControlPath;
									var ap = ot.AllProperties.Count();
									var r = ot.AllProperties.Select(z => new 
									{ 
										z.RefObjectType, 
										z.RefObjectProperty, 
										prk = z.RefObjectTypeID.HasValue ? z.RefObjectType.PrimaryKey : (MM_ObjectProperty[])null 
									}).ToList();
								}
							}
						}
					}
				}
			}
			if (objectTypes.ContainsKey(sysName.ToUpper()))
				return objectTypes[sysName.ToUpper()];
			return null;
        }

        public MM_ObjectType Get(int id)
        {
            return db.MM_ObjectTypes.SingleOrDefault(o => o.ObjectTypeID == id);
        }

        public void DeleteObjectType(int id, bool dropTable)
        {
            var obj = Get(id);
			foreach (MM_ObjectProperty p in db.MM_ObjectProperties.Where(o => obj.MM_ObjectProperties.Contains(o.RefObjectProperty)))
				p.RefObjectProperty = null;
			foreach (MM_FormFieldGroup g in db.MM_FormFieldGroups.Where(o => obj.MM_ObjectProperties.Contains(o.MM_ObjectProperty)))
				g.MM_ObjectProperty = null;

			db.MM_FormFieldGroups.DeleteAllOnSubmit(obj.MM_FormFieldGroups);
            db.MM_ObjectProperties.DeleteAllOnSubmit(obj.MM_ObjectProperties);
			db.MM_MethodGroupItems.DeleteAllOnSubmit(db.MM_MethodGroupItems.Where(o => obj.MM_MethodGroups.Select(oo => oo.MethodGroupID).Contains(o.MethodGroupID)));
			db.MM_Methods.DeleteAllOnSubmit(obj.MM_Methods);
			db.MM_MethodGroups.DeleteAllOnSubmit(obj.MM_MethodGroups);
            db.MM_FormViews.DeleteAllOnSubmit(obj.MM_FormViews);
            db.MM_ObjectTypes.DeleteOnSubmit(obj);
			
			

            if (dropTable)
            {
                SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

                var sc = b.IntegratedSecurity ? new ServerConnection(b.DataSource) : new ServerConnection(b.DataSource, b.UserID, b.Password);
                sc.Connect();
                Server server = new Server(sc);
                TableCollection tables = server.Databases[b.InitialCatalog].Tables;
                if (tables.Contains(obj.SysName + "Data") && obj.MM_ObjectProperties.Any(o => o.IsMultilingual))
                    dropTables.Add(tables[obj.SysName + "Data"]);
                if (tables.Contains(obj.SysName))
                    dropTables.Add(tables[obj.SysName]);
            }
        }

        public IQueryable<MM_ObjectType> GetTemplates(int packageID)
        {
            return db.MM_ObjectTypes.Where(o => o.IsTemplate && o.PackageID == packageID).OrderBy(o => o.Title);
        }

        public MM_ObjectType Create(int packageID)
        {
            var obj = new MM_ObjectType { PackageID = packageID };
            
            db.MM_ObjectTypes.InsertOnSubmit(obj);
            return obj;
        }

        public void Save(MM_ObjectType obj)
        {
            if (obj.ObjectTypeID == 0)
            {
                obj.Guid = Guid.NewGuid();
                string pkname = (obj.SysName.Contains('_') ? obj.SysName.Substring(obj.SysName.LastIndexOf('_') + 1) : obj.SysName);
                obj.MM_ObjectProperties.Add(new MM_ObjectProperty
                {
                    LowerBound = 1,
                    UpperBound = 1,
                    MM_ObjectType = obj,
                    SeqNo = 1,
                    TypeCode = ObjectPropertyType.Number,
                    IsPrimaryKey = true,
                    IsSystem = true,
                    SysName = pkname + "ID",
                    Title = "Ид",
                    DeleteRule = "-",
                    Guid = Guid.NewGuid(),
                    KindCode = "P"
                });
                
                foreach (var p in obj.MM_ObjectProperties)
                    p.MM_FormField = new MM_FormField { Title = p.Title, SeqNo = p.SeqNo, MM_ObjectProperty = p };
            }
			obj.LastModifiedUserID = AppSPM.GetCurrentSubjectID();
			obj.LastModifiedDate = DateTime.Now;
            Save();
        }

        public void Save()
        {
            db.SubmitChanges();

            foreach (var table in dropTables)
                table.Drop();

            dropTables.Clear();
        }

        /*public void EnableSPM(MM_ObjectType obj)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                string packageName = obj.SecurityPackageSystemName;
                if (String.IsNullOrEmpty(packageName))
                    packageName = "InfoObject";

                Nephrite.Web.SPM.SPM_Action package = AppSPM.DataContext.SPM_Actions.SingleOrDefault(a => a.SystemName == packageName);
                if (package == null)
                {
                    package = new Nephrite.Web.SPM.SPM_Action { Type = (int)SPMActionType.BusinessPackage, SystemName = packageName, Title = packageName };
                    AppSPM.DataContext.SPM_Actions.InsertOnSubmit(package);
                }

                Nephrite.Web.SPM.SPM_Action bobject = AppSPM.DataContext.SPM_Actions.SingleOrDefault(a => a.SystemName == obj.SysName);
                if (bobject == null)
                {
                    bobject = new Nephrite.Web.SPM.SPM_Action { Type = (int)SPMActionType.BusinessObject, SystemName = obj.SysName, Title = obj.Title };
                    AppSPM.DataContext.SPM_Actions.InsertOnSubmit(bobject);
                    AppSPM.DataContext.SPM_ActionAssos.InsertOnSubmit(new Nephrite.Web.SPM.SPM_ActionAsso { SPM_Action = bobject, SPM_ParentAction = package });
                }

                string[] methods = { };//"Edit", "View", "Delete", "CreateNew", "ViewList" };
				string[] methodtitles = { };// "Редактировать", "Просмотр", "Удалить", "Создать", "Список" };

                for (int i = 0; i < methods.Length; i++)
                {
                    var m = methods[i];
                    Nephrite.Web.SPM.SPM_Action method = (from bm in AppSPM.DataContext.SPM_Actions
                                                          join asso in AppSPM.DataContext.SPM_ActionAssos on bm.ActionID equals asso.ActionID
                                                          where asso.SPM_ParentAction.SystemName == obj.SysName &&
                                                          bm.SystemName == m
                                                          select bm).FirstOrDefault();
                    if (method == null)
                    {
                        method = new Nephrite.Web.SPM.SPM_Action { Type = (int)SPMActionType.Method, SystemName = m, Title = methodtitles[i] };
                        AppSPM.DataContext.SPM_Actions.InsertOnSubmit(method);
                        AppSPM.DataContext.SPM_ActionAssos.InsertOnSubmit(new Nephrite.Web.SPM.SPM_ActionAsso { SPM_Action = method, SPM_ParentAction = bobject });
                    }

                    MM_Method method1 = obj.MM_Methods.FirstOrDefault(m1 => m1.SysName == m);
                    if (method1 == null)
                        obj.MM_Methods.Add(new MM_Method { Guid = Guid.NewGuid(), MM_ObjectType = obj, SeqNo = obj.MM_Methods.Count > 0 ? obj.MM_Methods.Max(m1 => m1.SeqNo) + 1 : 1, SysName = m, Title = methodtitles[i] });
                }
                foreach (var method in obj.MM_Methods.Where(o => !methods.Contains(o.SysName)))
                {
                    Nephrite.Web.SPM.SPM_Action spmaction = (from bm in AppSPM.DataContext.SPM_Actions
                                                             join asso in AppSPM.DataContext.SPM_ActionAssos on bm.ActionID equals asso.ActionID
                                                             where asso.SPM_ParentAction.SystemName == obj.SysName &&
                                                             bm.SystemName == method.SysName
                                                             select bm).FirstOrDefault();
                    if (spmaction == null)
                    {
                        spmaction = new Nephrite.Web.SPM.SPM_Action { Type = (int)SPMActionType.Method, SystemName = method.SysName, Title = method.Title };
                        AppSPM.DataContext.SPM_Actions.InsertOnSubmit(spmaction);
                        AppSPM.DataContext.SPM_ActionAssos.InsertOnSubmit(new Nephrite.Web.SPM.SPM_ActionAsso { SPM_Action = spmaction, SPM_ParentAction = bobject });
                    }
                }
                obj.IsEnableSPM = true;
                AppSPM.DataContext.SubmitChanges();
                AppMM.DataContext.SubmitChanges();
                ts.Complete();
            }
        }

        public void DisableSPM(MM_ObjectType obj)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                Nephrite.Web.SPM.SPM_Action[] methods = (from bm in AppSPM.DataContext.SPM_Actions
                                        join asso in AppSPM.DataContext.SPM_ActionAssos on bm.ActionID equals asso.ActionID
                                        where asso.SPM_ParentAction.SystemName == obj.SysName
                                        select bm).ToArray();
                Nephrite.Web.SPM.SPM_ActionAsso[] assos = (from asso in AppSPM.DataContext.SPM_ActionAssos
                                          where asso.SPM_ParentAction.SystemName == obj.SysName
                                          select asso).ToArray();
                AppSPM.DataContext.SPM_RoleAccesses.DeleteAllOnSubmit(from a in obj.SPM_Actions
                                                                      join r in AppSPM.DataContext.SPM_RoleAccesses on a.ActionID equals r.ActionID
                                                                      select r);
                AppSPM.DataContext.SPM_ActionAssos.DeleteAllOnSubmit(assos);
                AppSPM.DataContext.SPM_Actions.DeleteAllOnSubmit(methods);
                AppSPM.DataContext.SPM_ActionAssos.DeleteAllOnSubmit(from asso in AppSPM.DataContext.SPM_ActionAssos
                                                                     where asso.SPM_Action.SystemName == obj.SysName
                                                                     select asso);
                AppSPM.DataContext.SPM_Actions.DeleteAllOnSubmit(AppSPM.DataContext.SPM_Actions.Where(a => a.SystemName == obj.SysName));
                AppSPM.DataContext.SubmitChanges();
                obj.IsEnableSPM = false;
                AppMM.DataContext.SubmitChanges();
                ts.Complete();
            }
        }*/

        public IEnumerable<MM_ObjectType> GetList()
        {
            return db.MM_ObjectTypes.ToList().OrderBy(o => o.SysName);
        }
    }
}
