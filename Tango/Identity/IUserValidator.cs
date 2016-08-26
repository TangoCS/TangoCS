using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tango.Identity
{
	public interface IUserValidator<TKey>
	{
		List<ValidationMessage> CheckPassword(string password);
		List<ValidationMessage> CheckName(TKey userId, string name);
		List<ValidationMessage> CheckEmail(TKey userId, string email);
	}
}
