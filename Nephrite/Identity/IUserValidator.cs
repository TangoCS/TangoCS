using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.Identity
{
	public interface IUserValidator<TKey>
	{
		List<ValidationMessage> CheckPassword(string password1, string password2);
		List<ValidationMessage> CheckName(TKey userId, string name);
		List<ValidationMessage> CheckEmail(TKey userId, string email);
	}
}
