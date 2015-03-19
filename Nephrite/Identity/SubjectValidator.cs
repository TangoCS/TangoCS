using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.Identity
{
	public interface ISubjectValidator
	{
		string CheckPassword(string password1, string password2);
		string CheckName(string name);
		string CheckEmail(string email);
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

		public virtual string CheckPassword(string password1, string password2)
		{
			string lMess = "";
			char[] pwdChars = _options.AllowedPasswordChars.ToCharArray();

			if (password1 != password2)
			{
				lMess = "Введенные пароли не совпадают!";
			}

			if (password1.Length < _options.MinPasswordLength)
			{
				if (!String.IsNullOrEmpty(lMess)) lMess += "<br />";
				lMess += "Пароль должен быть не короче " + _options.MinPasswordLength.ToString() + " символов!";
			}

			foreach (char c in password1.ToCharArray())
			{
				if (!pwdChars.Contains(c))
				{
					if (!String.IsNullOrEmpty(lMess)) lMess += "<br />";
					lMess += "Пароль содержит недопустимые символы!";
					break;
				}
			}

			return lMess;
		}

		public virtual string CheckName(string name)
		{
			string lMess = "";
			char[] loginChars = _options.AllowedLoginChars.ToCharArray();

			if (String.IsNullOrEmpty(name))
			{
				if (!String.IsNullOrEmpty(lMess)) lMess += "<br />";
				lMess += "Необходимо ввести имя пользователя!";
			}

			if (name.Length > _options.MaxLoginLength)
			{
				if (!String.IsNullOrEmpty(lMess)) lMess += "<br />";
				lMess += "Длина имени пользователя не должна превышать " + _options.MaxLoginLength.ToString() + " символов!";
			}


			foreach (char c in name.ToCharArray())
			{
				if (!loginChars.Contains(c))
				{
					if (!String.IsNullOrEmpty(lMess)) lMess += "<br />";
					lMess += "Имя пользователя содержит недопустимые символы!";
					break;
				}
			}

			if (_dc.SubjectFromName<Subject<TKey>>(name) != null)
			{
				if (!String.IsNullOrEmpty(lMess)) lMess += "<br />";
				lMess += "Введенное имя пользователя уже существует в системе!";
			}

			return lMess;
		}

		public virtual string CheckEmail(string email)
		{
			string lMess = "";

			if (_options.RequireEmail && String.IsNullOrEmpty(email))
			{
				if (!String.IsNullOrEmpty(lMess)) lMess += "<br />";
				lMess += "Необходимо ввести имя email!";
			}

			if (_options.RequireUniqueEmail && _dc.SubjectFromEmail<Subject<TKey>>(email) != null)
			{
				if (!String.IsNullOrEmpty(lMess)) lMess += "<br />";
				lMess += "В системе уже зарегистрирован пользователь с указанным адресом электронной почты!";
			}

			return lMess;
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
