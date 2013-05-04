using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Nephrite.Metamodel.Model;
using System.Reflection;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Linq;
using Nephrite.Web;
using System.Linq.Expressions;
using System.ComponentModel;
using System.IO;
using System.Collections;
using Nephrite.Web.FileStorage;

namespace Nephrite.Metamodel
{ 
    public class Repository
    {
        Assembly assembly;
        DataContext db;
        string ns;
        string defaultLanguage = "ru";

        public Repository(string assemblyFileName)
		{
			try
			{
				if (assemblyFileName == "default")
					assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(o => o.GetName().Name.ToUpper() == ConfigurationManager.AppSettings["ModelAssembly"].ToUpper());
				else
				{
					assemblyFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyFileName);
					assembly = Assembly.LoadFile(assemblyFileName);
				}
			}
			catch(FileNotFoundException)
			{
				throw new FileNotFoundException("Файл не найден: " + assemblyFileName);
			}
			var mdc = assembly.GetTypes().Single(o => o.Name == "modelDataContext");
            ns = mdc.Namespace;
			db = Activator.CreateInstance(mdc, ConnectionManager.Connection) as DataContext;
			db.Log = new StringWriter();
			db.CommandTimeout = 300;
        }
		
		public Repository(Assembly a, DataContext d, string n)
		{
			assembly = a;
			db = d;
			ns = n;
			defaultLanguage = AppMM.DefaultLanguage;
		}

