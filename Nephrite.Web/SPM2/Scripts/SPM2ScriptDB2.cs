using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web.Controls.Scripts
{
    public class SPM2ScriptDB2 : ISPM2Script
    {
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
        public string GetRolesAccessByIdQuery
        {
            get
            {
                return @"select cast(a.ItemGUID as  varchar(36)) || '-1-' || cast(ra.RoleID as  varchar(36))
				from DBO.SPM_Action a, DBO.SPM_RoleAccess ra
				where ra.ActionID = a.ActionID and a.ItemGUID is not null";
            }
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
    }
}