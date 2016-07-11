using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Reflection;
using System.Configuration;
using System.Linq.Expressions;
using System.ComponentModel;
using System.IO;
using System.Collections;
using Nephrite.FileStorage;
using Nephrite.Meta;
using System.Data.SqlClient;
using Nephrite.Web.Versioning;
using Nephrite.Multilanguage;
using Nephrite.Data;

namespace Nephrite.Web
{ 
    public class Repository
    {
        Assembly assembly;
        IDataContext db;
        string ns;
        string defaultLanguage = "ru";

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


            /*if (HttpContext.Current.Items[AppWeb.AppNamespace + "DataContext"] == null)
            {
				db = Activator.CreateInstance(assembly.GetTypes().Single(o => o.Name == "modelDataContext"), ConnectionManager.Connection) as DataContext;
                HttpContext.Current.Items[AppWeb.AppNamespace + "DataContext"] = db;
                db.Log = new StringWriter();
            }
            db = (DataContext)HttpContext.Current.Items[AppWeb.AppNamespace + "DataContext"];
			db.CommandTimeout = 300;*/
			db = A.Model;

			var language = A.RequestServices.GetService<ILanguage>();
			defaultLanguage = language.Default.Code;
        }

        public IModelObject Get(IMetaClass objectType, object id)
        {
			try
			{
				Type T = assembly.GetType(ns + "." + objectType.Name, true, true);
				// Параметр лямбда-выражения типа T
				ParameterExpression pe_c = ParameterExpression.Parameter(typeof(IModelObject), "c");
				// Преобразование IModelObject к нужному нам реальному типу объекта
				UnaryExpression ue_c = UnaryExpression.Convert(pe_c, T);
				// Получение у объекта свойства с именем, соответствующим первичному ключу
				MemberExpression me_id = MemberExpression.Property(ue_c, objectType.Key.ColumnName);
				// Константа, по которой будем искать объект
				ConstantExpression ce_val = ConstantExpression.Constant(id, id.GetType());
				// Сравнение первичного ключа с заданным идентификатором
				BinaryExpression be_eq = BinaryExpression.Equal(me_id, ce_val);
				// Само лямбда-выражение
				Expression<Func<IModelObject, bool>> expr2 = Expression.Lambda<Func<IModelObject, bool>>(be_eq, pe_c);
				// Загрузить нужный объект
				return db.GetTable(T).OfType<IModelObject>().SingleOrDefault(expr2);
			}
			catch (Exception e)
			{
				throw new Exception("Не удалось загрузить объект " + objectType.Name + ", " + id.ToString(), e);
			}
        }