        public Repository()
        {
            if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["ModelAssembly"]))
            {
                SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(ConnectionManager.ConnectionString);

                assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(o => o.GetName().Name.ToUpper() == csb.InitialCatalog.ToUpper() + ".MODEL");
            }
            else
            {
                assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(o => o.GetName().Name.ToUpper() == ConfigurationManager.AppSettings["ModelAssembly"].ToUpper());
				if (assembly == null)
					throw new Exception("Сборка не найдена в текущем домене: " + ConfigurationManager.AppSettings["ModelAssembly"]);
            }
			if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["ModelNamespace"]))
				ns = ConfigurationManager.AppSettings["ModelNamespace"] + ".Model";
			else
				ns = assembly.GetName().Name;


            if (HttpContext.Current.Items[AppMM.DBName() + "DataContext"] == null)
            {
				db = Activator.CreateInstance(assembly.GetTypes().Single(o => o.Name == "modelDataContext"), ConnectionManager.Connection) as DataContext;
                HttpContext.Current.Items[AppMM.DBName() + "DataContext"] = db;
                db.Log = new StringWriter();
            }
            db = (DataContext)HttpContext.Current.Items[AppMM.DBName() + "DataContext"];
			db.CommandTimeout = 300;

            defaultLanguage = AppMM.DefaultLanguage;
        }

        public IMMObject Get(MM_ObjectType objectType, object id)
        {
			try
			{
				Type T = assembly.GetType(ns + "." + objectType.SysName, true, true);
				// Параметр лямбда-выражения типа T
				ParameterExpression pe_c = ParameterExpression.Parameter(typeof(IMMObject), "c");
				// Преобразование IMMObject к нужному нам реальному типу объекта
				UnaryExpression ue_c = UnaryExpression.Convert(pe_c, T);
				// Получение у объекта свойства с именем, соответствующим первичному ключу
				MemberExpression me_id = MemberExpression.Property(ue_c, objectType.PrimaryKey.Single().ColumnName);
				// Константа, по которой будем искать объект
				ConstantExpression ce_val = ConstantExpression.Constant(id, id.GetType());
				// Сравнение первичного ключа с заданным идентификатором
				BinaryExpression be_eq = BinaryExpression.Equal(me_id, ce_val);
				// Само лямбда-выражение
				Expression<Func<IMMObject, bool>> expr2 = Expression.Lambda<Func<IMMObject, bool>>(be_eq, pe_c);
				// Загрузить нужный объект
				return db.GetTable(T).OfType<IMMObject>().SingleOrDefault(expr2);
			}
			catch (Exception e)
			{
				throw new Exception("Не удалось загрузить объект " + objectType.FullSysName + ", " + id.ToString(), e);
			}
        }

        public IMMObjectVersion GetVersion(MM_ObjectType objectType, object id)
        {
            Type T = assembly.GetType(ns + ".HST_" + objectType.SysName, true, true);
            // Параметр лямбда-выражения типа T
            ParameterExpression pe_c = ParameterExpression.Parameter(typeof(IMMObjectVersion), "c");
            // Преобразование IMMObject к нужному нам реальному типу объекта
            UnaryExpression ue_c = UnaryExpression.Convert(pe_c, T);
            // Получение у объекта свойства с именем, соответствующим первичному ключу
			string pkname = objectType.PrimaryKey.Single().ColumnName;
			if (pkname.EndsWith("GUID"))
				pkname = pkname.Substring(0, pkname.Length - 4);
            else if (pkname.EndsWith("ID"))
                pkname = pkname.Substring(0, pkname.Length - 2);
			pkname += "Version" + (objectType.PrimaryKey.Single().TypeCode == ObjectPropertyType.Guid ? "GUID" : "ID");
            MemberExpression me_id = MemberExpression.Property(ue_c, pkname);
            // Константа, по которой будем искать объект
            ConstantExpression ce_val = ConstantExpression.Constant(id, id.GetType());
            // Сравнение первичного ключа с заданным идентификатором
            BinaryExpression be_eq = BinaryExpression.Equal(me_id, ce_val);
            // Само лямбда-выражение
            Expression<Func<IMMObjectVersion, bool>> expr2 = Expression.Lambda<Func<IMMObjectVersion, bool>>(be_eq, pe_c);
            // Загрузить нужный объект
            return db.GetTable(T).OfType<IMMObjectVersion>().SingleOrDefault(expr2);
        }

        public IMMObject GetData(MM_ObjectType objectType, int id, string languageCode)
        {
            Type T = assembly.GetType(ns + "." + objectType.SysName + "Data", true, true);
            // Параметр лямбда-выражения типа Object
            ParameterExpression pe_c = ParameterExpression.Parameter(typeof(IMMObject), "c");
            // Преобразование Object к нужному нам реальному типу объекта
            UnaryExpression ue_c = UnaryExpression.Convert(pe_c, T);
            // Получение у объекта свойства с именем, соответствующим первичному ключу
			MemberExpression me_id = MemberExpression.Property(ue_c, objectType.PrimaryKey.Single().ColumnName);
            // Константа, по которой будем искать объект
            ConstantExpression ce_val = ConstantExpression.Constant(id, typeof(Int32));
            // Сравнение первичного ключа с заданным идентификатором
            BinaryExpression be_eq = BinaryExpression.Equal(me_id, ce_val);
            // Получение свойства с именем LanguageCode
            MemberExpression me_lc = MemberExpression.Property(ue_c, "LanguageCode");
            // Константа, по которой будем искать объект
            ConstantExpression ce_vallc = ConstantExpression.Constant(languageCode, typeof(string));
            // Сравнение первичного ключа с заданным идентификатором
            BinaryExpression be_eqlc = BinaryExpression.Equal(me_lc, ce_vallc);
            // Логическое И
            BinaryExpression be_and = BinaryExpression.And(be_eq, be_eqlc);
            // Само лямбда-выражение
            Expression<Func<IMMObject, bool>> expr2 = Expression.Lambda<Func<IMMObject, bool>>(be_and, pe_c);
            // Загрузить нужный объект
            return db.GetTable(T).OfType<IMMObject>().SingleOrDefault<IMMObject>(expr2);
        }

        public XElement LoadObject(MM_ObjectType objectType, int id, string languageCode)
        {
            // Загрузить нужный объект
            object o = Get(objectType, id);
            if (o == null)
                return null;
            // Преобразовать объект в XElement
            XElement xe = new XElement(objectType.SysName);
            foreach (var p in objectType.MM_ObjectProperties.Where(p1 => !p1.IsMultilingual))
            {
                PropertyInfo pi = o.GetType().GetProperties().Single(p1 => p1.Name.ToLower() == p.SysName.ToLower());
                xe.Add(new XElement(p.SysName, TypeDescriptor.GetConverter(pi.PropertyType).ConvertToInvariantString(pi.GetValue(o, null))));
            }

            object odata = null;
            foreach (var p in objectType.MM_ObjectProperties.Where(p1 => p1.IsMultilingual))
            {
                if (odata == null)
                {
                    odata = GetData(objectType, id, languageCode ?? defaultLanguage);
                    if (odata == null)
                        break;
                }
                PropertyInfo pi = odata.GetType().GetProperties().Single(p1 => p1.Name.ToLower() == p.SysName.ToLower());
                xe.Add(new XElement(p.SysName, TypeDescriptor.GetConverter(pi.PropertyType).ConvertToInvariantString(pi.GetValue(odata, null))));
            }

            return xe;
        }

        public XElement ExportObject(MM_ObjectType objectType, object id)
        {
			if (objectType.SysName == "DbFile")
			{
				var f = FileStorageManager.GetFile((Guid)id);
				return f == null ? null : f.SerializeToXml();
			}
            // Загрузить нужный объект
            IMMObject o = Get(objectType, id);
            if (o == null)
                return null;
            // Преобразовать объект в XElement
            XElement xe = new XElement(objectType.SysName);
            foreach (var p in objectType.MM_ObjectProperties.Where(p1 => !p1.IsMultilingual && String.IsNullOrEmpty(p1.Expression)))
            {
                string pname = p.SysName.ToLower();
				if (p.UpperBound == 1 || p.TypeCode != ObjectPropertyType.Object)
				{
					if (p.TypeCode == ObjectPropertyType.Object)
						pname = pname + (p.RefObjectType != null && p.RefObjectType.PrimaryKey.Single().TypeCode == ObjectPropertyType.Guid ? "guid" : "id");
					PropertyInfo pi = GetPropertyInfo(o, pname);
					if (pi != null)
					{
						object val = pi.GetValue(o, null);
						if (p.TypeCode == ObjectPropertyType.File)
						{
							if (val != null)
							{
								PropertyInfo pi_g = val.GetType().GetProperty("Guid");
								val = pi_g.GetValue(val, null);
								xe.Add(new XElement(p.SysName, val.ToString()));
							}
							else
							{
								xe.Add(new XElement(p.SysName, ""));
							}
						}
						else if (p.TypeCode == ObjectPropertyType.ZoneDateTime)
						{
							if (val != null)
								xe.Add(new XElement(p.SysName, TypeDescriptor.GetConverter(pi.PropertyType).ConvertToInvariantString(val)));
							else
								xe.Add(new XElement(p.SysName, ""));

							pi = GetPropertyInfo(o, pname + "TimeZoneID");
							val = pi.GetValue(o, null);
							if (val != null)
								xe.Add(new XElement(p.SysName + "TimeZoneID", TypeDescriptor.GetConverter(pi.PropertyType).ConvertToInvariantString(val)));
							else
								xe.Add(new XElement(p.SysName + "TimeZoneID", ""));
						}
						else if (p.TypeCode == ObjectPropertyType.Data)
						{
							if (val != null)
							{
								xe.Add(new XElement(p.SysName, Convert.ToBase64String(((Binary)val).ToArray())));
							}
						}
						else
							xe.Add(new XElement(p.SysName, SanitizeXmlString(TypeDescriptor.GetConverter(pi.PropertyType).ConvertToInvariantString(val))));
					}
				}
				if (p.UpperBound != 1 && p.TypeCode == ObjectPropertyType.Object && !(p.RefObjectPropertyID.HasValue && p.RefObjectProperty.UpperBound == 1))
				{
					XElement xp = new XElement(p.SysName);
					xe.Add(xp);
					PropertyInfo pi = GetPropertyInfo(o, pname);
					if (pi != null)
					{
						var val = pi.GetValue(o, null) as IEnumerable;
						if (val != null)
						{
							var casted = val.Cast<IModelObject>();
							foreach (var obj in casted)
							{
								if (obj == null)
									continue;
								var itemname = p.RefObjectType.PrimaryKey.Single().ColumnName;
								if (p.RefObjectType.PrimaryKey.Single().TypeCode == ObjectPropertyType.Guid)
									xp.Add(new XElement(itemname, obj.ObjectGUID));
								else
									xp.Add(new XElement(itemname, obj.ObjectID));
							}
						}
					}
				}
            }

            return xe;
        }

		/// <summary>
		/// Remove illegal XML characters from a string.
		/// </summary>
		public string SanitizeXmlString(string xml)
		{
			if (xml == null)
				return null;

			System.Text.StringBuilder buffer = new System.Text.StringBuilder(xml.Length);

			foreach (char c in xml)
				if (IsLegalXmlChar(c))
					buffer.Append(c);

			return buffer.ToString();
		}

		/// <summary>
		/// Whether a given character is allowed by XML 1.0.
		/// </summary>
		public bool IsLegalXmlChar(int character)
		{
			return character == 0x9 ||/* == '\t' == 9   */
				 character == 0xA ||/* == '\n' == 10  */
				 character == 0xD ||/* == '\r' == 13  */
				(character >= 0x20 && character <= 0xD7FF) ||
				(character >= 0xE000 && character <= 0xFFFD) ||
				(character >= 0x10000 && character <= 0x10FFFF);
		}

		public XElement ExportClassVersion(string objectClassVersion, object id)
		{
			IClassVersion o = GetClassVersion(objectClassVersion, id);
			if (o == null)
				return null;
			// Преобразовать объект в XElement
			XElement xe = new XElement(objectClassVersion);
			foreach (var pi in typeof(IClassVersion).GetProperties())
			{
				object val = pi.GetValue(o, null);
				xe.Add(new XElement(pi.Name, SanitizeXmlString(TypeDescriptor.GetConverter(pi.PropertyType).ConvertToInvariantString(val))));
			}

			return xe;
		}

		public IClassVersion GetClassVersion(string objectClassVersion, object id)
		{
			Type T = assembly.GetType(ns + "." + objectClassVersion, true, true);
			// Параметр лямбда-выражения типа T
			ParameterExpression pe_c = ParameterExpression.Parameter(typeof(IClassVersion), "c");
			// Преобразование IClassVersion к нужному нам реальному типу объекта
			UnaryExpression ue_c = UnaryExpression.Convert(pe_c, T);
			// Получение у объекта свойства с именем, соответствующим первичному ключу
			MemberExpression me_id = MemberExpression.Property(ue_c, "ClassVersionID");
			// Константа, по которой будем искать объект
			ConstantExpression ce_val = ConstantExpression.Constant(id, id.GetType());
			// Сравнение первичного ключа с заданным идентификатором
			BinaryExpression be_eq = BinaryExpression.Equal(me_id, ce_val);
			// Само лямбда-выражение
			Expression<Func<IClassVersion, bool>> expr2 = Expression.Lambda<Func<IClassVersion, bool>>(be_eq, pe_c);
			// Загрузить нужный объект
			return db.GetTable(T).OfType<IClassVersion>().SingleOrDefault(expr2);
		}

        public XElement ExportObjectVersion(MM_ObjectType objectType, object id)
        {
            // Загрузить нужный объект
            IMMObjectVersion o = GetVersion(objectType, id);
            if (o == null)
                return null;
            // Преобразовать объект в XElement
            XElement xe = new XElement(objectType.SysName);
            foreach (var p in objectType.MM_ObjectProperties.Where(p1 => !p1.IsMultilingual && String.IsNullOrEmpty(p1.Expression)))
            {
                string pname = p.SysName.ToLower();
				if (p.UpperBound == 1 || p.TypeCode != ObjectPropertyType.Object)
				{
					if (p.TypeCode == ObjectPropertyType.Object)
						pname = pname + (p.RefObjectTypeID.HasValue && p.RefObjectType.PrimaryKey.Single().TypeCode == ObjectPropertyType.Guid ? "guid" : "id");
					PropertyInfo pi = GetPropertyInfo(o, pname);
					if (pi != null)
					{
						object val = pi.GetValue(o, null);
						xe.Add(new XElement(p.SysName, SanitizeXmlString(TypeDescriptor.GetConverter(pi.PropertyType).ConvertToInvariantString(val))));
					}
				}
				if (p.UpperBound != 1 && p.TypeCode == ObjectPropertyType.Object && !(p.RefObjectPropertyID.HasValue && p.RefObjectProperty.UpperBound == 1))
				{
					XElement xp = new XElement(p.SysName);
					PropertyInfo pi = GetPropertyInfo(o, pname);
					if (pi != null)
					{
						var val = pi.GetValue(o, null) as IEnumerable;
						if (val != null)
						{
							var casted = val.Cast<IModelObject>();
							foreach (var obj in casted)
							{
								var itemname = p.RefObjectType.PrimaryKey.Single().ColumnName;
								if (p.RefObjectType.PrimaryKey.Single().TypeCode == ObjectPropertyType.Guid)
									xp.Add(new XElement(itemname, obj.ObjectGUID));
								else
									xp.Add(new XElement(itemname, obj.ObjectID));
							}
						}
					}
				}
            }
            xe.Add(new XElement("VersionNumber", o.VersionNumber));
            xe.Add(new XElement("IsCurrentVersion", o.IsCurrentVersion));
			if (objectType.HistoryTypeCode == HistoryType.IdentifiersMiss ||
				objectType.HistoryTypeCode == HistoryType.IdentifiersRetain)
			{
				IMMObjectVersion2 o2 = (IMMObjectVersion2)o;
				xe.Add(new XElement("ClassVersionID", o2.ClassVersionID));
			}
            // Экспорт первичного ключа
			string pkname = objectType.PrimaryKey.Single().ColumnName;
            if (pkname.EndsWith("GUID"))
                pkname = pkname.Substring(0, pkname.Length - 4);
			else if (pkname.EndsWith("ID"))
				pkname = pkname.Substring(0, pkname.Length - 2);
			pkname += "Version" + (objectType.PrimaryKey.Single().TypeCode == ObjectPropertyType.Guid ? "GUID" : "ID");
            PropertyInfo pi1 = GetPropertyInfo(o, pkname);
            xe.Add(new XElement(pkname, pi1.GetValue(o, null)));
			xe.Add(new XElement(objectType.PrimaryKey.Single().ColumnName, o.ObjectID));
            return xe;
        }

        public void ImportObject(MM_ObjectType objectType, XElement obj)
        {
            Type T = assembly.GetType(ns + "." + objectType.SysName, true, true);
			// Определить ид
			XElement xeid = obj.Element(objectType.PrimaryKey.Single().SysName);
			object id = xeid != null ? (xeid.Value.ToInt32(0) > 0 ? (object)xeid.Value.ToInt32(0) : xeid.Value.ToGuid()) : 0;
            IMMObject o = Get(objectType, id);
            if (o == null)
            {
                o = (IMMObject)Activator.CreateInstance(T);
                db.GetTable(T).InsertOnSubmit(o);
            }

            foreach (var p in objectType.MM_ObjectProperties.Where(p1 => !p1.IsMultilingual && String.IsNullOrEmpty(p1.Expression)))
            {
                XElement e = obj.Element(p.SysName);
                if (e == null)
                    continue;
				if (p.UpperBound == 1 || p.TypeCode != ObjectPropertyType.Object)
				{
					string pname = p.TypeCode == ObjectPropertyType.Object ? e.Name.LocalName.ToLower() + (p.RefObjectType != null && p.RefObjectType.PrimaryKey.Single().TypeCode == ObjectPropertyType.Guid ? "guid" : "id") : e.Name.LocalName.ToLower();
					if (p.TypeCode == ObjectPropertyType.FileEx)
					{
						pname = pname + "FileGUID";
						PropertyInfo pi1 = GetPropertyInfo(o, pname);
						if (pi1 != null)
						{
							if (e.Value == "")
							{
								pi1.SetValue(o, null, null);
							}
							else
							{
								pi1.SetValue(o, FileStorageManager.DeserializeFromXml(e), null);
							}
						}
						continue;
					}
					if (p.TypeCode == ObjectPropertyType.File)
					{
						pname = pname + "FileID";
						PropertyInfo pi1 = GetPropertyInfo(o, pname);
						if (pi1 != null)
						{
							if (e.Value == "")
							{
								pi1.SetValue(o, null, null);
							}
							else
							{
								pi1.SetValue(o, TypeDescriptor.GetConverter(typeof(int)).ConvertFromInvariantString(e.Value), null);
							}
						}
						continue;
					}
					if (p.TypeCode == ObjectPropertyType.ZoneDateTime)
					{
						PropertyInfo pi1 = GetPropertyInfo(o, pname);
						if (pi1 != null)
						{
							if (e.Value == "")
							{
								pi1.SetValue(o, null, null);
							}
							else
							{
								pi1.SetValue(o, TypeDescriptor.GetConverter(typeof(DateTime)).ConvertFromInvariantString(e.Value), null);
							}
						}
						e = obj.Element(p.SysName + "TimeZoneID");
						if (e != null)
						{
							pi1 = GetPropertyInfo(o, pname + "TimeZoneID");
							if (pi1 != null)
							{
								if (e.Value == "")
								{
									pi1.SetValue(o, null, null);
								}
								else
								{
									pi1.SetValue(o, TypeDescriptor.GetConverter(typeof(int)).ConvertFromInvariantString(e.Value), null);
								}
							}
						}
						continue;
					}
					PropertyInfo pi = GetPropertyInfo(o, pname);
					if (pi != null)
					{
						if (p.TypeCode == ObjectPropertyType.Data)
						{
							var xe = obj.Element(p.SysName);
							if (xe == null)
							{
								pi.SetValue(o, null, null);
							}
							else
							{
								pi.SetValue(o, new Binary(Convert.FromBase64String(xe.Value)), null);
							}
							continue;
						}
					
						try
						{
							if (p.RefObjectTypeID.HasValue && p.RefObjectType.SysName == "SPM_Subject")
								pi.SetValue(o, 1, null);
							else
								pi.SetValue(o, TypeDescriptor.GetConverter(pi.PropertyType).ConvertFromInvariantString(e.Value), null);
						}
						catch (Exception ex)
						{
							throw new Exception("Ошибка установки свойства " + objectType.SysName + "." + pname + " [" + pi.PropertyType.ToString() + "]: {" + e.Value + "}", ex);
						}
					}
				}
				if (p.UpperBound != 1 && p.TypeCode == ObjectPropertyType.Object)
				{
					PropertyInfo pi = GetPropertyInfo(o, p.SysName);
					if (pi != null)
					{
						object coll = pi.GetValue(o, null);
						if (coll != null && coll.GetType().Name == objectType.SysName + p.SysName + "Collection")
						{
							coll.GetType().InvokeMember("Clear", BindingFlags.InvokeMethod, null, coll, new object[] { DataContext });
							foreach (var xi in e.Elements())
							{
								object objid = p.RefObjectType.PrimaryKey.Single().TypeCode == ObjectPropertyType.Guid ? (object)xi.Value.ToGuid() : (object)xi.Value.ToInt32(0);
								var refobj = Get(p.RefObjectType, objid);
								if (refobj == null)
								{
									SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(ConnectionManager.ConnectionString);
									throw new Exception("В базе данных " + csb.InitialCatalog + " не найден объект " + p.RefObjectType.SysName + " с идентификатором " + objid.ToString() +
									", необходимый для импорта объекта " + o.MetaClass.Name + "," + (o.ObjectID > 0 ? o.ObjectID.ToString() : o.ObjectGUID.ToString()));
								}
								coll.GetType().InvokeMember("Add", BindingFlags.InvokeMethod, null, coll, new object[] { refobj });
							}
						}
					}
				}
            }
        }

		public IMMObject DeserializeObject(MM_ObjectType objectType, XElement obj)
		{
			Type T = assembly.GetType(ns + "." + objectType.SysName, true, true);

			// Определить ид
			XElement xeid = obj.Element(objectType.PrimaryKey.Single().ColumnName);
			int id = xeid != null ? xeid.Value.ToInt32(0) : 0;
			IMMObject o = (IMMObject)Activator.CreateInstance(T);

			foreach (var p in objectType.MM_ObjectProperties.Where(p1 => !p1.IsMultilingual && p1.UpperBound == 1 && String.IsNullOrEmpty(p1.Expression)))
			{
				XElement e = obj.Element(p.SysName);
				if (e == null)
					continue;
				string pname = p.TypeCode == ObjectPropertyType.Object ? e.Name.LocalName.ToLower() + (p.RefObjectType.PrimaryKey.Single().TypeCode == ObjectPropertyType.Guid ? "guid" : "id") : e.Name.LocalName.ToLower();
				if (p.TypeCode == ObjectPropertyType.File)
				{
					pname = pname + "FileID";
					PropertyInfo pi1 = GetPropertyInfo(o, pname);
					if (e.Value == "")
					{
						pi1.SetValue(o, null, null);
					}
					else if (e.Elements().Count() == 0)
					{
						try
						{
							// Guid
							PropertyInfo pi2 = GetPropertyInfo(o, pname.Replace("FileID", ""));
							var file = Activator.CreateInstance(pi2.PropertyType);
							file.SetPropertyValue("Guid", new Guid(e.Value));
							pi2.SetValue(o, file, null);
						}
						catch (Exception x)
						{
							throw new Exception("Ошибка установки свойства " + pname + " для класса " + objectType.SysName + ", значение [" + e.Value + "]", x);
						}
					}
					continue;
				}
				if (p.TypeCode == ObjectPropertyType.FileEx)
				{
					PropertyInfo pi1 = GetPropertyInfo(o, pname);
					if (e.Elements().Count() == 0)
					{
						pi1.SetValue(o, null, null);
					}
					else
					{
						pi1.SetValue(o, FileStorageManager.DeserializeFromXml(e), null);
					}
					continue;
				}
				if (p.TypeCode == ObjectPropertyType.ZoneDateTime)
				{
					PropertyInfo pi1 = GetPropertyInfo(o, pname);
					if (e.Value == "")
					{
						pi1.SetValue(o, null, null);
					}
					else
					{
						pi1.SetValue(o, TypeDescriptor.GetConverter(typeof(DateTime)).ConvertFromInvariantString(e.Value), null);
					}
					e = obj.Element(p.SysName + "TimeZoneID");
					if (e != null)
					{
						pi1 = GetPropertyInfo(o, pname + "TimeZoneID");
						if (e.Value == "")
						{
							pi1.SetValue(o, null, null);
						}
						else
						{
							pi1.SetValue(o, TypeDescriptor.GetConverter(typeof(int)).ConvertFromInvariantString(e.Value), null);
						}
					}
					continue;
				}
				PropertyInfo pi = GetPropertyInfo(o, pname);
				try
				{
					if (pname == "LastModifiedUserID")
						pi.SetValue(o, 1, null);
					else if (e.Elements().Count() == 0)
						pi.SetValue(o, TypeDescriptor.GetConverter(pi.PropertyType).ConvertFromInvariantString(e.Value), null);
				}
				catch (Exception ex)
				{
					throw new Exception("Ошибка установки свойства " + objectType.SysName + "." + pname + " [" + pi.PropertyType.ToString() + "]: {" + e.Value + "}" + Environment.NewLine +
					obj.ToString(), ex);
				}
			}
			return o;
		}

		public IMMObject DeserializeObjectVersion(MM_ObjectType objectType, XElement obj)
		{
			Type T = assembly.GetType(ns + ".HST_" + objectType.SysName, true, true);

			string pkname = objectType.PrimaryKey.Single().ColumnName;
			if (pkname.EndsWith("GUID"))
				pkname = pkname.Substring(0, pkname.Length - 4);
			else if (pkname.EndsWith("ID"))
				pkname = pkname.Substring(0, pkname.Length - 2);
			pkname += "Version" + (objectType.PrimaryKey.Single().TypeCode == ObjectPropertyType.Guid ? "GUID" : "ID");
			
			IMMObject o = (IMMObject)Activator.CreateInstance(T);

			foreach (var p in objectType.MM_ObjectProperties.Where(p1 => !p1.IsMultilingual && p1.UpperBound == 1 && String.IsNullOrEmpty(p1.Expression)))
			{
				XElement e = obj.Element(p.SysName);
				if (e == null)
					continue;
				string pname = p.TypeCode == ObjectPropertyType.Object ? e.Name.LocalName.ToLower() + (p.RefObjectType.PrimaryKey.Single().TypeCode == ObjectPropertyType.Guid ? "guid" : "id") : e.Name.LocalName.ToLower();
				if (p.TypeCode == ObjectPropertyType.File)
				{
					pname = pname + "FileID";
					PropertyInfo pi1 = GetPropertyInfo(o, pname);
					if (e.Value == "")
					{
						pi1.SetValue(o, null, null);
					}
					else if (e.Elements().Count() == 0)
					{
						try
						{
							// Guid
							PropertyInfo pi2 = GetPropertyInfo(o, pname.Replace("FileID", ""));
							var file = Activator.CreateInstance(pi2.PropertyType);
							file.SetPropertyValue("Guid", new Guid(e.Value));
							pi2.SetValue(o, file, null);
						}
						catch (Exception x)
						{
							throw new Exception("Ошибка установки свойства " + pname + " для класса " + objectType.SysName + ", значение [" + e.Value + "]", x);
						}
					}
					continue;
				}
				if (p.TypeCode == ObjectPropertyType.ZoneDateTime)
				{
					PropertyInfo pi1 = GetPropertyInfo(o, pname);
					if (e.Value == "")
					{
						pi1.SetValue(o, null, null);
					}
					else
					{
						pi1.SetValue(o, TypeDescriptor.GetConverter(typeof(DateTime)).ConvertFromInvariantString(e.Value), null);
					}
					e = obj.Element(p.SysName + "TimeZoneID");
					if (e != null)
					{
						pi1 = GetPropertyInfo(o, pname + "TimeZoneID");
						if (e.Value == "")
						{
							pi1.SetValue(o, null, null);
						}
						else
						{
							pi1.SetValue(o, TypeDescriptor.GetConverter(typeof(int)).ConvertFromInvariantString(e.Value), null);
						}
					}
					continue;
				}
				PropertyInfo pi = GetPropertyInfo(o, pname);
				try
				{
					if (pname == "LastModifiedUserID")
						pi.SetValue(o, 1, null);
					else if (e.Elements().Count() == 0)
						pi.SetValue(o, TypeDescriptor.GetConverter(pi.PropertyType).ConvertFromInvariantString(e.Value), null);
				}
				catch (Exception ex)
				{
					throw new Exception("Ошибка установки свойства " + objectType.SysName + "." + pname + " [" + pi.PropertyType.ToString() + "]: {" + e.Value + "}", ex);
				}
			}

			// Определить ид
			XElement xeid = obj.Element(pkname);
			PropertyInfo pi3 = GetPropertyInfo(o, pkname);
			if (objectType.PrimaryKey.Single().TypeCode == ObjectPropertyType.Guid)
				pi3.SetValue(o, xeid.Value.ToGuid(), null);
			else
				pi3.SetValue(o, xeid.Value.ToInt32(0), null);

			pi3 = GetPropertyInfo(o, objectType.PrimaryKey.Single().ColumnName);
			pi3.SetValue(o, TypeDescriptor.GetConverter(pi3.PropertyType).ConvertFromInvariantString(obj.Element(objectType.PrimaryKey.Single().ColumnName).Value), null);

			pi3 = GetPropertyInfo(o, "VersionNumber");
			pi3.SetValue(o, TypeDescriptor.GetConverter(pi3.PropertyType).ConvertFromInvariantString(obj.Element("VersionNumber").Value), null);

			pi3 = GetPropertyInfo(o, "IsCurrentVersion");
			pi3.SetValue(o, TypeDescriptor.GetConverter(pi3.PropertyType).ConvertFromInvariantString(obj.Element("IsCurrentVersion").Value), null);

			if (objectType.HistoryTypeCode == HistoryType.IdentifiersMiss ||
				objectType.HistoryTypeCode == HistoryType.IdentifiersRetain)
			{
				pi3 = GetPropertyInfo(o, "ClassVersionID");
				pi3.SetValue(o, TypeDescriptor.GetConverter(pi3.PropertyType).ConvertFromInvariantString(obj.Element("ClassVersionID").Value), null);
			}
			return o;
		}

		public void ImportClassVersion(XElement obj)
		{
			Type T = assembly.GetType(ns + "." + obj.Name.LocalName, true, true);

			// Определить ид
			XElement xeid = obj.Element("ClassVersionID");
			int id = xeid != null ? xeid.Value.ToInt32(0) : 0;
			IClassVersion o = GetClassVersion(obj.Name.LocalName, id);
			if (o == null)
			{
				o = (IClassVersion)Activator.CreateInstance(T);
				db.GetTable(T).InsertOnSubmit(o);
			}

			foreach (var pi in typeof(IClassVersion).GetProperties())
			{
				XElement e = obj.Element(pi.Name);
				if (e == null)
					continue;
				pi.SetValue(o, TypeDescriptor.GetConverter(pi.PropertyType).ConvertFromInvariantString(e.Value), null);
			}
		}

        public void ImportObjectVersion(MM_ObjectType objectType, XElement obj)
        {
            Type T = assembly.GetType(ns + ".HST_" + objectType.SysName, true, true);

            // Определить ид
			string pkname = objectType.PrimaryKey.Single().ColumnName;
			if (pkname.EndsWith("GUID"))
				pkname = pkname.Substring(0, pkname.Length - 4);
			else if (pkname.EndsWith("ID"))
				pkname = pkname.Substring(0, pkname.Length - 2);
			pkname += "Version" + (objectType.PrimaryKey.Single().TypeCode == ObjectPropertyType.Guid ? "GUID" : "ID");
			
            XElement xeid = obj.Element(pkname);
			object id = xeid != null ? (xeid.Value.ToInt32(0) > 0 ? (object)xeid.Value.ToInt32(0) : xeid.Value.ToGuid()) : 0;
            IMMObjectVersion o = GetVersion(objectType, id);
            if (o == null)
            {
                o = (IMMObjectVersion)Activator.CreateInstance(T);
                db.GetTable(T).InsertOnSubmit(o);
            }

            foreach (var p in objectType.MM_ObjectProperties.Where(p1 => !p1.IsMultilingual && String.IsNullOrEmpty(p1.Expression)))
            {
                XElement e = obj.Element(p.SysName);
                if (e == null)
                    continue;
				if (p.UpperBound == 1 || p.TypeCode != ObjectPropertyType.Object)
				{
					string pname = p.TypeCode == ObjectPropertyType.Object ? e.Name.LocalName.ToLower() + (p.RefObjectType != null && p.RefObjectType.PrimaryKey.Single().TypeCode == ObjectPropertyType.Guid ? "guid" : "id") : e.Name.LocalName.ToLower();
					PropertyInfo pi = GetPropertyInfo(o, pname);
					if (pi != null)
					{
						if (pi.Name == "LastModifiedUserID")
							pi.SetValue(o, 1, null);
						else
							pi.SetValue(o, TypeDescriptor.GetConverter(pi.PropertyType).ConvertFromInvariantString(e.Value), null);
					}
				}
				if (p.UpperBound != 1 && p.TypeCode == ObjectPropertyType.Object)
				{
					PropertyInfo pi = GetPropertyInfo(o, p.SysName);
					if (pi != null)
					{
						object coll = pi.GetValue(o, null);
						if (coll != null && coll.GetType().Name == objectType.SysName + p.SysName + "Collection")
						{
							coll.GetType().InvokeMember("Clear", BindingFlags.InvokeMethod, null, coll, null);
							foreach (var xi in e.Elements())
							{
								object objid = p.RefObjectType.PrimaryKey.Single().TypeCode == ObjectPropertyType.Guid ? (object)xi.Value.ToGuid() : (object)xi.Value.ToInt32(0);
								var refobj = Get(p.RefObjectType, objid);
								coll.GetType().InvokeMember("Add", BindingFlags.InvokeMethod, null, coll, new object[] { refobj });
							}
						}
					}
				}
            }
			
            PropertyInfo pi1 = GetPropertyInfo(o, pkname);
			if (pi1 != null)
			{
				pi1.SetValue(o, id, null);
				pi1 = GetPropertyInfo(o, objectType.PrimaryKey.Single().ColumnName);
				pi1.SetValue(o, TypeDescriptor.GetConverter(pi1.PropertyType).ConvertFromInvariantString(obj.Element(objectType.PrimaryKey.Single().ColumnName).Value), null);
			}

            pi1 = GetPropertyInfo(o, "VersionNumber");
			if (pi1 != null)
			{
				pi1.SetValue(o, TypeDescriptor.GetConverter(pi1.PropertyType).ConvertFromInvariantString(obj.Element("VersionNumber").Value), null);

				pi1 = GetPropertyInfo(o, "IsCurrentVersion");
				pi1.SetValue(o, TypeDescriptor.GetConverter(pi1.PropertyType).ConvertFromInvariantString(obj.Element("IsCurrentVersion").Value), null);
			}

			if (objectType.HistoryTypeCode == HistoryType.IdentifiersMiss ||
				objectType.HistoryTypeCode == HistoryType.IdentifiersRetain)
			{
				pi1 = GetPropertyInfo(o, "ClassVersionID");
				if (pi1 != null)
				{
					pi1.SetValue(o, TypeDescriptor.GetConverter(pi1.PropertyType).ConvertFromInvariantString(obj.Element("ClassVersionID").Value), null);
				}
			}
        }

        public static PropertyInfo GetPropertyInfo(IMMObject o, string propertyName)
        {
            PropertyInfo pi = o.GetType().GetProperties().SingleOrDefault(p1 => p1.Name.ToLower() == propertyName.ToLower());
            //if (pi == null)
            //    throw new Exception("У объекта " + o.Title + " (" + o.GetType().FullName + ") не найдено свойство " + propertyName);
            return pi;
        }

        public void SaveObject(XElement obj, string languageCode)
        {
            Type T = assembly.GetType(ns + "." + obj.Name.LocalName, true, true);
            var objectType = AppMM.DataContext.MM_ObjectTypes.SingleOrDefault(m => m.SysName == T.Name);
            // Определить ид
			XElement xeid = obj.Element(objectType.PrimaryKey.Single().ColumnName);
            int id = xeid != null ? xeid.Value.ToInt32(0) : 0;
            object o = Get(objectType, id);
            if (o == null)
            {
                o = Activator.CreateInstance(T);
                db.GetTable(T).InsertOnSubmit(o);
            }
            
            foreach (var p in objectType.MM_ObjectProperties.Where(p1=>!p1.IsMultilingual))
            {
                XElement e = obj.Element(p.SysName);
                if (e == null)
                    continue;
                PropertyInfo pi = T.GetProperties().Single(p1 => p1.Name.ToLower() == e.Name.LocalName.ToLower());
                pi.SetValue(o, TypeDescriptor.GetConverter(pi.PropertyType).ConvertFromInvariantString(e.Value), null);
            }

            object odata = null;
            Type Tdata = null;
            foreach (var p in objectType.MM_ObjectProperties.Where(p1 => p1.IsMultilingual))
            {
                XElement e = obj.Element(p.SysName);
                if (e == null)
                    continue;

                if (Tdata == null)
                    Tdata = assembly.GetType(ns + "." + obj.Name.LocalName + "Data", true, true);
                        
                if (odata == null)
                {
                    odata = GetData(objectType, id, languageCode ?? defaultLanguage);
                    if (odata == null)
                    {
                        odata = Activator.CreateInstance(Tdata);
                        Tdata.GetProperty("LanguageCode").SetValue(odata, languageCode ?? defaultLanguage, null);
                        Tdata.GetProperties().Single(p1 => p1.Name.ToLower() == objectType.SysName.ToLower()).SetValue(odata, o, null);
                        db.GetTable(Tdata).InsertOnSubmit(odata);
                    }
                }
                PropertyInfo pi = Tdata.GetProperties().Single(p1 => p1.Name.ToLower() == e.Name.LocalName.ToLower());
                pi.SetValue(odata, TypeDescriptor.GetConverter(pi.PropertyType).ConvertFromInvariantString(e.Value), null);
            }
        }

        public void SubmitChanges()
        {
			db.CommandTimeout = 300;
            db.SubmitChanges();
        }

        public DataContext DataContext
        {
            get { return db; }
        }

        public IQueryable<IMMObject> GetList(MM_ObjectType objectType)
        {
            string sn;
            if (objectType.MM_ObjectProperties.Any(o => o.IsMultilingual))
            {
                sn = ns + ".V_" + objectType.SysName;
                Type T = assembly.GetType(sn);
                return Repository.GetCurrentLang(db.GetTable(T).OfType<IMMObjectMLView>());
            }
            else
            {
                sn = ns + "." + objectType.SysName;
                Type T = assembly.GetType(sn);
                return db.GetTable(T).OfType<IMMObject>();
            }
        }

        public IQueryable<IMMObjectVersion> GetListHst(MM_ObjectType objectType)
        {
            Type T = assembly.GetType(ns + ".HST_" + objectType.SysName);
            return db.GetTable(T).OfType<IMMObjectVersion>();
        }

		public static IQueryable<IMMObject> GetCurrentLang(IQueryable<IMMObjectMLView> data)
		{
			string langCode = AppMM.CurrentLanguage.LanguageCode.ToLower();
			return data.Where(o => o.LanguageCode == langCode).OfType<IMMObject>();
		}

        public void Delete(object obj)
        {
            db.GetTable(obj.GetType()).DeleteOnSubmit(obj);
        }

        public IMMObject Create(MM_ObjectType objectType)
        {
            Type T = assembly.GetType(ns + "." + objectType.SysName);
            object obj = Activator.CreateInstance(T);
            db.GetTable(T).InsertOnSubmit(obj);
            return (IMMObject)obj;
        }

        public IMMObject Empty(MM_ObjectType objectType)
        {
            if (objectType.MM_ObjectProperties.Any(o => o.IsMultilingual))
            {
                Type T = assembly.GetType(ns + ".V_" + objectType.SysName);
                object obj = Activator.CreateInstance(T);
                return (IMMObject)obj;
            }
            else
            {
                Type T = assembly.GetType(ns + "." + objectType.SysName);
                object obj = Activator.CreateInstance(T);
                return (IMMObject)obj;
            }
        }

        public IMMObjectVersion EmptyHst(MM_ObjectType objectType)
        {
            Type T = assembly.GetType(ns + ".HST_" + objectType.SysName);
            object obj = Activator.CreateInstance(T);
            return (IMMObjectVersion)obj;
        }
    }
}
