using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web.Controls.Scripts
{
    public class SPM2ScriptMSSQL : ISPM2Script
    {
		string subjSelect = @"select SubjectID as ID, SystemName as ""Login"", Title, PasswordHash as ""PasswordHash"", 
cast(IsActive as int) as ""_IsActive"", cast(IsDeleted as int) as ""_IsDeleted"", SID , cast(MustChangePassword as int) as ""_MustChangePassword"", 
Email from SPM_Subject where {0}";

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
				return @"select upper(a.SystemName) + '-1-' + convert(varchar, ra.RoleID)
				from SPM_Action a, SPM_RoleAccess ra
				where ra.ActionID = a.ActionID
				order by a.SystemName";
            }
        }
        public string GetRolesAccessByNameQuery
        {
            get
            {
                return @"select upper(a.SystemName) + '-1-' + convert(varchar, ra.RoleID)
				from SPM_Action a, SPM_RoleAccess ra
				where ra.ActionID = a.ActionID
				order by a.SystemName";
            }
        }
        public string GetItemsIdsQuery
        {
            get
            {
				return @"select upper(a.SystemName) + '-1'
				from SPM_Action a
				order by a.SystemName";
            }
        }
        public string GetItemsNamesQuery
        {
            get
            {
                return @"select upper(a.SystemName) + '-1'
				from SPM_Action a
				order by a.SystemName";
            }
        }
    }
}