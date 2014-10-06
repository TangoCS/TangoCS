using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web.Controls.Scripts
{
    public class SPM2ScriptDB2 : ISPM2Script
    {
		string subjSelect = @"select SubjectID as ID, SystemName as ""Login"", Title as ""Title"", PasswordHash as ""PasswordHash"", 
IsActive as ""_IsActive"", IsDeleted as ""_IsDeleted"", SID , MustChangePassword as ""_MustChangePassword"", 
Email as ""Email"" from SPM_Subject where {0}";

        public string FromLogin
        {
			get { return String.Format(subjSelect, "lower(SystemName) = ?"); }
        }
        public string FromSID
        {
			get { return String.Format(subjSelect, "SID = ? or lower(SystemName) = ?"); }
        }
        public string FromID
        {
			get { return String.Format(subjSelect, "SubjectID = ?"); }
        }
        public string FromEmail
        {
			get { return String.Format(subjSelect, "lower(Email) = ?"); }
        }
        public string GetRolesAccessByIdQuery
        {
            get
            {
				return @"select ucase(a.SystemName) || '-1-' || cast(ra.RoleID as varchar(10))
				from dbo.SPM_Action a, dbo.SPM_RoleAccess ra
				where ra.ActionID = a.ActionID
				order by a.SystemName";
            }
        }
        public string GetRolesAccessByNameQuery
        {
            get
            {
				return @"select ucase(a.SystemName) || '-1-' || cast(ra.RoleID as varchar(10))
				from dbo.SPM_Action a, dbo.SPM_RoleAccess ra
				where ra.ActionID = a.ActionID
				order by a.SystemName";
            }
        }
        public string GetItemsIdsQuery
        {
            get
            {
				return @"select ucase(a.SystemName) || '-1'
				from DBO.SPM_Action a
				order by a.SystemName";
            }
        }
        public string GetItemsNamesQuery
        {
            get
            {
				return @"select ucase(a.SystemName) || '-1'
				from DBO.SPM_Action a
				order by a.SystemName";
            }
        }
    }
}