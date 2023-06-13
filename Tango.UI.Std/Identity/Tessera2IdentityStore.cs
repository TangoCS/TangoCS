using Dapper;
using System;
using System.Data;
using System.Linq;

namespace Tango.Identity.Std
{
	public class Tessera2IdentityStore : IIdentityStore
	{
		IDbConnection _dc;

		public Tessera2IdentityStore(IDbConnection dc)
		{
			_dc = dc;
		}

		string subjSelect = @"select SubjectID as ID, SystemName as UserName, Title, CONVERT(VARCHAR(MAX), PasswordHash, 2) as PasswordHash, ~IsActive as LockoutEnabled, MustChangePassword, Email, RegMagicString as SecurityStamp from SPM_Subject";

		public IdentityUser UserFromName(string name)
		{
			return _dc.Query<IdentityUser>(subjSelect + " where lower(SystemName) = @p1", new { p1 = name.ToLower() }).FirstOrDefault();
		}

		public IdentityUser UserFromProviderKey(string providerName, string providerKey)
		{
			throw new NotSupportedException();
			//string provSelect = subjSelect + "u join V_IdentityUser_{0} l on u.ID = l.ID where lower(ProviderKey) = @p1";
			//return _dc.Query<TUser>(string.Format(provSelect, providerName), new { p1 = providerKey.ToLower() }).FirstOrDefault();
		}

		public IdentityUser UserFromID(long id)
		{
			return _dc.Query<IdentityUser>(subjSelect + " where SubjectID = @p1", new { p1 = id }).FirstOrDefault();
		}

		public IdentityUser UserFromEmail(string email)
		{
			return _dc.Query<IdentityUser>(subjSelect + " where lower(Email) = @p1", new { p1 = email.ToLower() }).FirstOrDefault();
		}

		public void Activate(long id)
		{
			throw new NotImplementedException();
		}

		public void Deactivate(long id)
		{
			throw new NotImplementedException();
		}
	}
}
