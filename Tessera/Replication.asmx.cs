﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Nephrite.Metamodel;
using Nephrite.Metamodel.Model;
using Nephrite.Web;
using System.Xml.Linq;
using System.Configuration;
using System.EnterpriseServices;
using System.IO;
using System.Web.Services.Protocols;
using System.Linq.Expressions;
using Nephrite.Web.SPM;
using Nephrite.Web.FileStorage;
using System.Data.SqlClient;
using Nephrite.Meta;

namespace Tessera
{
    /// <summary>
    /// Summary description for Replication
    /// </summary>
	[WebService(Namespace = "http://nephritetech.com/tessera/integration/1.0")]
	[WebServiceBinding(Name = "Replication", ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Replication : System.Web.Services.WebService
    {
		public class UserCredentials : System.Web.Services.Protocols.SoapHeader
		{
			public string Login;
			public string Password;
		}

		public UserCredentials user;

        Repository r = new Repository();
        Tessera.ReplicationService.Replication svc;
        
        Tessera.ReplicationService.Replication ReplicationSourceServer
        {
            get
            {
                if (svc == null)
                {
                    svc = new Tessera.ReplicationService.Replication();
                    svc.Url = ConfigurationManager.AppSettings["ReplicationSourceUrl"];
                    svc.AllowAutoRedirect = true;
                    svc.UseDefaultCredentials = true;
                    svc.PreAuthenticate = true;
					svc.UserCredentialsValue = new Tessera.ReplicationService.UserCredentials { Login = user.Login, Password = user.Password };
                }
                return svc;
            }
        }
		
		void CheckCredentials()
		{
			if (user == null)
				throw new Exception("Аутентификация не пройдена");
			if (user.Login != Nephrite.Web.App.AppSettings.Get("ReplicationLogin"))
				throw new Exception("Неправильный логин или пароль");
			if (user.Password != Nephrite.Web.App.AppSettings.Get("ReplicationPassword"))
				throw new Exception("Неправильный логин или пароль");
		}

		[WebMethod]
		[SoapDocumentMethod(Binding = "Replication")]
		[SoapHeader("user")]
		public string RunDynamic(string type, string method, string data)
		{
			CheckCredentials();
			Type t = Type.GetType(type, true, true);
			return (string)t.GetMethod(method).Invoke(null, new object[] { data });
		}

        [WebMethod]
		[SoapDocumentMethod(Binding = "Replication")]
		[SoapHeader("user")]
		public string[] GetVersionRecords(string objectType, string objectID, DateTime startDate)
        {
			CheckCredentials();
            // Получить список версий строк

			MetaClass ot = Base.Meta.GetClass(objectType); //ObjectTypeRepository.Get(objectType);
			if (ot == null)
				throw new Exception("Класс " + objectType + " не найден в метамодели");

			var instance = r.EmptyHst(ot);
			
			object id = ot.Key.Type is MetaGuidType ? (object)objectID.ToGuid() : objectID.ToInt32(0);
            var list = r.GetListHst(ot).Where(instance.FilterByObjectID(id)).Where(o => o.LastModifiedDate > startDate).
                Select(instance.GetIdentifierSelector());
            return list.Select(o => o.ToString()).ToArray();
        }

        [WebMethod]
		[SoapDocumentMethod(Binding = "Replication")]
		[SoapHeader("user")]
		public string GetRecord(string objectType, string id)
        {
			CheckCredentials();
			if (objectType.StartsWith("CHST_"))
			{
				var xe = r.ExportClassVersion(objectType, id);
				if (xe == null)
					return "";
				return xe.ToString(SaveOptions.DisableFormatting);
			}
			else
			{
				var ot = ObjectTypeRepository.Get(objectType);
				var xe = r.ExportObject(ot, id.ToInt32(0) > 0 ? (object)id.ToInt32(0) : id.ToGuid());
				if (xe == null)
					return "";
				return xe.ToString(SaveOptions.DisableFormatting);
			}
        }

        [WebMethod]
		[SoapDocumentMethod(Binding = "Replication")]
		[SoapHeader("user")]
		public string GetVersionRecord(string objectType, string versionid)
        {
			CheckCredentials();
			MetaClass ot = Base.Meta.GetClass(objectType); //ObjectTypeRepository.Get(objectType);
			var obj = r.ExportObjectVersion(ot, versionid.ToInt32(0) > 0 ? (object)versionid.ToInt32(0) : versionid.ToGuid());
			if (obj == null)
				throw new Exception("Версия объекта " + objectType + " " + versionid + " не найдена в БД");
			return obj.ToString(SaveOptions.DisableFormatting);
        }

        [WebMethod]
		[SoapDocumentMethod(Binding = "Replication")]
		[SoapHeader("user")]
		public byte[] GetFileContent(Guid fileGuid)
        {
			CheckCredentials();
			var file = FileStorageManager.GetFile(fileGuid);
            if (file == null)
                return new byte[0];
            return file.GetBytes();
        }

		[WebMethod]
		[SoapDocumentMethod(Binding = "Replication")]
		[SoapHeader("user")]
		public string GetFile(Guid fileGuid)
		{
			CheckCredentials();
			var file = FileStorageManager.GetFile(fileGuid);
			if (file == null)
				return "";
			XElement xe = new XElement("N_File");
			xe.SetElementValue("Name", file.Title);
			xe.SetElementValue("Title", file.Title);
			xe.SetElementValue("Extension", file.Extension);
			xe.SetElementValue("LastModifiedDate", file.LastModifiedDate);
			xe.SetElementValue("Path", file.Path);
			return xe.ToString(SaveOptions.DisableFormatting);
		}

        [WebMethod]
		[SoapDocumentMethod(Binding = "Replication")]
		[SoapHeader("user")]
		public ReplicationObject[] GetReplicationObjects(int maxCount)
        {
			CheckCredentials();
			var nr = AppMM.DataContext.N_ReplicationObjects.OrderBy(o => o.ChangeDate).Take(maxCount);
            return nr.Select(o => new ReplicationObject
            {
                ObjectID = o.ObjectID,
                ObjectType = o.ObjectTypeSysName,
                Ticks = o.ChangeDate.Ticks
            }).ToArray();
        }

        [WebMethod]
		[SoapDocumentMethod(Binding = "Replication")]
		[SoapHeader("user")]
		public void ConfirmReplication(string objectType, string id, long ticks)
        {
			CheckCredentials();
			DateTime dt = new DateTime(ticks);
            var nr = AppMM.DataContext.N_ReplicationObjects.SingleOrDefault(o => o.ObjectTypeSysName == objectType &&
                o.ObjectID == id && o.ChangeDate == dt);
            if (nr != null)
            {
                AppMM.DataContext.N_ReplicationObjects.DeleteOnSubmit(nr);
                AppMM.DataContext.SubmitChanges();
            }
        }

        [WebMethod]
		[SoapDocumentMethod(Binding = "Replication")]
		[SoapHeader("user")]
		public bool ImportObject(string xmlData)
        {
			try
			{
				CheckCredentials();
				if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["LogImportObjects"]))
					File.AppendAllText(ConfigurationManager.AppSettings["LogImportObjects"], DateTime.Now.ToString() + "\tImportObject" + Environment.NewLine + XElement.Parse(xmlData).ToString(SaveOptions.None) + Environment.NewLine + Environment.NewLine);
				XElement xe = XElement.Parse(xmlData);

				var ot = ObjectTypeRepository.Get(xe.Name.LocalName);

				if (ot == null)
					throw new Exception("Класс " + xe.Name.LocalName + " не найден в метамодели");

				if (!ImportObject(ot, xmlData))
					return false;

				return true;
			}
			catch (Exception e)
			{
				ErrorLogger.Log(e);
				throw e;
			}
        }

        [WebMethod]
		[SoapDocumentMethod(Binding = "Replication")]
		[SoapHeader("user")]
		public string[] GetObjectsByPropertyValue(string objectType, string propertyName, string id)
        {
			CheckCredentials();
			MM_ObjectType ot = ObjectTypeRepository.Get(objectType);
            var instance = r.Empty(ot);
            return r.GetList(ot).Where(instance.FilterByProperty(propertyName, id)).
                Select(instance.GetIdentifierSelector()).Select(o => o.ToString()).ToArray();
        }

		[WebMethod]
		[SoapDocumentMethod(Binding = "Replication")]
		[SoapHeader("user")]
		public string[] GetObjectsList(string objectType, string controllerMethod, string[] parameters)
		{
			CheckCredentials();
			MM_ObjectType ot = ObjectTypeRepository.Get(objectType);
            var instance = r.Empty(ot);
			Type c = ControllerFactory.GetControllerType(objectType);
			var m = c.GetMethod(controllerMethod);
			if (m == null)
				throw new Exception("Класс " + c.FullName + " не содержит метода " + controllerMethod);
			var controller = Activator.CreateInstance(c);
			var mp = m.GetParameters();
			var plist = new object[mp.Length];
			for (int i = 0; i < plist.Length; i++)
			{
				try
				{
					if (mp[i].ParameterType == typeof(Guid))
						plist[i] = parameters[i].ToGuid();
					else
						plist[i] = Convert.ChangeType(parameters[i], mp[i].ParameterType);
				}
				catch
				{
					switch (mp[i].ParameterType.Name)
					{
						case "DateTime":
							plist[i] = DateTime.Today;
							break;
						case "Int32":
							plist[i] = 0;
							break;
						case "Boolean":
							plist[i] = false;
							break;
						default:
							throw;
					}
				}
			}
			var exp = m.Invoke(controller, plist) as Expression<Func<IMMObject, bool>>;
			return r.GetList(ot).Where(exp).Select(instance.GetIdentifierSelector()).Select(o => o.ToString()).ToArray();
		}

		[WebMethod]
		[SoapDocumentMethod(Binding = "Replication")]
		[SoapHeader("user")]
		public ObjectTransition[] GetObjectTransitions(string objectType, string id)
		{
			CheckCredentials();
			MM_ObjectType ot = ObjectTypeRepository.Get(objectType);
			var instance = r.Empty(ot);
			Type c = ControllerFactory.GetControllerType(objectType);
			var m = c.GetMethod("GetTransitions");
			var tlist = (m.Invoke(null, new object[] {ot.PrimaryKey[0].TypeCode == ObjectPropertyType.Guid ? (object)id.ToGuid() : id.ToInt32(0) }) as IQueryable<IMMObjectTransition>).ToList();
			List<ObjectTransition> otlist = new List<ObjectTransition>();
			foreach (var t in tlist)
			{
				var otr = new ObjectTransition();
				otr.CreateDate = t.CreateDate;
				otr.Comment = t.Comment;
				otr.IsCurrent = t.IsCurrent;
				otr.IsLast = t.IsLast;
				otr.SeqNo = t.SeqNo;

				otr.ActivitySysName = AppMM.DataContext.WF_Activities.Single(o => o.ActivityID == t.ActivityID).SysName;
				otr.WorkflowSysName = AppMM.DataContext.WF_Workflows.Single(o => o.WorkflowID == t.WorkflowID).SysName;
				otr.PrevActivitySysName = AppMM.DataContext.WF_Transitions.Single(o => o.TransitionID == t.TransitionID).ParentActivity.SysName;
				otr.ObjectTransitionID = (int)t.GetType().GetProperty(objectType + "TransitionID").GetValue(t, null);
				otlist.Add(otr);
			}
			
			return otlist.ToArray();
		}

        void ImportObjectVersion(MM_ObjectType objectType, string verid)
        {
			string versionData = ReplicationSourceServer.GetVersionRecord(objectType.SysName, verid);
            XElement xeVer = XElement.Parse(versionData);
            // Получить сам объект
            string id = (string)xeVer.Element(objectType.PrimaryKey[0].SysName);
			string data = ReplicationSourceServer.GetRecord(objectType.SysName, id);
            r.ImportObject(objectType, XElement.Parse(data));
            r.SubmitChanges();
            r.ImportObjectVersion(objectType, xeVer);
			r.SubmitChanges();
        }

		[WebMethod]
		[SoapDocumentMethod(Binding = "Replication")]
		[SoapHeader("user")]
		public void RunTasks()
		{
			AppSPM.RunWithElevatedPrivileges(() => Nephrite.Web.TaskManager.TaskManager.Run());
		}

		public bool ImportObject(MM_ObjectType objectType, string data)
        {
			XElement xe = XElement.Parse(data);
			
			if (objectType.SysName == "N_Folder")
			{
				var folder = FileStorageManager.GetFolder(xe.Element("Guid").Value.ToGuid());
				if (folder == null)
				{
					folder = FileStorageManager.CreateFolder(xe.Element("Title").Value, "");
					folder.SetPropertyValue("ID", xe.Element("Guid").Value.ToGuid());
				}
				folder.Title = xe.Element("Title").Value;
				folder.CheckValid();
				Base.Model.SubmitChanges();
				return true;
			}

			if (objectType.SysName == "N_File")
			{
				IDbFolder folder = null;
				string path = "";
				if (xe.Element("FolderGUID") != null)
				{
					folder = FileStorageManager.GetFolder(xe.Element("FolderGUID").Value.ToGuid());
					if (folder == null)
						return false;
					else
						path = folder.FullPath;
				}
				if (xe.Element("Path") != null)
					path = xe.Element("Path").Value;

				var file = FileStorageManager.GetFile(xe.Element("Guid").Value.ToGuid());
				if (file == null)
				{
					file = FileStorageManager.CreateFile(xe.Element("Title").Value, path);
					file.SetPropertyValue("ID", xe.Element("Guid").Value.ToGuid());
				}
				else
				{
					file.Title = xe.Element("Title").Value;
				}
				file.Write(Convert.FromBase64String(xe.Element("Data").Value));
				file.CheckValid();
				Base.Model.SubmitChanges();
				return true;
			}

            // Проверка наличия объектов, на который ссылается данный объект
            foreach (var prop in objectType.MM_ObjectProperties.Where(o => o.RefObjectTypeID.HasValue && !o.IsAggregate && o.UpperBound == 1))
            {
                if (prop.RefObjectType.SysName == "SPM_Subject")
                    continue;

                XElement xep = xe.Element(prop.SysName);
                if (xep != null)
                {
                    string propID = xep.Value;
                    if (propID.ToInt32(0) > 0 || propID.ToGuid() != Guid.Empty)
                    {
                        var ref_ot = ObjectTypeRepository.Get(prop.RefObjectType.SysName);
						object propIDobj = propID.ToInt32(0) > 0 ? (object)propID.ToInt32(0) : (object)propID.ToGuid();
						if ((prop.IsReferenceToVersion ? r.GetVersion(ref_ot, propIDobj) : r.Get(ref_ot, propIDobj)) == null)
                        {
                            // Импорт версии или объекта
                            if (prop.IsReferenceToVersion)
                            {
								ImportObjectVersion(ref_ot, propID);
                            }
                            else
                            {
								string refdata = ReplicationSourceServer.GetRecord(ref_ot.SysName, propID);
                                if (refdata == "")
                                {
                                    ErrorLogger.Log(new Exception("Передача не удалась, т.к. сервер не вернул объект " + ref_ot.SysName + ", ID=" +
                                        propID.ToString() + ", необходимый для импорта свойства " + prop.SysName + " объекта " + data));
                                    return false;
                                }
								ImportObject(ref_ot, refdata);
                            }
                        }
                    }
                }
            }
            
            // Если имеются ссылки на файлы, то сначала загрузить файлы
            foreach (var fileProp in objectType.MM_ObjectProperties.Where(o => o.TypeCode == ObjectPropertyType.File &&
                o.UpperBound == 1))
            {
                var guid = xe.Element(fileProp.SysName).Value;
                if (guid != "")
                {
                    var g = new Guid(guid);
					string fileinfo = ReplicationSourceServer.GetFile(g);
					if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["LogImportObjects"]))
						File.AppendAllText(ConfigurationManager.AppSettings["LogImportObjects"], DateTime.Now.ToString() + "\tGetFile" + Environment.NewLine + XElement.Parse(fileinfo).ToString(SaveOptions.None) + Environment.NewLine + Environment.NewLine);
            
                    if (fileinfo != String.Empty)
                    {
                        var fx = XElement.Parse(fileinfo);
						byte[] filedata = ReplicationSourceServer.GetFileContent(g);
                        var file = FileStorageManager.GetFile(g);
                        if (file == null)
                        {
							file = FileStorageManager.CreateFile(fx.Element("Name").Value, fx.Element("Path").Value);
							file.Write(filedata);
							file.CheckValid();
							Base.Model.SubmitChanges();
                        }
                        else
                        {
							file.Write(filedata);
							file.CheckValid();
							Base.Model.SubmitChanges();
                        }
						var cmd = ConnectionManager.Connection.CreateCommand();
						cmd.CommandText = "select FileID from N_File where Guid = @Guid";
						cmd.CommandType = System.Data.CommandType.Text;
						cmd.Parameters.Add(new SqlParameter("@Guid", file.ID));
						if (cmd.Connection.State != System.Data.ConnectionState.Open)
							cmd.Connection.Open();
						try
						{
							xe.SetElementValue(fileProp.SysName, cmd.ExecuteScalar());
						}
						finally
						{
							cmd.Connection.Close();
						}
                    }
                }
            }

