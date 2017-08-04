using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Identity
{
	public class UserOptions
	{
		public static string EngLettersAndDigitsChars = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890_";
		public static string EngRusLettersAndDigitsChars = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnmАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя1234567890_";


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
			AllowedLoginChars = EngLettersAndDigitsChars;
			MaxLoginLength = 32;
			RequireEmail = true;
			RequireUniqueEmail = true;
		}
	}
}
