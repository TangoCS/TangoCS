using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using Nephrite.Meta;
using Nephrite.Meta.Database;
using Nephrite.Metamodel;
using Nephrite.Metamodel.Model;
using Nephrite.Web;
using Nephrite.Web.Controls;
using Nephrite.Web.CoreDataContext;
using Nephrite.Web.Hibernate;
using Nephrite.Web.MetaStorage;
using NHibernate;
using NHibernate.Cfg.Loquacious;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;


namespace Tessera.Test
{
    class Program
    {
        private static string getType(MetaClassifier type, bool notNull)
        {
            switch (type.GetType().Name)
            {
                case "MetaStringType":
                    return "new MetaStringType {Name = \"String\"}";
                case "MetaByteArrayType":
                    return "new MetaByteArrayType {Name = \"Data\"}";
                case "MetaDateType":
                    return "new MetaDateType { Name = \"Date\", NotNullable = " + notNull.ToString().ToLower() + " }";
                case "MetaDateTimeType":
                    return "new MetaDateTimeType  { Name = \"DateTime\", NotNullable = " + notNull.ToString().ToLower() + " }";
                case "MetaIntType":
                    return "new MetaIntType { Name = \"Int\", NotNullable = " + notNull.ToString().ToLower() + " }";
                case "MetaLongType":
                    return "new MetaLongType { Name = \"Long\", NotNullable = " + notNull.ToString().ToLower() + " }";
                case "MetaBooleanType":
                    return "new MetaBooleanType { Name = \"Boolean\", NotNullable = " + notNull.ToString().ToLower() + " }";
                case "MetaGuidType":
                    return "new MetaGuidType { Name = \"Guid\", NotNullable = " + notNull.ToString().ToLower() + " }";
                case "MetaDecimalType":
                    return "new MetaDecimalType { Precision = 18, Scale = 5, Name = \"Decimal\", NotNullable = " + notNull.ToString().ToLower() + " }";
                case "MetaFileType":
                    if ((type as MetaFileType).IdentifierType is MetaGuidType)
                        return " new MetaFileType { Name = \"FileGUID\", IdentifierType = TypeFactory.Guid(false), NotNullable =  " + notNull.ToString().ToLower() + " }";
                    else
                        return "new MetaFileType { Name = \"FileID\", IdentifierType = TypeFactory.Int(false), NotNullable =  " + notNull.ToString().ToLower() + " }";
                case "MetaEnum":

                    return " new MetaEnum { Name = \""+type.Name+"\" , NotNullable = " + notNull.ToString().ToLower() + " }";
                default:
                    return "";

            }

        }


