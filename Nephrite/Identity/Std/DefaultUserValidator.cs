using System;
using System.Collections.Generic;
using System.Linq;

namespace Nephrite.Identity.Std
{
	public class DefaultUserValidator<TUser, TKey> : IUserValidator
		where TKey : IEquatable<TKey>
		where TUser : class
	{
		UserOptions _options;
		IIdentityStore<TUser, TKey> _dc;

		public DefaultUserValidator(IIdentityStore<TUser, TKey> dataContext, UserOptions options = null)
		{
			_options = options ?? new UserOptions();
			_dc = dataContext;
		}

		public virtual List<ValidationMessage> CheckPassword(string password1, string password2)
		{
			List<ValidationMessage> res = new List<ValidationMessage>();
			//char[] pwdChars = _options.AllowedPasswordChars.ToCharArray();

			if (password1 != password2)
			{
				res.Add(new ValidationMessage(Resources.PasswordsAreDifferent));
			}

			if (password1.Length < _options.MinPasswordLength)
			{
				res.Add(new ValidationMessage(string.Format(Resources.PasswordTooShort, _options.MinPasswordLength.ToString())));
			}

			if (_options.RequireNonAlphanumericInPassword && password1.All(IsLetterOrDigit))
			{
				res.Add(new ValidationMessage(Resources.PasswordRequiresNonAlphanumeric));
			}
			if (_options.RequireDigitInPassword && !password1.Any(IsDigit))
			{
				res.Add(new ValidationMessage(Resources.PasswordRequiresDigit));
			}
			if (_options.RequireLowercaseInPassword && !password1.Any(IsLower))
			{
				res.Add(new ValidationMessage(Resources.PasswordRequiresLower));
			}
			if (_options.RequireUppercaseInPassword && !password1.Any(IsUpper))
			{
				res.Add(new ValidationMessage(Resources.PasswordRequiresUpper));
			}

			return res;
		}

		public virtual List<ValidationMessage> CheckName(string name)
		{
			List<ValidationMessage> res = new List<ValidationMessage>();
			char[] loginChars = _options.AllowedLoginChars.ToCharArray();

			if (String.IsNullOrEmpty(name))
			{
				res.Add(new ValidationMessage(Resources.EmptyUserName));
			}

			if (name.Length > _options.MaxLoginLength)
			{
				res.Add(new ValidationMessage(string.Format(Resources.InvalidUserNameLength, _options.MaxLoginLength.ToString())));
			}

			foreach (char c in name.ToCharArray())
			{
				if (!loginChars.Contains(c))
				{
					res.Add(new ValidationMessage(Resources.InvalidUserName));
					break;
				}
			}

			if (_dc.UserFromName(name) != null)
			{
				res.Add(new ValidationMessage(Resources.LoginAlreadyAssociated));
			}

			return res;
		}

		public virtual List<ValidationMessage> CheckEmail(string email)
		{
			List<ValidationMessage> res = new List<ValidationMessage>();

			if (_options.RequireEmail && String.IsNullOrEmpty(email))
			{
				res.Add(new ValidationMessage(Resources.EmptyEmail));
			}

			if (_options.RequireUniqueEmail && _dc.UserFromEmail(email) != null)
			{
				res.Add(new ValidationMessage(string.Format(Resources.DuplicateEmail, email)));
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