            r.ImportObject(objectType, xe);
            r.SubmitChanges();

            XElement xepk = xe.Element(objectType.PrimaryKey[0].SysName);
            // Импорт агрегируемых объектов
            foreach (var prop in objectType.MM_ObjectProperties.Where(o => o.RefObjectPropertyID.HasValue && o.IsAggregate && o.UpperBound == -1))
            {
                var ref_ot = ObjectTypeRepository.Get(prop.RefObjectType.SysName);

				string[] ids = ReplicationSourceServer.GetObjectsByPropertyValue(ref_ot.SysName, prop.RefObjectProperty.ColumnName, xepk.Value);
                foreach (var objid in ids)
                {
					string refdata = ReplicationSourceServer.GetRecord(ref_ot.SysName, objid);
					if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["LogImportObjects"]))
						File.AppendAllText(ConfigurationManager.AppSettings["LogImportObjects"], DateTime.Now.ToString() + "\tGetRecord" + Environment.NewLine + XElement.Parse(refdata).ToString(SaveOptions.None) + Environment.NewLine + Environment.NewLine);
            
                    if (refdata == "")
                    {
                        ErrorLogger.Log(new Exception("Передача не удалась, т.к. сервер не вернул агрегируемый объект " + ref_ot.SysName + ", ID=" +
                            objid.ToString() + ", необходимый для импорта свойства " + prop.SysName + " объекта " + data));

                        return false;
                    }
					ImportObject(ref_ot, refdata);
                }
            }
			
            return true;
        }
    }

    [Serializable]
    public class ReplicationObject
    {
        public string ObjectType { get; set; }
        public string ObjectID { get; set; }
        public long Ticks { get; set; }
    }

	[Serializable]
	public class ObjectTransition
	{
		public DateTime CreateDate { get; set; }
		public string Comment { get; set; }
		public bool IsCurrent { get; set; }
		public bool IsLast { get; set; }
		public int SeqNo { get; set; }
		public string WorkflowSysName { get; set; }
		public string ActivitySysName { get; set; }
		public string PrevActivitySysName { get; set; }
		public int ObjectTransitionID { get; set; }
	}
}