        private static void Main(string[] args)
        {
            //StringBuilder ss = new StringBuilder();
            //var mapType = new DataTypeMapper();
            //ConnectionManager.SetConnectionString(
            //    "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=ServantsN;Data Source=(local)");
            //HDataContext.DBType = "MSSQL";
            //var sqlServerMetadataReader = new SqlServerMetadataReader();
            //var schema = sqlServerMetadataReader.ReadSchema("dbo");
            //foreach (var table in schema.Tables)
            //{

            //    foreach (var column in table.Value.Columns)
            //    {
            //        if (column.Value.Type is MetaGuidType)
            //        {
            //            ss.AppendLine("ALTER TABLE DBO." + table.Key.ToUpper() + " 	ALTER COLUMN " +
            //                          column.Key.ToUpper() + " SET DATA TYPE VARCHAR(36);");
            //        }
            //    }
            //    //if (table.Value.PrimaryKey != null && table.Value.PrimaryKey.Columns.Length == 1)
            //    //{
            //    //    var pkColumn =
            //    //        table.Value.Columns.SingleOrDefault(t => t.Key.ToUpper() == table.Value.PrimaryKey.Columns[0].ToUpper());
            //    //    var guidPropertyConfig = "";
            //    //    if ((pkColumn.Value.Type is Nephrite.Meta.MetaGuidType))
            //    //    {

            //    //    }
            //    //}
            //}
            //var ff = ss.ToString();
            ConnectionManager.SetConnectionString(
                "Database=servants;UserID=dbo;Password=q121212;Server=193.233.68.82:50000");
            HDataContext.DBType = "DB2";
            A.Model = new HCoreDataContext(Nephrite.Web.Hibernate.HDataContext.DBConfig(ConnectionManager.ConnectionString));

            //var objectTypesWithWorkflow = (from o in
            //                                   AppMM.DataContext.MM_ObjectTypes
            //                               join p in
            //                                   AppMM.DataContext.MM_ObjectProperties on o.ObjectTypeID equals p.ObjectTypeID
            //                               where (!o.IsTemplate && o.IsSeparateTable && !o.SysName.EndsWith("Transition") && !AppMM.DataContext.MM_ObjectTypes.Any(o1 => o1.SysName == o.SysName + "Transition")
            //                                           && p.SysName == "Activity" && p.RefObjectType != null && p.RefObjectType.SysName == "WF_Activity")
            //                               select o).ToList();
            //foreach (var cls in objectTypesWithWorkflow)
            //{
            //    foreach (
            //        var prop in
            //            cls.MM_ObjectProperties.Where(
            //                o => o.UpperBound == 1 && o.TypeCode == ObjectPropertyType.Code && o.CodifierID.HasValue))
            //    {
            //    }
            //}

            var classes = A.DynamicMeta.Classes;
            var packages = A.DynamicMeta.Packages;
            // var classes = A.DynamicMeta.Classes.Where(o => o.Name=="Appendix");
            var objectTypes = AppMM.DataContext.MM_ObjectTypes.Where(o => !o.IsTemplate).ToList();
            var allObjectProperties = AppMM.DataContext.MM_ObjectProperties;
            var dddd = classes.Count(o => o.IsPersistent);
            foreach (var _class in classes)
            {
                if (_class.Name == "OutDocSend")
                    foreach (var attribute in _class.Properties.Where(t => t is MetaAttribute))
                    {
                        var metaAttribute = attribute as MetaAttribute;
                        if (metaAttribute.Name == "UnloadTitle")
                            getType(metaAttribute.Type, metaAttribute.IsRequired);
                    }

            }

            var sdds = "";

            //var mapType = new DataTypeMapper();
            //ConnectionManager.SetConnectionString(
            //    "Database=servants;UserID=dbo;Password=q121212;Server=193.233.68.82:50000");
            //HDataContext.DBType = "DB2";
            //var sqlServerMetadataReader = new DB2ServerMetadataReader();
            //var schema = sqlServerMetadataReader.ReadSchema("dbo");
            //foreach (var table in schema.Views)
            //{
            //    if (table.Key == "V_DbFolder")
            //    {
            //    }
            //    foreach (var column in table.Value.Columns)
            //    {
            //        Type t = mapType.MapFromSqlServerDBType(column.Value.Type.GetDBType(new DBScriptMSSQL("DBO")), null, null, null);
            //    }
            //    //if (table.Value.PrimaryKey != null && table.Value.PrimaryKey.Columns.Length == 1)
            //    //{
            //    //    var pkColumn =
            //    //        table.Value.Columns.SingleOrDefault(t => t.Key.ToUpper() == table.Value.PrimaryKey.Columns[0].ToUpper());
            //    //    var guidPropertyConfig = "";
            //    //    if ((pkColumn.Value.Type is Nephrite.Meta.MetaGuidType))
            //    //    {

            //    //    }
            //    //}
            //}

            //ConnectionManager.SetConnectionString("Database=servants;UserID=dbo;Password=q121212;Server=193.233.68.82:50000");
            //HDataContext.DBType = "DB2";
            //var objectTypes = from o in  App.DataContext.DocTask
            //                  join dc in App.DataContext.Doc on o.DocID equals dc.DocID
            //                  join dt in App.DataContext.C_DocType on dc.DocTypeID equals dt.DocTypeID

            //                  where
            //                      !o.CloseDate.HasValue && !o.AnnulmentDate.HasValue &&
            //                      !o.SuspendDate.HasValue &&
            //                      (o.PlanCompleteDate <
            //                       DateTime.Today.AddDays(dt.CompleteWarning) ||
            //                       o.Doc.IsCheckDeadline)
            //                  select o;

            //var objectTypes = AppMM.DataContext.MM_ObjectTypes.Where(o => !o.IsTemplate && o.IsSeparateTable).ToList();
            //foreach (var cls in objectTypes)
            //{


            //    var allProperties =
            //        (!cls.BaseObjectTypeID.HasValue
            //             ? AppMM.DataContext.MM_ObjectProperties.Where(o => o.ObjectTypeID == cls.ObjectTypeID)
            //             : AppMM.DataContext.MM_ObjectProperties.Where(o => o.ObjectTypeID == cls.ObjectTypeID)
            //                    .ToList()
            //                    .Union(
            //                        AppMM.DataContext.MM_ObjectProperties.Where(o => o.ObjectTypeID == cls.BaseObjectTypeID)
            //                             .ToList())).ToList();
            //    //var allProperties = AppMM.DataContext.MM_ObjectProperties.Where(o => o.ObjectTypeID == cls.ObjectTypeID).ToList().Union(AppMM.DataContext.MM_ObjectProperties.Where(o => o.ObjectTypeID == cls.BaseObjectTypeID).ToList());
            //    if (
            //        allProperties.Any(
            //            o =>
            //            o.SysName == "SeqNo" && o.LowerBound == 1 && o.UpperBound == 1 &&
            //            o.TypeCode == ObjectPropertyType.Number))
            //    {
            //        MM_ObjectProperty parentprop =
            //            cls.MM_ObjectProperties.ToList().Where(o => o.RefObjectPropertyID.HasValue &&
            //                                                        o.RefObjectProperty.IsAggregate)
            //               .OrderBy(o => o.RefObjectTypeID == o.ObjectTypeID)
            //               .FirstOrDefault();

            //        if (parentprop.ObjectType.SysName == "MM_FormField")
            //        {

            //        }
            //    }
            //}
            //var allProperties = (AppMM.DataContext.MM_ObjectProperties.Where(o => o.ObjectTypeID == cls.ObjectTypeID)).ToList();
            // BaseObjectTypeID.HasValue ? BaseObjectType.MM_ObjectProperties.Union(MM_ObjectProperties) : MM_ObjectProperties; }

            //Func<string, Expression<Func<SPM_Subject, bool>>> SearchExpression = s => (o => SqlMethods.Like(o.SystemName, "%" + s + "%"));

            //bool val = false;
            //Expression<Func<SPM_Subject, bool>> column = o => o.IsActive;
            //var expr = Expression.Lambda<Func<SPM_Subject, bool>>(Expression.Equal(column.Body, Expression.Constant(val)), column.Parameters);

            //IMM_FormView r = dc.IMM_FormView.First();
            //r = ApplyFilter(r, SearchExpression, "anonymous");
            //var r2 = r.First();
            //var r = App.DataContext.V_OrgUnit.Where(o => (o.ParentOrgUnitGUID ?? Guid.Empty) == new Guid("00000000-0000-0000-0000-000000000000")).ToList();
            //r.LastModifiedDate = DateTime.Now;
            //dc.SubmitChanges();



            //var r = dc.IMailMessage.Where(o => o.LastSendAttemptDate.HasValue && (o.LastSendAttemptDate - DateTime.Today) > new TimeSpan(o.AttemptsToSendCount, 0, 0, 0)).Select(o => o.MailMessageID).ToList();

            //Console.WriteLine(App.DataContext.Log.ToString());
            Console.WriteLine(A.Model.Log.ToString());
            Console.ReadKey();
        }

