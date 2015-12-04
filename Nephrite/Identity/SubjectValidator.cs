using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.Identity
{
	public interface ISubjectValidator
	{
		List<ValidationMessage> CheckPassword(string password1, string password2);
		List<ValidationMessage> CheckName(string name);
		List<ValidationMessage> CheckEmail(string email);
	}

	public class DefaultSubjectValidator<TKey> : ISubjectValidator
	{
		SubjectOptions _options;
		IDC_Identity<TKey> _dc;

		public DefaultSubjectValidator(IDC_Identity<TKey> dataContext, SubjectOptions options = null)
		{
			_options = options ?? new SubjectOptions();
			_dc = dataContext;
		}

		public virtual List<ValidationMessage> CheckPassword(string password1, string password2)
		{
			List<ValidationMessage> res = new List<ValidationMessage>();
			char[] pwdChars = _options.AllowedPasswordChars.ToCharArray();

			if (password1 != password2)
			{
				res.Add(new ValidationMessage("Введенные пароли не совпадают!"));
			}

			if (password1.Length < _options.MinPasswordLength)
			{
				res.Add(new ValidationMessage("Пароль должен быть не короче " + _options.MinPasswordLength.ToString() + " символов!"));
			}

			foreach (char c in password1.ToCharArray())
			{
				if (!pwdChars.Contains(c))
				{
					res.Add(new ValidationMessage("Пароль содержит недопустимые символы!"));
					break;
				}
			}

			return res;
		}

		public virtual List<ValidationMessage> CheckName(string name)
		{
			List<ValidationMessage> res = new List<ValidationMessage>();
			char[] loginChars = _options.AllowedLoginChars.ToCharArray();

			if (String.IsNullOrEmpty(name))
			{
				res.Add(new ValidationMessage("Необходимо ввести имя пользователя!"));
			}

			if (name.Length > _options.MaxLoginLength)
			{
				res.Add(new ValidationMessage("Длина имени пользователя не должна превышать " + _options.MaxLoginLength.ToString() + " символов!"));
			}

			foreach (char c in name.ToCharArray())
			{
				if (!loginChars.Contains(c))
				{
					res.Add(new ValidationMessage("Имя пользователя содержит недопустимые символы!"));
					break;
				}
			}

			if (_dc.SubjectFromName<Subject<TKey>>(name) != null)
			{
				res.Add(new ValidationMessage("Введенное имя пользователя уже существует в системе!"));
			}

			return res;
		}

		public virtual List<ValidationMessage> CheckEmail(string email)
		{
			List<ValidationMessage> res = new List<ValidationMessage>();

			if (_options.RequireEmail && String.IsNullOrEmpty(email))
			{
				res.Add(new ValidationMessage("Необходимо ввести email!"));
			}

			if (_options.RequireUniqueEmail && _dc.SubjectFromEmail<Subject<TKey>>(email) != null)
			{
				res.Add(new ValidationMessage("В системе уже зарегистрирован пользователь с указанным адресом электронной почты!"));
			}

			return res;
		}
	}

	public class SubjectOptions
	{
		public int MinPasswordLength { get; set; }
		public string AllowedPasswordChars { get; set; }
		public string AllowedLoginChars { get; set; }
		public int MaxLoginLength { get; set; }
		public bool RequireEmail { get; set; }
		public bool RequireUniqueEmail { get; set; }

		public SubjectOptions()
		{
			MinPasswordLength = 4;
			AllowedPasswordChars = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890~!@#$%^&*()-=\\][{}|+_`';:/?.>,<";
			AllowedLoginChars = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890_";
			MaxLoginLength = 12;
			RequireEmail = true;
			RequireUniqueEmail = true;
		}
	}
}
