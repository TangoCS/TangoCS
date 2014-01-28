using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Nephrite.Web.DBTypeScripts
{
	public class DB2 : IDBTypeScripts
	{
		#region IDBTypeScripts Members

		public string FromLogin
		{
			get { return "select SubjectID as ID, SystemName as \"Login\", Title as \"Title\", PasswordHash as \"PasswordHash\",  IsActive as \"_IsActive\", IsDeleted as \"_IsDeleted\", SID , MustChangePassword  as \"_MustChangePassword\", Email as  \"Email\" from SPM_Subject where lower(SystemName) = ?"; }
		}
		public string FromSID
		{
			get { return "select SubjectID  as ID, SystemName as \"Login\", Title as \"Title\", PasswordHash as \"PasswordHash\", IsActive as \"_IsActive\", IsDeleted  as \"_IsDeleted\", SID, MustChangePassword  as \"_MustChangePassword\", Email as \"Email\" from SPM_Subject where SID = ? or lower(SystemName) = ?"; }
		}
		public string FromID
		{
			get { return "select SubjectID as ID, SystemName as \"Login\", Title as \"Title\", PasswordHash as \"PasswordHash\", IsActive as \"_IsActive\", IsDeleted as \"_IsDeleted\", SID, MustChangePassword as \"_MustChangePassword\", Email as  \"Email\" from SPM_Subject where SubjectID = ?"; }
		}
		public string FromEmail
		{
			get { return "select SubjectID as ID, SystemName as \"Login\", Title as \"Title\", PasswordHash as \"PasswordHash\", IsActive as \"_IsActive\", IsDeleted as \"IsDeleted\", SID, MustChangePassword as \"MustChangePassword\", Email as \"Email\" from SPM_Subject where lower(Email) = ?"; }
		}
		public string Menu
		{
			get
			{
				return "SELECT t0.Title as \"Title\", t0.URL AS \"Url\", t0.Type AS \"MenuItemType\", t0.ImageURL AS \"ImageUrl\", " +
							"	(CASE"+
							"		WHEN t2.test IS NOT NULL THEN cast(t6.SysName as varchar(32000))" +
							"		WHEN t4.test IS NOT NULL THEN t7.SysName || 'Pck'"+
							"		ELSE cast('' as varchar(32000))"+
							"	 END) AS \"ClassName\","+ 
							"	(CASE "+
							"		WHEN (t2.test IS NOT NULL) AND (t6.IsEnableSPM = 1) THEN 1"+
							"		WHEN NOT ((t2.test IS NOT NULL) AND (t6.IsEnableSPM = 1)) THEN 0"+
							"		ELSE NULL"+
							"	 END) AS \"EnableSPM\", " +
							"	(CASE "+
							"		WHEN t0.Type = 'V' THEN t4.SysName "+
							"		ELSE t2.SysName "+
							"	 END) AS \"MethodSysName\", t0.SeqNo as \"SeqNo\", t0.ParentGUID AS \"_ParentMenuItemGUID\", t0.NavigItemGUID AS \"_MenuItemGUID\",  " +
							"	(CASE  "+
							"		WHEN t0.Type = 'V' THEN t0.SPMActionGUID "+
							"		ELSE t2.Guid "+
							"	 END) AS \"_SPMActionGUID\"" +
							"FROM dbo.V_N_NavigItem AS t0 "+
							"LEFT OUTER JOIN ( "+
							"	SELECT 1 AS test, t1.MethodID, t1.SysName, t1.ObjectTypeID, t1.Guid "+
							"	FROM dbo.MM_Method AS t1  "+
							"	) AS t2 ON t0.MethodID = (t2.MethodID) "+
							"LEFT OUTER JOIN ( "+
							"	SELECT 1 AS test, t3.FormViewID, t3.SysName, t3.PackageID "+
							"	FROM dbo.MM_FormView AS t3 "+
							"	) AS t4 ON t0.FormViewID = (t4.FormViewID) "+
							"INNER JOIN dbo.N_Navig AS t5 ON t5.NavigGUID = t0.NavigGUID "+
							"LEFT OUTER JOIN dbo.MM_ObjectType AS t6 ON t6.ObjectTypeID = t2.ObjectTypeID "+
							"LEFT OUTER JOIN dbo.MM_Package AS t7 ON t7.PackageID = t4.PackageID "+
							"WHERE (NOT (t0.IsDeleted = 1)) AND (t5.SysName = 'MainMenu') AND (t0.LanguageCode = 'ru')"; }
		}
		public string GetRolesAccessByNameQuery
		{
			get
			{
				return @"select substr(pa.SystemName || '.' || a.SystemName,1,255) || '-1'
				from DBO.SPM_Action a, 
				DBO.SPM_ActionAsso asso,
				DBO.SPM_Action pa,
				DBO.SPM_ActionAsso asso2,
				DBO.SPM_Action roota
				where a.ActionID = asso.ActionID and pa.ActionID = asso.ParentActionID and
				pa.ActionID = asso2.ActionID and roota.ActionID = asso2.ParentActionID 
				order by substr(pa.SystemName || '.' || a.SystemName,1,255) || '-1'";// order by pa.SystemName + '.' + a.SystemName
			}
		}

		public string GetRolesAccessByIdQuery
		{
			get
			{
				return @"select cast(a.ItemGUID as  varchar(36)) || '-1-' || cast(ra.RoleID as  varchar(36))
				from DBO.SPM_Action a, DBO.SPM_RoleAccess ra
				where ra.ActionID = a.ActionID and a.ItemGUID is not null"; }
		}
		public string GetItemsIdsQuery
		{
			get
			{
				return @"select CAST(a.ItemGUID as varchar(36) ) || '-1'
				from DBO.SPM_Action a
				where a.ItemGUID is not null";
			}
		}

		public string GetItemsNamesQuery
		{
			get
			{
				return @"select pa.SystemName || '.' || a.SystemName || '-1'
				from DBO.SPM_Action a, 
				DBO.SPM_ActionAsso asso,
				DBO.SPM_Action pa,
				DBO.SPM_ActionAsso asso2,
				DBO.SPM_Action roota
				where a.ActionID = asso.ActionID and pa.ActionID = asso.ParentActionID and
				pa.ActionID = asso2.ActionID and roota.ActionID = asso2.ParentActionID "; // order by pa.SystemName + '.' + a.SystemName
			}
		}
		
		#endregion
	}
}