        public IMMObjectVersion GetVersion(IMetaClass objectType, object id)
        {
            Type T = assembly.GetType(ns + ".HST_" + objectType.Name, true, true);
            // Параметр лямбда-выражения типа T
            ParameterExpression pe_c = ParameterExpression.Parameter(typeof(IMMObjectVersion), "c");
            // Преобразование IModelObject к нужному нам реальному типу объекта
            UnaryExpression ue_c = UnaryExpression.Convert(pe_c, T);
            // Получение у объекта свойства с именем, соответствующим первичному ключу
			string pkname = objectType.Key.ColumnName;
			if (pkname.EndsWith("GUID"))
				pkname = pkname.Substring(0, pkname.Length - 4);
            else if (pkname.EndsWith("ID"))
                pkname = pkname.Substring(0, pkname.Length - 2);
			pkname += "Version" + (objectType.Key.Type is MetaGuidType ? "GUID" : "ID");
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

        public IModelObject GetData(IMetaClass objectType, int id, string languageCode)
        {
            Type T = assembly.GetType(ns + "." + objectType.Name + "Data", true, true);
            // Параметр лямбда-выражения типа Object
            ParameterExpression pe_c = ParameterExpression.Parameter(typeof(IModelObject), "c");
            // Преобразование Object к нужному нам реальному типу объекта
            UnaryExpression ue_c = UnaryExpression.Convert(pe_c, T);
            // Получение у объекта свойства с именем, соответствующим первичному ключу
			MemberExpression me_id = MemberExpression.Property(ue_c, objectType.Key.ColumnName);
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
            Expression<Func<IModelObject, bool>> expr2 = Expression.Lambda<Func<IModelObject, bool>>(be_and, pe_c);
            // Загрузить нужный объект
            return db.GetTable(T).OfType<IModelObject>().SingleOrDefault<IModelObject>(expr2);
        }

        public XElement LoadObject(IMetaClass objectType, int id, string languageCode)
        {
            // Загрузить нужный объект
            object o = Get(objectType, id);
            if (o == null)
                return null;
            // Преобразовать объект в XElement
            XElement xe = new XElement(objectType.Name);
            foreach (var p in objectType.Properties)
            {
				if (p is IMetaAttribute && (p as IMetaAttribute).IsMultilingual) continue;
                PropertyInfo pi = o.GetType().GetProperties().Single(p1 => p1.Name.ToLower() == p.Name.ToLower());
                xe.Add(new XElement(p.Name, TypeDescriptor.GetConverter(pi.PropertyType).ConvertToInvariantString(pi.GetValue(o, null))));
            }

            object odata = null;
			foreach (var p in objectType.Properties)
            {
				if (p is IMetaAttribute && (p as IMetaAttribute).IsMultilingual) continue;
                if (odata == null)
                {
                    odata = GetData(objectType, id, languageCode ?? defaultLanguage);
                    if (odata == null)
                        break;
                }
                PropertyInfo pi = odata.GetType().GetProperties().Single(p1 => p1.Name.ToLower() == p.Name.ToLower());
                xe.Add(new XElement(p.Name, TypeDescriptor.GetConverter(pi.PropertyType).ConvertToInvariantString(pi.GetValue(odata, null))));
            }

            return xe;
        }

        public XElement ExportObject(IMetaClass objectType, object id)
        {
			//if (objectType.Name == "DbFile")
			//{
			//	var f = FileStorageManager.GetFile((Guid)id);
			//	return f == null ? null : f.SerializeToXml();
			//}
            // Загрузить нужный объект
            IModelObject o = Get(objectType, id);
            if (o == null)
                return null;
            // Преобразовать объект в XElement
            XElement xe = new XElement(objectType.Name);
			foreach (var p in objectType.Properties)
            {
				IMetaAttribute a = p as IMetaAttribute;
				IMetaReference r = p as IMetaReference;
				if (a != null && a.IsMultilingual) continue;

                string pname = p.Name.ToLower();
				if (p.UpperBound == 1 || r == null)
				{
					if (r != null)
						pname = pname + (r.RefClass != null && r.RefClass.Key.Type is MetaGuidType ? "guid" : "id");
					PropertyInfo pi = GetPropertyInfo(o, pname);
					if (pi != null)
					{
						object val = pi.GetValue(o, null);
						if (p.Type is MetaFileType)
						{
							if (val != null)
							{
								PropertyInfo pi_g = val.GetType().GetProperty("Guid");
								val = pi_g.GetValue(val, null);
								xe.Add(new XElement(p.Name, val.ToString()));
							}
							else
							{
								xe.Add(new XElement(p.Name, ""));
							}
						}
						else if (p.Type is MetaZoneDateTimeType)
						{
							if (val != null)
								xe.Add(new XElement(p.Name, TypeDescriptor.GetConverter(pi.PropertyType).ConvertToInvariantString(val)));
							else
								xe.Add(new XElement(p.Name, ""));

							pi = GetPropertyInfo(o, pname + "TimeZoneID");
							val = pi.GetValue(o, null);
							if (val != null)
								xe.Add(new XElement(p.Name + "TimeZoneID", TypeDescriptor.GetConverter(pi.PropertyType).ConvertToInvariantString(val)));
							else
								xe.Add(new XElement(p.Name + "TimeZoneID", ""));
						}
						else if (p.Type is MetaByteArrayType)
						{
							if (val != null)
							{
								xe.Add(new XElement(p.Name, Convert.ToBase64String(((byte[])val).ToArray())));
							}
						}
						else
							xe.Add(new XElement(p.Name, SanitizeXmlString(TypeDescriptor.GetConverter(pi.PropertyType).ConvertToInvariantString(val))));
					}
				}
				if (p.UpperBound != 1 && r != null && !(r.InverseProperty != null && r.InverseProperty.UpperBound == 1))
				{
					XElement xp = new XElement(p.Name);
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
								var itemname = r.RefClass.Key.ColumnName;
								if (r.RefClass.Key.Type is MetaGuidType)
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

        public XElement ExportObjectVersion(IMetaClass objectType, object id)
        {
            // Загрузить нужный объект
            IMMObjectVersion o = GetVersion(objectType, id);
            if (o == null)
                return null;
            // Преобразовать объект в XElement
            XElement xe = new XElement(objectType.Name);
			foreach (var p in objectType.Properties)
            {
				if (p is IMetaAttribute && (p as IMetaAttribute).IsMultilingual) continue;
				IMetaReference r = p as IMetaReference;

                string pname = p.Name.ToLower();
				if (p.UpperBound == 1 || r == null)
				{
					if (r != null)
						pname = pname + (r.RefClass != null && r.RefClass.Key.Type is MetaGuidType ? "guid" : "id");
					PropertyInfo pi = GetPropertyInfo(o, pname);
					if (pi != null)
					{
						object val = pi.GetValue(o, null);
						xe.Add(new XElement(p.Name, SanitizeXmlString(TypeDescriptor.GetConverter(pi.PropertyType).ConvertToInvariantString(val))));
					}
				}
				if (p.UpperBound != 1 && r != null && !(r.InverseProperty != null && r.InverseProperty.UpperBound == 1))
				{
					XElement xp = new XElement(p.Name);
					PropertyInfo pi = GetPropertyInfo(o, pname);
					if (pi != null)
					{
						var val = pi.GetValue(o, null) as IEnumerable;
						if (val != null)
						{
							var casted = val.Cast<IModelObject>();
							foreach (var obj in casted)
							{
								var itemname = r.RefClass.Key.ColumnName;
								if (r.RefClass.Key.Type is MetaGuidType)
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

			//var v = objectType.S<SVersioning>();
			//if (v.VersioningType == VersioningType.IdentifiersMiss ||
			//	v.VersioningType == VersioningType.IdentifiersRetain)
			//{
			//	IMMObjectVersion2 o2 = (IMMObjectVersion2)o;
			//	xe.Add(new XElement("ClassVersionID", o2.ClassVersionID));
			//}

            // Экспорт первичного ключа
			string pkname = objectType.Key.ColumnName;
            if (pkname.EndsWith("GUID"))
                pkname = pkname.Substring(0, pkname.Length - 4);
			else if (pkname.EndsWith("ID"))
				pkname = pkname.Substring(0, pkname.Length - 2);
			pkname += "Version" + (objectType.Key.Type is MetaGuidType ? "GUID" : "ID");
            PropertyInfo pi1 = GetPropertyInfo(o, pkname);
            xe.Add(new XElement(pkname, pi1.GetValue(o, null)));
			xe.Add(new XElement(objectType.Key.ColumnName, o.ObjectID));
            return xe;
        }

        public void ImportObject(IMetaClass objectType, XElement obj)
        {
            Type T = assembly.GetType(ns + "." + objectType.Name, true, true);
			// Определить ид
			XElement xeid = obj.Element(objectType.Key.Name);
			object id = xeid != null ? (xeid.Value.ToInt32(0) > 0 ? (object)xeid.Value.ToInt32(0) : xeid.Value.ToGuid()) : 0;
            IModelObject o = Get(objectType, id);
            if (o == null)
            {
                o = (IModelObject)Activator.CreateInstance(T);
                db.GetTable(T).InsertOnSubmit(o);
            }

            foreach (var p in objectType.Properties)
            {
				IMetaReference r = p as IMetaReference;
				IMetaAttribute a = p as IMetaAttribute;
				if (a != null && a.IsMultilingual) continue;
				if (a == null && r == null) continue;

                XElement e = obj.Element(p.Name);
                if (e == null)
                    continue;
				if (p.UpperBound == 1 || r == null)
				{
					string pname = r != null ? e.Name.LocalName.ToLower() + (r.RefClass != null && r.RefClass.Key.Type is MetaGuidType ? "guid" : "id") : e.Name.LocalName.ToLower();
					//if (p.Type is MetaFileType && (p.Type as MetaFileType).IdentifierType is MetaGuidType)
					//{
					//	pname = pname + "FileGUID";
					//	PropertyInfo pi1 = GetPropertyInfo(o, pname);
					//	if (pi1 != null)
					//	{
					//		if (e.Value == "")
					//		{
					//			pi1.SetValue(o, null, null);
					//		}
					//		else
					//		{
					//			pi1.SetValue(o, FileStorageManager.DeserializeFromXml(e), null);
					//		}
					//	}
					//	continue;
					//}
					//if (p.Type is MetaFileType && (p.Type as MetaFileType).IdentifierType is MetaIntType)
					//{
					//	pname = pname + "FileID";
					//	PropertyInfo pi1 = GetPropertyInfo(o, pname);
					//	if (pi1 != null)
					//	{
					//		if (e.Value == "")
					//		{
					//			pi1.SetValue(o, null, null);
					//		}
					//		else
					//		{
					//			pi1.SetValue(o, TypeDescriptor.GetConverter(typeof(int)).ConvertFromInvariantString(e.Value), null);
					//		}
					//	}
					//	continue;
					//}
					if (p.Type is MetaZoneDateTimeType)
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
						e = obj.Element(p.Name + "TimeZoneID");
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
						if (p.Type is MetaByteArrayType)
						{
							var xe = obj.Element(p.Name);
							if (xe == null)
							{
								pi.SetValue(o, null, null);
							}
							else
							{
								pi.SetValue(o, Convert.FromBase64String(xe.Value), null);
							}
							continue;
						}
					
						try
						{
							if (r.RefClass != null && r.RefClass.Name == "SPM_Subject") // @Sad
								pi.SetValue(o, 1, null);
							else
								pi.SetValue(o, TypeDescriptor.GetConverter(pi.PropertyType).ConvertFromInvariantString(e.Value), null);
						}
						catch (Exception ex)
						{
							throw new Exception("Ошибка установки свойства " + objectType.Name + "." + pname + " [" + pi.PropertyType.ToString() + "]: {" + e.Value + "}", ex);
						}
					}
				}
				if (p.UpperBound != 1 && r != null)
				{
					PropertyInfo pi = GetPropertyInfo(o, p.Name);
					if (pi != null)
					{
						object coll = pi.GetValue(o, null);
						if (coll != null && coll.GetType().Name == objectType.Name + p.Name + "Collection")
						{
							coll.GetType().InvokeMember("Clear", BindingFlags.InvokeMethod, null, coll, new object[] { DataContext });
							foreach (var xi in e.Elements())
							{
								object objid = r.RefClass.Key.Type is MetaGuidType ? (object)xi.Value.ToGuid() : (object)xi.Value.ToInt32(0);
								var refobj = Get(r.RefClass, objid);
								if (refobj == null)
								{
									SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(ConnectionManager.ConnectionString);
									throw new Exception("В базе данных " + csb.InitialCatalog + " не найден объект " + r.RefClass.Name + " с идентификатором " + objid.ToString() +
									", необходимый для импорта объекта " + o.MetaClass.Name + "," + (o.ObjectID > 0 ? o.ObjectID.ToString() : o.ObjectGUID.ToString()));
								}
								coll.GetType().InvokeMember("Add", BindingFlags.InvokeMethod, null, coll, new object[] { refobj });
							}
						}
					}
				}
            }
        }

		public IModelObject DeserializeObject(IMetaClass objectType, XElement obj)
		{
			Type T = assembly.GetType(ns + "." + objectType.Name, true, true);

			// Определить ид
			XElement xeid = obj.Element(objectType.Key.ColumnName);
			int id = xeid != null ? xeid.Value.ToInt32(0) : 0;
			IModelObject o = (IModelObject)Activator.CreateInstance(T);

			foreach (var p in objectType.Properties.Where(p1 => p1.UpperBound == 1))
			{
				IMetaReference r = p as IMetaReference;
				IMetaAttribute a = p as IMetaAttribute;
				if (a != null && a.IsMultilingual) continue;
				if (a == null && r == null) continue;

				XElement e = obj.Element(p.Name);
				if (e == null)
					continue;
				string pname = r != null ? e.Name.LocalName.ToLower() + (r.RefClass.Key.Type is MetaGuidType ? "guid" : "id") : e.Name.LocalName.ToLower();
				//if (p.Type is MetaFileType && (p.Type as MetaFileType).IdentifierType is MetaIntType)
				//{
				//	pname = pname + "FileID";
				//	PropertyInfo pi1 = GetPropertyInfo(o, pname);
				//	if (e.Value == "")
				//	{
				//		pi1.SetValue(o, null, null);
				//	}
				//	else if (e.Elements().Count() == 0)
				//	{
				//		try
				//		{
				//			// Guid
				//			PropertyInfo pi2 = GetPropertyInfo(o, pname.Replace("FileID", ""));
				//			var file = Activator.CreateInstance(pi2.PropertyType);
				//			file.SetPropertyValue("Guid", new Guid(e.Value));
				//			pi2.SetValue(o, file, null);
				//		}
				//		catch (Exception x)
				//		{
				//			throw new Exception("Ошибка установки свойства " + pname + " для класса " + objectType.Name + ", значение [" + e.Value + "]", x);
				//		}
				//	}
				//	continue;
				//}
				//if (p.Type is MetaFileType && (p.Type as MetaFileType).IdentifierType is MetaIntType)
				//{
				//	PropertyInfo pi1 = GetPropertyInfo(o, pname);
				//	if (e.Elements().Count() == 0)
				//	{
				//		pi1.SetValue(o, null, null);
				//	}
				//	else
				//	{
				//		pi1.SetValue(o, FileStorageManager.DeserializeFromXml(e), null);
				//	}
				//	continue;
				//}
				if (p.Type is MetaZoneDateTimeType)
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
					e = obj.Element(p.Name + "TimeZoneID");
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
					throw new Exception("Ошибка установки свойства " + objectType.Name + "." + pname + " [" + pi.PropertyType.ToString() + "]: {" + e.Value + "}" + Environment.NewLine +
					obj.ToString(), ex);
				}
			}
			return o;
		}

		public IModelObject DeserializeObjectVersion(MetaClass objectType, XElement obj)
		{
			Type T = assembly.GetType(ns + ".HST_" + objectType.Name, true, true);

			string pkname = objectType.Key.ColumnName;
			if (pkname.EndsWith("GUID"))
				pkname = pkname.Substring(0, pkname.Length - 4);
			else if (pkname.EndsWith("ID"))
				pkname = pkname.Substring(0, pkname.Length - 2);
			pkname += "Version" + (objectType.Key.Type is MetaGuidType ? "GUID" : "ID");
			
			IModelObject o = (IModelObject)Activator.CreateInstance(T);

			foreach (var p in objectType.Properties.Where(p1 => p1.UpperBound == 1))
			{
				MetaReference r = p as MetaReference;
				MetaAttribute a = p as MetaAttribute;
				if (a != null && a.IsMultilingual) continue;
				if (a == null && r == null) continue;

				XElement e = obj.Element(p.Name);
				if (e == null)
					continue;
				string pname = r != null ? e.Name.LocalName.ToLower() + (r.RefClass.Key.Type is MetaGuidType ? "guid" : "id") : e.Name.LocalName.ToLower();
				if (p.Type is MetaFileType && (p.Type as MetaFileType).IdentifierType is MetaIntType)
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
							pi2.PropertyType.GetProperty("Guid").SetValue(file, new Guid(e.Value));
							pi2.SetValue(o, file);
						}
						catch (Exception x)
						{
							throw new Exception("Ошибка установки свойства " + pname + " для класса " + objectType.Name + ", значение [" + e.Value + "]", x);
						}
					}
					continue;
				}
				if (p.Type is MetaZoneDateTimeType)
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
					e = obj.Element(p.Name + "TimeZoneID");
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
					if (pname == "LastModifiedUserID") // @Sad
						pi.SetValue(o, 1, null);
					else if (e.Elements().Count() == 0)
						pi.SetValue(o, TypeDescriptor.GetConverter(pi.PropertyType).ConvertFromInvariantString(e.Value), null);
				}
				catch (Exception ex)
				{
					throw new Exception("Ошибка установки свойства " + objectType.Name + "." + pname + " [" + pi.PropertyType.ToString() + "]: {" + e.Value + "}", ex);
				}
			}

			// Определить ид
			XElement xeid = obj.Element(pkname);
			PropertyInfo pi3 = GetPropertyInfo(o, pkname);
			if (objectType.Key.Type is MetaGuidType)
				pi3.SetValue(o, xeid.Value.ToGuid(), null);
			else
				pi3.SetValue(o, xeid.Value.ToInt32(0), null);

			pi3 = GetPropertyInfo(o, objectType.Key.ColumnName);
			pi3.SetValue(o, TypeDescriptor.GetConverter(pi3.PropertyType).ConvertFromInvariantString(obj.Element(objectType.Key.ColumnName).Value), null);

			pi3 = GetPropertyInfo(o, "VersionNumber");
			pi3.SetValue(o, TypeDescriptor.GetConverter(pi3.PropertyType).ConvertFromInvariantString(obj.Element("VersionNumber").Value), null);

			pi3 = GetPropertyInfo(o, "IsCurrentVersion");
			pi3.SetValue(o, TypeDescriptor.GetConverter(pi3.PropertyType).ConvertFromInvariantString(obj.Element("IsCurrentVersion").Value), null);

			var v = objectType.Stereotype<SVersioning>();
			if (v.VersioningType == VersioningType.IdentifiersMiss ||
				v.VersioningType == VersioningType.IdentifiersRetain)
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

        public void ImportObjectVersion(IMetaClass objectType, XElement obj)
        {
            Type T = assembly.GetType(ns + ".HST_" + objectType.Name, true, true);

            // Определить ид
			string pkname = objectType.Key.ColumnName;
			if (pkname.EndsWith("GUID"))
				pkname = pkname.Substring(0, pkname.Length - 4);
			else if (pkname.EndsWith("ID"))
				pkname = pkname.Substring(0, pkname.Length - 2);
			pkname += "Version" + (objectType.Key.Type is MetaGuidType ? "GUID" : "ID");
			
            XElement xeid = obj.Element(pkname);
			object id = xeid != null ? (xeid.Value.ToInt32(0) > 0 ? (object)xeid.Value.ToInt32(0) : xeid.Value.ToGuid()) : 0;
            IMMObjectVersion o = GetVersion(objectType, id);
            if (o == null)
            {
                o = (IMMObjectVersion)Activator.CreateInstance(T);
                db.GetTable(T).InsertOnSubmit(o);
            }

            foreach (var p in objectType.Properties)
            {
				MetaReference r = p as MetaReference;
				MetaAttribute a = p as MetaAttribute;
				if (a != null && a.IsMultilingual) continue;
				if (a == null && r == null) continue;

                XElement e = obj.Element(p.Name);
                if (e == null)
                    continue;
				if (p.UpperBound == 1 || r == null)
				{
					string pname = r != null ? e.Name.LocalName.ToLower() + (r.RefClass != null && r.RefClass.Key.Type is MetaGuidType ? "guid" : "id") : e.Name.LocalName.ToLower();
					PropertyInfo pi = GetPropertyInfo(o, pname);
					if (pi != null)
					{
						if (pi.Name == "LastModifiedUserID") //@Sad
							pi.SetValue(o, 1, null);
						else
							pi.SetValue(o, TypeDescriptor.GetConverter(pi.PropertyType).ConvertFromInvariantString(e.Value), null);
					}
				}
				if (p.UpperBound != 1 && r != null)
				{
					PropertyInfo pi = GetPropertyInfo(o, p.Name);
					if (pi != null)
					{
						object coll = pi.GetValue(o, null);
						if (coll != null && coll.GetType().Name == objectType.Name + p.Name + "Collection")
						{
							coll.GetType().InvokeMember("Clear", BindingFlags.InvokeMethod, null, coll, null);
							foreach (var xi in e.Elements())
							{
								object objid = r.RefClass.Key.Type is MetaGuidType ? (object)xi.Value.ToGuid() : (object)xi.Value.ToInt32(0);
								var refobj = Get(r.RefClass, objid);
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
				pi1 = GetPropertyInfo(o, objectType.Key.ColumnName);
				pi1.SetValue(o, TypeDescriptor.GetConverter(pi1.PropertyType).ConvertFromInvariantString(obj.Element(objectType.Key.ColumnName).Value), null);
			}

            pi1 = GetPropertyInfo(o, "VersionNumber");
			if (pi1 != null)
			{
				pi1.SetValue(o, TypeDescriptor.GetConverter(pi1.PropertyType).ConvertFromInvariantString(obj.Element("VersionNumber").Value), null);

				pi1 = GetPropertyInfo(o, "IsCurrentVersion");
				pi1.SetValue(o, TypeDescriptor.GetConverter(pi1.PropertyType).ConvertFromInvariantString(obj.Element("IsCurrentVersion").Value), null);
			}

			//var v = objectType.S<SVersioning>();
			//if (v.VersioningType == VersioningType.IdentifiersMiss ||
			//	v.VersioningType == VersioningType.IdentifiersRetain)
			//{
			//	pi1 = GetPropertyInfo(o, "ClassVersionID");
			//	if (pi1 != null)
			//	{
			//		pi1.SetValue(o, TypeDescriptor.GetConverter(pi1.PropertyType).ConvertFromInvariantString(obj.Element("ClassVersionID").Value), null);
			//	}
			//}
        }

        public static PropertyInfo GetPropertyInfo(IModelObject o, string propertyName)
        {
            PropertyInfo pi = o.GetType().GetProperties().SingleOrDefault(p1 => p1.Name.ToLower() == propertyName.ToLower());
            //if (pi == null)
            //    throw new Exception("У объекта " + o.Title + " (" + o.GetType().FullName + ") не найдено свойство " + propertyName);
            return pi;
        }

        public void SaveObject(XElement obj, string languageCode)
        {
            Type T = assembly.GetType(ns + "." + obj.Name.LocalName, true, true);
			var objectType = A.Meta.GetClass(T.Name);
            // Определить ид
			XElement xeid = obj.Element(objectType.Key.ColumnName);
            int id = xeid != null ? xeid.Value.ToInt32(0) : 0;
            object o = Get(objectType, id);
            if (o == null)
            {
                o = Activator.CreateInstance(T);
                db.GetTable(T).InsertOnSubmit(o);
            }
            
            foreach (var p in objectType.Properties)
            {
				MetaAttribute a = p as MetaAttribute;
				if (a != null && a.IsMultilingual) continue;

                XElement e = obj.Element(p.Name);
                if (e == null)
                    continue;
                PropertyInfo pi = T.GetProperties().Single(p1 => p1.Name.ToLower() == e.Name.LocalName.ToLower());
                pi.SetValue(o, TypeDescriptor.GetConverter(pi.PropertyType).ConvertFromInvariantString(e.Value), null);
            }

            object odata = null;
            Type Tdata = null;
            foreach (var p in objectType.Properties.Where(p1 => p1 is MetaAttribute && (p1 as MetaAttribute).IsMultilingual))
            {
                XElement e = obj.Element(p.Name);
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
                        Tdata.GetProperties().Single(p1 => p1.Name.ToLower() == objectType.Name.ToLower()).SetValue(odata, o, null);
                        db.GetTable(Tdata).InsertOnSubmit(odata);
                    }
                }
                PropertyInfo pi = Tdata.GetProperties().Single(p1 => p1.Name.ToLower() == e.Name.LocalName.ToLower());
                pi.SetValue(odata, TypeDescriptor.GetConverter(pi.PropertyType).ConvertFromInvariantString(e.Value), null);
            }
        }

        public void SubmitChanges()
        {
			//db.CommandTimeout = 300;
			db = (IDataContext)System.Web.HttpContext.Current.Items["SolutionDataContext"];
            db.SubmitChanges();
        }

        public IDataContext DataContext
        {
            get { return db; }
        }

		public IQueryable<IModelObject> GetList(IMetaClass objectType)
        {
            string sn;
			if (objectType.Properties.Any(o => o is MetaAttribute && (o as MetaAttribute).IsMultilingual))
            {
                sn = ns + ".V_" + objectType.Name;
                Type T = assembly.GetType(sn);
                return Repository.GetCurrentLang(db.GetTable(T).OfType<IMMObjectMLView>());
            }
            else
            {
                sn = ns + "." + objectType.Name;
                Type T = assembly.GetType(sn);
                return db.GetTable(T).OfType<IModelObject>();
            }
        }

		public IQueryable<IMMObjectVersion> GetListHst(IMetaClass objectType)
        {
            Type T = assembly.GetType(ns + ".HST_" + objectType.Name);
            return db.GetTable(T).OfType<IMMObjectVersion>();
        }

		public static IQueryable<IModelObject> GetCurrentLang(IQueryable<IMMObjectMLView> data)
		{
			var language = A.RequestServices.GetService<ILanguage>();
			string langCode = language.Current.Code.ToLower();
			return data.Where(o => o.LanguageCode == langCode).OfType<IModelObject>();
		}

        public void Delete(object obj)
        {
            db.GetTable(obj.GetType()).DeleteOnSubmit(obj);
        }

		public IModelObject Create(IMetaClass objectType)
        {
            Type T = assembly.GetType(ns + "." + objectType.Name);
            object obj = Activator.CreateInstance(T);
            db.GetTable(T).InsertOnSubmit(obj);
            return (IModelObject)obj;
        }

		public IModelObject Empty(IMetaClass objectType)
        {
			if (objectType.Properties.Any(o => o is MetaAttribute && (o as MetaAttribute).IsMultilingual))
            {
                Type T = assembly.GetType(ns + ".V_" + objectType.Name);
                object obj = Activator.CreateInstance(T);
                return (IModelObject)obj;
            }
            else
            {
                Type T = assembly.GetType(ns + "." + objectType.Name);
                object obj = Activator.CreateInstance(T);
                return (IModelObject)obj;
            }
        }

        public IMMObjectVersion EmptyHst(IMetaClass objectType)
        {
            Type T = assembly.GetType(ns + ".HST_" + objectType.Name);
            object obj = Activator.CreateInstance(T);
            return (IMMObjectVersion)obj;
        }
    }
}
