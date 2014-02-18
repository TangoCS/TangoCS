using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web.Controls.Scripts
{
    public class SPM2ScriptMSSQL : ISPM2Script
    {
        public string FromLogin
        {
            get
            {
                return "select SubjectID as ID, SystemName as \"Login\", Title as \"Title\", PasswordHash as \"PasswordHash\",  CASE WHEN  IsActive = '1' THEN 1 ELSE 0 END  as \"_IsActive\", CASE WHEN  IsDeleted = '1' THEN 1 ELSE 0 END  as \"_IsDeleted\", SID ,  CASE WHEN  MustChangePassword = '1' THEN 1 ELSE 0 END  as \"_MustChangePassword\", Email as  \"Email\" from SPM_Subject where lower(SystemName) = ?";
            }
        }
        public string FromSID
        {
            get
            {
                return "select SubjectID  as ID, SystemName as \"Login\", Title as \"Title\", PasswordHash as \"PasswordHash\",  CASE WHEN  IsActive = '1' THEN 1 ELSE 0 END as \"_IsActive\",  CASE WHEN  IsDeleted = '1' THEN 1 ELSE 0 END  as \"_IsDeleted\", SID,  CASE WHEN  MustChangePassword = '1' THEN 1 ELSE 0 END  as \"_MustChangePassword\", Email as \"Email\" from SPM_Subject where SID = ? or lower(SystemName) = ?";
            }
        }
        public string FromID
        {
            get
            {
                return "select SubjectID as ID, SystemName as \"Login\", Title as \"Title\", PasswordHash as \"PasswordHash\", CASE WHEN  IsActive = '1' THEN 1 ELSE 0 END  as \"_IsActive\", CASE WHEN  IsDeleted = '1' THEN 1 ELSE 0 END  as \"_IsDeleted\", SID,  CASE WHEN  MustChangePassword = '1' THEN 1 ELSE 0 END as \"_MustChangePassword\", Email as  \"Email\" from SPM_Subject where SubjectID = ?";
            }
        }
        public string FromEmail
        {
            get { return "select SubjectID as ID, SystemName as \"Login\", Title as \"Title\", PasswordHash as \"PasswordHash\",  CASE WHEN  IsActive = '1' THEN 1 ELSE 0 END  as \"_IsActive\",  CASE WHEN  IsDeleted = '1' THEN 1 ELSE 0 END  as \"IsDeleted\", SID,  CASE WHEN  MustChangePassword = '1' THEN 1 ELSE 0 END as \"MustChangePassword\", Email as \"Email\" from SPM_Subject where lower(Email) = ?"; }
        }
        public string GetRolesAccessByIdQuery
        {
            get
            {
                return @"select convert(varchar(36), a.ItemGUID) + '-1-' + convert(varchar, ra.RoleID)
				from SPM_Action a, SPM_RoleAccess ra
				where ra.ActionID = a.ActionID and a.ItemGUID is not null";
            }
        }
        public string GetRolesAccessByNameQuery
        {
            get
            {
                return @"select upper(pa.SystemName + '.' + a.SystemName) + '-1-' + convert(varchar, ra.RoleID)
				from SPM_Action a, 
				SPM_ActionAsso asso,
				SPM_Action pa,
				SPM_ActionAsso asso2,
				SPM_Action roota,
				SPM_RoleAccess ra
				where a.ActionID = asso.ActionID and pa.ActionID = asso.ParentActionID and
				pa.ActionID = asso2.ActionID and roota.ActionID = asso2.ParentActionID and
				ra.ActionID = a.ActionID
				order by pa.SystemName + '.' + a.SystemName";
            }
        }
        public string GetItemsIdsQuery
        {
            get
            {
                return @"select convert(varchar(36), a.ItemGUID) + '-1'
				from SPM_Action a
				where a.ItemGUID is not null";
            }
        }
        public string GetItemsNamesQuery
        {
            get
            {
                return @"select upper(pa.SystemName + '.' + a.SystemName) + '-1'
				from SPM_Action a, 
				SPM_ActionAsso asso,
				SPM_Action pa,
				SPM_ActionAsso asso2,
				SPM_Action roota
				where a.ActionID = asso.ActionID and pa.ActionID = asso.ParentActionID and
				pa.ActionID = asso2.ActionID and roota.ActionID = asso2.ParentActionID 
				order by pa.SystemName + '.' + a.SystemName";
            }
        }
    }
}