using System;
using System.Collections.Generic;
using System.Linq;

namespace Tango.Identity.Std
{
	public class DefaultUserValidator : IUserValidator
	{
		UserOptions _options;
		IIdentityStore _dc;

		public DefaultUserValidator(IIdentityStore dataContext, UserOptions options = null)
		{
			_options = options ?? new UserOptions();
			_dc = dataContext;
		}

		public virtual List<ValidationMessage> CheckPassword(string password)
		{
			List<ValidationMessage> res = new List<ValidationMessage>();
			//char[] pwdChars = _options.AllowedPasswordChars.ToCharArray();

			if (password.Length < _options.MinPasswordLength)
			{
				res.Add(new ValidationMessage("password", string.Format(Resources.PasswordTooShort, _options.MinPasswordLength.ToString())));
			}

			if (_options.RequireNonAlphanumericInPassword && password.All(IsLetterOrDigit))
			{
				res.Add(new ValidationMessage("password", Resources.PasswordRequiresNonAlphanumeric));
			}
			if (_options.RequireDigitInPassword && !password.Any(IsDigit))
			{
				res.Add(new ValidationMessage("password", Resources.PasswordRequiresDigit));
			}
			if (_options.RequireLowercaseInPassword && !password.Any(IsLower))
			{
				res.Add(new ValidationMessage("password", Resources.PasswordRequiresLower));
			}
			if (_options.RequireUppercaseInPassword && !password.Any(IsUpper))
			{
				res.Add(new ValidationMessage("password", Resources.PasswordRequiresUpper));
			}

			return res;
		}

		public virtual List<ValidationMessage> CheckName(int userId, string name)
		{
			List<ValidationMessage> res = new List<ValidationMessage>();
			char[] loginChars = _options.AllowedLoginChars.ToCharArray();

			if (String.IsNullOrEmpty(name))
			{
				res.Add(new ValidationMessage("username", Resources.EmptyUserName));
			}

			if (name.Length > _options.MaxLoginLength)
			{
				res.Add(new ValidationMessage("username", string.Format(Resources.InvalidUserNameLength, _options.MaxLoginLength.ToString())));
			}

			foreach (char c in name.ToCharArray())
			{
				if (!loginChars.Contains(c))
				{
					res.Add(new ValidationMessage("username", Resources.InvalidUserName));
					break;
				}
			}

			if (_dc.UserFromName(name) != null)
			{
				res.Add(new ValidationMessage("username", Resources.LoginAlreadyAssociated));
			}

			return res;
		}

		public virtual List<ValidationMessage> CheckEmail(int userId, string email)
		{
			List<ValidationMessage> res = new List<ValidationMessage>();

			if (_options.RequireEmail && String.IsNullOrEmpty(email))
			{
				res.Add(new ValidationMessage("email", Resources.EmptyEmail));
			}

			var owner = _dc.UserFromEmail(email);
			if (_options.RequireUniqueEmail && owner != null && !owner.Id.Equals(userId))
			{
				res.Add(new ValidationMessage("email", string.Format(Resources.DuplicateEmail, email)));
			}

			return res;
		}

		public virtual bool IsDigit(char c)
		{
			return c >= '0' && c <= '9';
		}

		public virtual bool IsLower(char c)
		{
			return c >= 'a' && c <= 'z';
		}

		public virtual bool IsUpper(char c)
		{
			return c >= 'A' && c <= 'Z';
		}

		public virtual bool IsLetterOrDigit(char c)
		{
			return IsUpper(c) || IsLower(c) || IsDigit(c);
		}
	}
}
