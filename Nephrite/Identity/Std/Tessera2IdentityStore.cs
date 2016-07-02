using Dapper;
using System;
using System.Data;
using System.Linq;

namespace Nephrite.Identity.Std
{
	public class Tessera2IdentityStore<TUser, TKey> : IIdentityStore<TUser, TKey>
		where TKey : IEquatable<TKey>
	{
		IDbConnection _dc;

		public Tessera2IdentityStore(IDbConnection dc)
		{
			_dc = dc;
		}

		string subjSelect = @"select SubjectID as ID, SystemName as UserName, Title, CONVERT(VARCHAR(MAX), PasswordHash, 2) as PasswordHash, ~IsActive as LockoutEnabled, MustChangePassword, Email, RegMagicString as SecurityStamp from SPM_Subject";

		public TUser UserFromName(string name)
		{
			return _dc.Query<TUser>(subjSelect + " where lower(SystemName) = @p1", new { p1 = name.ToLower() }).FirstOrDefault();
		}

		public TUser UserFromProviderKey(string providerName, string providerKey)
		{
			throw new NotSupportedException();
			//string provSelect = subjSelect + "u join V_IdentityUser_{0} l on u.ID = l.ID where lower(ProviderKey) = @p1";
			//return _dc.Query<TUser>(string.Format(provSelect, providerName), new { p1 = providerKey.ToLower() }).FirstOrDefault();
		}

		public TUser UserFromID(TKey id)
		{
			return _dc.Query<TUser>(subjSelect + " where SubjectID = @p1", new { p1 = id }).FirstOrDefault();
		}

		public TUser UserFromEmail(string email)
		{
			return _dc.Query<TUser>(subjSelect + " where lower(Email) = @p1", new { p1 = email.ToLower() }).FirstOrDefault();
		}
	}
}
