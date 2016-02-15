using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.Identity
{
	public interface IUserValidator
	{
		List<ValidationMessage> CheckPassword(string password1, string password2);
		List<ValidationMessage> CheckName(string name);
		List<ValidationMessage> CheckEmail(string email);
	}
}