        public static IQueryable<T> ApplyFilter<T>(IQueryable<T> query, Func<string, Expression<Func<T, bool>>> SearchExpression, string val)
            where T : class
        {
            return query.Where(SearchExpression(val));
        }


    }

    public class ViewData
    {
        public string UserName { get; set; }
    }


    public class SPM_Subject : IEntity, IWithTitle, IWithKey<SPM_Subject, int>, IWithTimeStamp, IWithPropertyAudit
    {
        public virtual int SubjectID { get; set; }
        public virtual string SystemName { get; set; }
        public virtual string Title { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual DateTime LastModifiedDate { get; set; }

        //int _LastModifiedUserID = 0;
        public virtual int LastModifiedUserID
        {
            get { return LastModifiedUser.SubjectID; }
            set { LastModifiedUser = new SPM_Subject { SubjectID = value }; }
        }
        public virtual SPM_Subject LastModifiedUser { get; set; }

        public virtual System.Linq.Expressions.Expression<Func<SPM_Subject, bool>> KeySelector(int id)
        {
            return o => o.SubjectID == id;
        }

        public virtual string GetTitle()
        {
            return Title;
        }
    }

    public class Employee
    {
        public virtual Guid EmployeeGUID { get; set; }
        public virtual Guid OrgUnitGUID { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return true;
        }
        public override int GetHashCode()
        {
            return EmployeeGUID.GetHashCode();
        }


    }

