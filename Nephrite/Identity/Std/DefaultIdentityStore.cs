using System;
using System.Data;
using System.Linq;
using Dapper;

namespace Nephrite.Identity.Std
{
	public class DefaultIdentityStore<TUser, TKey> : IIdentityStore<TUser, TKey>
		where TKey : IEquatable<TKey>
	{
		IDbConnection _dc;

		public DefaultIdentityStore(IDbConnection dc)
		{
			_dc = dc;
		}

		string subjSelect = @"select ID, UserName, Title, PasswordHash, LockoutEnabled, MustChangePassword, Email, SecurityStamp from V_IdentityUser";

		public TUser UserFromName(string name)
		{
			return _dc.Query<TUser>(subjSelect + " where lower(UserName) = @p1", new { p1 = name.ToLower() }).FirstOrDefault();
		}

		public TUser UserFromProviderKey(string providerName, string providerKey)
		{
			string provSelect = subjSelect + "u join V_IdentityUser_{0} l on u.ID = l.ID where lower(ProviderKey) = @p1";
			return _dc.Query<TUser>(string.Format(provSelect, providerName), new { p1 = providerKey.ToLower() }).FirstOrDefault();
		}

		public TUser UserFromID(TKey id)
		{
			return _dc.Query<TUser>(subjSelect + " where ID = @p1", new { p1 = id }).FirstOrDefault();
		}

		public TUser UserFromEmail(string email)
		{
			return _dc.Query<TUser>(subjSelect + " where lower(Email) = @p1", new { p1 = email.ToLower() }).FirstOrDefault();
		}
	}
}
