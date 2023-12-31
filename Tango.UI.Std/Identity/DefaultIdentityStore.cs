using System;
using System.Data;
using System.Linq;
using Dapper;
using Tango.Data;

namespace Tango.Identity.Std
{
	public class DefaultIdentityStore : IIdentityStore
	{
		IDatabase _db;

		public DefaultIdentityStore(IDatabase db)
		{
			_db = db;
		}

		readonly string subjSelect = @"select u.ID, UserName, Title, PasswordHash, LockoutEnabled, MustChangePassword, Email, SecurityStamp from V_IdentityUser u";

		public IdentityUser UserFromName(string name)
		{
			return _db.Connection.Query<IdentityUser>(subjSelect + " where lower(UserName) = @p1", new { p1 = name.ToLower() }).FirstOrDefault();
		}

		public IdentityUser UserFromProviderKey(string providerName, string providerKey)
		{
			string provSelect = subjSelect + " join V_IdentityUser_{0} l on u.ID = l.ID where lower(ProviderKey) = @p1";
			return _db.Connection.Query<IdentityUser>(string.Format(provSelect, providerName), new { p1 = providerKey.ToLower() }).FirstOrDefault();
		}

		public IdentityUser UserFromID(long id)
		{
			return _db.Connection.Query<IdentityUser>(subjSelect + " where ID = @p1", new { p1 = id }).FirstOrDefault();
		}

		public IdentityUser UserFromEmail(string email)
		{
			return _db.Connection.Query<IdentityUser>(subjSelect + " where lower(Email) = @p1 and logintypeid = 0", new { p1 = email.ToLower() }).FirstOrDefault();
		}

		public IdentityUser UserFromSecurityStamp(string securityStamp)
		{
			return _db.Connection.Query<IdentityUser>(subjSelect + " where lower(securitystamp) = @p1", new { p1 = securityStamp.ToLower() }).FirstOrDefault();
		}

		public void Activate(long id)
		{
			_db.Connection.Execute("update spm_subject set isactive = true where subjectid = @subjectid", new {
				subjectid = id
			}, _db.Transaction);
		}

		public void Deactivate(long id)
		{
			_db.Connection.Execute("update spm_subject set isactive = false where subjectid = @subjectid", new {
				subjectid = id
			}, _db.Transaction);
		}

		public void SetSecurityStamp(long id, string securityStamp)
		{
			_db.Connection.Execute("update spm_subject set regmagicstring = @securityStamp where subjectid = @subjectid", new { 
				subjectid = id,
				securityStamp
			}, _db.Transaction);
		}

		public void SetNewPassword(long id, string newHash)
		{
			_db.Connection.Execute("update spm_subject set passwordhash = @newHash, regmagicstring = null where subjectid = @subjectid", new {
				subjectid = id,
				newHash
			}, _db.Transaction);
		}
	}
}
