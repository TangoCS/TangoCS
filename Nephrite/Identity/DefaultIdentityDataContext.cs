using System;
using System.Data;
using System.Linq;
using Dapper;

namespace Nephrite.Identity
{
	public class DefaultIdentityDataContext : IDC_Identity<int>
	{
		IDbConnection _dc;

		public DefaultIdentityDataContext(IDbConnection dc)
		{
			_dc = dc;
		}

		string subjSelect = @"select ID, Name, Title, PasswordHash, IsActive, IsDeleted, SID, MustChangePassword, Email from V_Identity_Subject where {0}";

		public TSubject SubjectFromName<TSubject>(string name)
		{
			return _dc.Query<TSubject>(String.Format(subjSelect, "lower(Name) = @p1"), new { p1 = name.ToLower() }).FirstOrDefault();
		}

		public TSubject SubjectFromSID<TSubject>(string sid)
		{
			return _dc.Query<TSubject>(String.Format(subjSelect, "lower(SID) = @p1"), new { p1 = sid.ToLower() }).FirstOrDefault();
		}

		public TSubject SubjectFromID<TSubject>(int id)
		{
			return _dc.Query<TSubject>(String.Format(subjSelect, "ID = @p1"), new { p1 = id }).FirstOrDefault();
		}

		public TSubject SubjectFromEmail<TSubject>(string email)
		{
			return _dc.Query<TSubject>(String.Format(subjSelect, "lower(Email) = @p1"), new { p1 = email.ToLower() }).FirstOrDefault();
		}
	}
}
