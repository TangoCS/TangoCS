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

	public class DefaultUserValidator<TKey> : IUserValidator
		where TKey : IEquatable<TKey>
	{
		UserOptions _options;
		IDC_Identity<IdentityUser<TKey>, TKey> _dc;

		public DefaultUserValidator(IDC_Identity<IdentityUser<TKey>, TKey> dataContext, UserOptions options = null)
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

	public class UserOptions
	{
		public int MinPasswordLength { get; set; }
		//public string AllowedPasswordChars { get; set; }

		public bool RequireNonAlphanumericInPassword { get; set; } = false;
		public bool RequireLowercaseInPassword { get; set; } = false;
		public bool RequireUppercaseInPassword { get; set; } = false;
		public bool RequireDigitInPassword { get; set; } = false;

		public string AllowedLoginChars { get; set; }
		public int MaxLoginLength { get; set; }

		public bool RequireEmail { get; set; }
		public bool RequireUniqueEmail { get; set; }

		public UserOptions()
		{
			MinPasswordLength = 6;
			//AllowedPasswordChars = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890~!@#$%^&*()-=\\][{}|+_`';:/?.>,<";
			AllowedLoginChars = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890_";
			MaxLoginLength = 12;
			RequireEmail = true;
			RequireUniqueEmail = true;
		}
	}
}
