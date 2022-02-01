using System;
using System.Text.RegularExpressions;

namespace Tango
{
	public static class StringUtils
	{
		public static string GetInitials(string surname, string firstname, string patronymic)
		{
			string f = String.IsNullOrEmpty(firstname) ? "" : " " + firstname[0] + ".";
			string p = String.IsNullOrEmpty(patronymic) ? "" : " " + patronymic[0] + ".";
			return surname + f + p;
		}

		public static string GetInitials(string firstname, string patronymic)
		{
			string f = String.IsNullOrEmpty(firstname) ? "" : firstname[0] + ".";
			string p = String.IsNullOrEmpty(patronymic) ? "" : " " + patronymic[0] + ".";
			return f + p;
		}

		static int tinSumm(string s, int[] c)
		{
			int r = 0;
			for (int i = 0; i < c.Length; i++)
				r += c[i] * int.Parse(s[i].ToString());
			return r;
		}

		public static bool ValidateTIN(string str)
		{
			if (str.Length != 10 && str.Length != 12)
				return false;

			for (int i = 0; i < str.Length; i++)
				if (!char.IsDigit(str[i]))
					return false;

			if (str.Length == 10)
			{
				int[] coeff = { 2, 4, 10, 3, 5, 9, 4, 6, 8 };
				int n10 = (tinSumm(str, coeff) % 11) % 10;
				return str[9].ToString() == n10.ToString();
			}
			if (str.Length == 12)
			{
				int[] coeff1 = { 7, 2, 4, 10, 3, 5, 9, 4, 6, 8 };
				int[] coeff2 = { 3, 7, 2, 4, 10, 3, 5, 9, 4, 6, 8 };
				int n11 = (tinSumm(str, coeff1) % 11) % 10;
				int n12 = (tinSumm(str, coeff2) % 11) % 10;
				return str[10].ToString() == n11.ToString() && str[11].ToString() == n12.ToString();
			}
			return false;
		}

		public static bool ValidateOGRN(string str)
		{
			if (str.Length != 13 && str.Length != 15)
				return false;
			for (int i = 0; i < str.Length; i++)
				if (!char.IsDigit(str[i]))
					return false;
			decimal d = decimal.Parse(str.Substring(0, str.Length - 1));
			string checknum = (d % (str.Length - 2)).ToString();
			return str[12].ToString() == checknum.Substring(checknum.Length - 1);
		}

		public static bool ValidateOKPO(string str)
		{
			for (int i = 0; i < str.Length; i++)
				if (!char.IsDigit(str[i]))
					return false;
			return true;
		}

		public static bool ValidateKPP(string str)
		{
			if (str.Length != 9)
				return false;
			for (int i = 0; i < str.Length; i++)
				if (!char.IsDigit(str[i]))
					return false;
			return true;
		}

		const string emailregex = "^[-a-z0-9!#$%&'*+/=?^_`{|}~]+(\\.[-a-z0-9!#$%&'*+/=?^_`{|}~]+)*@([a-z0-9]([-a-z0-9]{0,61}[a-z0-9])?\\.)*(aero|arpa|asia|biz|cat|com|coop|edu|gov|info|int|jobs|mil|mobi|museum|name|net|org|pro|tel|travel|[a-z][a-z])$";
		public static bool ValidateEmail(string str)
		{
			if (str.IsEmpty())
				return true;
			return Regex.Match(str.Trim(), emailregex, RegexOptions.IgnoreCase).Success;
		}

		public static bool ValidateContainsEmail(string str)
		{
			if (str.IsEmpty())
				return true;
			return Regex.Match(str.Trim(), "(" + emailregex.Substring(1, emailregex.Length - 2) + ")+").Success;
		}

		const string ipregex = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
		public static bool ValidateIP(string str)
		{
			if (str.IsEmpty())
				return true;
			return Regex.Match(str.Trim(), ipregex, RegexOptions.IgnoreCase).Success;
		}

		const string wwwregex = @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$";
		public static bool ValidateWWW(string str)
		{
			if (str.IsEmpty())
				return true;
			return Regex.Match(str.Trim(), wwwregex, RegexOptions.IgnoreCase).Success;
		}

		public static int GetDamerauLevenshteinDistance(string s, string t)
		{
			var snull = string.IsNullOrEmpty(s);
			var tnull = string.IsNullOrEmpty(t);

			if (snull && tnull)
				return 0;

			if (snull)
				return t.Length;

			if (tnull)
				return s.Length;

			int n = s.Length; // length of s
			int m = t.Length; // length of t

			if (n == 0)
				return m;

			if (m == 0)
				return n;

			int[] p = new int[n + 1]; //'previous' cost array, horizontally
			int[] d = new int[n + 1]; // cost array, horizontally

			// indexes into strings s and t
			int i; // iterates through s
			int j; // iterates through t

			for (i = 0; i <= n; i++)
				p[i] = i;

			for (j = 1; j <= m; j++)
			{
				char tJ = t[j - 1]; // jth character of t
				d[0] = j;

				for (i = 1; i <= n; i++)
				{
					int cost = s[i - 1] == tJ ? 0 : 1; // cost
					// minimum of cell to the left+1, to the top+1, diagonally left and up +cost                
					d[i] = Math.Min(Math.Min(d[i - 1] + 1, p[i] + 1), p[i - 1] + cost);
				}

				// copy current distance counts to 'previous row' distance counts
				int[] dPlaceholder = p; //placeholder to assist in swapping p and d
				p = d;
				d = dPlaceholder;
			}

			// our last action in the above loop was to switch d and p, so p now 
			// actually has the most recent cost counts
			return p[n];
		}
	}
}