    public class Appendix
    {
        public virtual int AppendixID { get; set; }
        public virtual Guid? FileGUID { get; set; }
    }

    public class V_OrgUnit
    {
        public virtual Guid OrgUnitGUID { get; set; }
        public virtual Guid? ParentOrgUnitGUID { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return true;
        }
        public override int GetHashCode()
        {
            return OrgUnitGUID.GetHashCode();
        }
    }




    public class SPM_SubjectMap : ClassMapping<SPM_Subject>
    {
        public SPM_SubjectMap()
        {
            Id(x => x.SubjectID, map => map.Generator(Generators.Identity));
            Property(x => x.SystemName);
            Property(x => x.Title);
            Property(x => x.IsActive, map => map.Type<IntBackedBoolUserType>());
            Property(x => x.LastModifiedDate);

            Property(x => x.LastModifiedUserID, map => map.Formula("LastModifiedUserID"));
            ManyToOne(x => x.LastModifiedUser, map => { map.Column("LastModifiedUserID"); map.Cascade(Cascade.None); });
        }
    }

    public class AppendixMap : ClassMapping<Appendix>
    {
        public AppendixMap()
        {
            Id(x => x.AppendixID, map => map.Generator(Generators.Identity));
            Property(x => x.FileGUID, map => map.Type<StringBackedGuidUserType>());
        }
    }

    public class V_OrgUnitMap : ClassMapping<V_OrgUnit>
    {
        public V_OrgUnitMap()
        {
            ComposedId(i => i.Property(p => p.OrgUnitGUID, map =>
            {
                map.Column("OrgUnitGUID");
                map.Type<StringBackedGuidUserType>();
            }));

            Property(x => x.ParentOrgUnitGUID, map => { map.Type<StringBackedGuidUserType>(); });
        }
    }
    public class EmployeeMap : ClassMapping<Employee>
    {
        public EmployeeMap()
        {
            ComposedId(i => i.Property(p => p.EmployeeGUID, map =>
            {
                map.Column("EmployeeGUID");
                map.Type<StringBackedGuidUserType>();
            }));
            Property(x => x.OrgUnitGUID, map => map.Type<StringBackedGuidUserType>());
        }
    }

    public class App
    {
        static HibernateDataContext _dataContext = new HibernateDataContext(HDataContext.DBConfig(ConnectionManager.ConnectionString));

        public static HibernateDataContext DataContext
        {
            get { return _dataContext; }
        }
    }

    public class HibernateDataContext : HDataContext
    {
        public HibernateDataContext(Action<IDbIntegrationConfigurationProperties> dbConfig)
            : base(dbConfig)
        {
        }

        public override IEnumerable<Type> GetEntitiesTypes()
        {
            List<Type> l = new List<Type>();
            l.Add(typeof(SPM_SubjectMap));
            l.Add(typeof(EmployeeMap));
            l.Add(typeof(V_OrgUnitMap));
            l.Add(typeof(AppendixMap));

            return l;
        }

        public HTable<SPM_Subject> SPM_Subject
        {
            get
            {
                return new HTable<SPM_Subject>(this, Session.Query<SPM_Subject>());
            }
        }
        public HTable<Employee> Employee
        {
            get
            {
                return new HTable<Employee>(this, Session.Query<Employee>());
            }
        }
        public HTable<V_OrgUnit> V_OrgUnit
        {
            get
            {
                return new HTable<V_OrgUnit>(this, Session.Query<V_OrgUnit>());
            }
        }
        public HTable<Appendix> Appendix
        {
            get
            {
                return new HTable<Appendix>(this, Session.Query<Appendix>());
            }
        }


        public override IDataContext NewDataContext()
        {
            return new HibernateDataContext(DBConfig(ConnectionManager.ConnectionString));
        }

        public override IDataContext NewDataContext(string connectionString)
        {
            return new HibernateDataContext(DBConfig(connectionString));
        }
    }


}
