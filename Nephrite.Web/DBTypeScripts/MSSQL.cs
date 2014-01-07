using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.DBTypeScripts;

namespace Nephrite.Web.DBTypeScripts
{
	public class MSSQL : IDBTypeScripts
	{
		#region IDBTypeScripts Members

		public string FromLogin
		{
			get { return "select SubjectID as ID, SystemName as \"Login\", Title as \"Title\", PasswordHash as \"PasswordHash\",  CASE WHEN  IsActive = '1' THEN 1 ELSE 0 END  as \"_IsActive\", CASE WHEN  IsDeleted = '1' THEN 1 ELSE 0 END  as \"_IsDeleted\", SID , MustChangePassword  as \"_MustChangePassword\", Email as  \"Email\" from SPM_Subject where lower(SystemName) = ?"; }
		}
		public string FromSID
		{
			get { return "select SubjectID  as ID, SystemName as \"Login\", Title as \"Title\", PasswordHash as \"PasswordHash\",  CASE WHEN  IsActive = '1' THEN 1 ELSE 0 END as \"_IsActive\",  CASE WHEN  IsDeleted = '1' THEN 1 ELSE 0 END  as \"_IsDeleted\", SID, MustChangePassword  as \"_MustChangePassword\", Email as \"Email\" from SPM_Subject where SID = ? or lower(SystemName) = ?"; }
		}
		public string FromID
		{
			get { return "select SubjectID as ID, SystemName as \"Login\", Title as \"Title\", PasswordHash as \"PasswordHash\", CASE WHEN  IsActive = '1' THEN 1 ELSE 0 END  as \"_IsActive\", CASE WHEN  IsDeleted = '1' THEN 1 ELSE 0 END  as \"_IsDeleted\", SID, MustChangePassword as \"_MustChangePassword\", Email as  \"Email\" from SPM_Subject where SubjectID = ?"; }
		}

		public string FromEmail
		{
			get { return "select SubjectID as ID, SystemName as \"Login\", Title as \"Title\", PasswordHash as \"PasswordHash\",  CASE WHEN  IsActive = '1' THEN 1 ELSE 0 END  as \"_IsActive\",  CASE WHEN  IsDeleted = '1' THEN 1 ELSE 0 END  as \"IsDeleted\", SID, MustChangePassword as \"MustChangePassword\", Email as \"Email\" from SPM_Subject where lower(Email) = ?"; }
		}
		#endregion
	}
}