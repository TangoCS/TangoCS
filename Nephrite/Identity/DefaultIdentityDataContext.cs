using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite.Identity
{
	public class DefaultIdentityDataContext : IDC_Identity<int>
	{
		IDataContext _dc;

		public DefaultIdentityDataContext(IDataContext dc)
		{
			_dc = dc;
		}

		string subjSelect = @"select ID, Name, Title, PasswordHash, IsActive, IsDeleted, SID, MustChangePassword, Email from V_Identity_Subject where {0}";

		public TSubject SubjectFromName<TSubject>(string name)
		{
			return _dc.ExecuteQuery<TSubject>(String.Format(subjSelect, "lower(Name) = ?"), name.ToLower()).FirstOrDefault();
		}

		public TSubject SubjectFromSID<TSubject>(string sid)
		{
			return _dc.ExecuteQuery<TSubject>(String.Format(subjSelect, "lower(SID) = ?"), sid.ToLower()).FirstOrDefault();
		}

		public TSubject SubjectFromID<TSubject>(int id)
		{
			return _dc.ExecuteQuery<TSubject>(String.Format(subjSelect, "ID = ?"), id).FirstOrDefault();
		}

		public TSubject SubjectFromEmail<TSubject>(string email)
		{
			return _dc.ExecuteQuery<TSubject>(String.Format(subjSelect, "lower(Email) = ?"), email.ToLower()).FirstOrDefault();
		}
	}
}
