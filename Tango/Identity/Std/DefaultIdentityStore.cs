using System;
using System.Data;
using System.Linq;
using Dapper;

namespace Tango.Identity.Std
{
	public class DefaultIdentityStore : IIdentityStore
	{
		IDbConnection _dc;

		public DefaultIdentityStore(IDbConnection dc)
		{
			_dc = dc;
		}

		readonly string subjSelect = @"select ID, UserName, Title, PasswordHash, LockoutEnabled, MustChangePassword, Email, SecurityStamp from V_IdentityUser";

		public IdentityUser UserFromName(string name)
		{
			return _dc.Query<IdentityUser>(subjSelect + " where lower(UserName) = @p1", new { p1 = name.ToLower() }).FirstOrDefault();
		}

		public IdentityUser UserFromProviderKey(string providerName, string providerKey)
		{
			string provSelect = subjSelect + "u join V_IdentityUser_{0} l on u.ID = l.ID where lower(ProviderKey) = @p1";
			return _dc.Query<IdentityUser>(string.Format(provSelect, providerName), new { p1 = providerKey.ToLower() }).FirstOrDefault();
		}

		public IdentityUser UserFromID(int id)
		{
			return _dc.Query<IdentityUser>(subjSelect + " where ID = @p1", new { p1 = id }).FirstOrDefault();
		}

		public IdentityUser UserFromEmail(string email)
		{
			return _dc.Query<IdentityUser>(subjSelect + " where lower(Email) = @p1", new { p1 = email.ToLower() }).FirstOrDefault();
		}
	}
}
