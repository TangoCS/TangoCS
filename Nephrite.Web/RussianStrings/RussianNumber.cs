using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Collections.Specialized;

namespace Nephrite.Web.RussianStrings
{
	public static class RussianNumber
	{
		static string[] hunds =
        {
            "", "сто ", "двести ", "триста ", "четыреста ",
            "пятьсот ", "шестьсот ", "семьсот ", "восемьсот ", "девятьсот "
        };

		static string[] tens =
        {
            "", "десять ", "двадцать ", "тридцать ", "сорок ", "пятьдесят ",
            "шестьдесят ", "семьдесят ", "восемьдесят ", "девяносто "
        };

		static string Str(int val, bool male, string one, string two, string five)
		{
			string[] frac20 =
            {
                "", "один ", "два ", "три ", "четыре ", "пять ", "шесть ",
                "семь ", "восемь ", "девять ", "десять ", "одиннадцать ",
                "двенадцать ", "тринадцать ", "четырнадцать ", "пятнадцать ",
                "шестнадцать ", "семнадцать ", "восемнадцать ", "девятнадцать "
            };

			int num = val % 1000;
			if (0 == num) return "";
			if (num < 0) throw new ArgumentOutOfRangeException("val", "Параметр не может быть отрицательным");
			if (!male)
			{
				frac20[1] = "одна ";
				frac20[2] = "две ";
			}

			StringBuilder r = new StringBuilder(hunds[num / 100]);

			if (num % 100 < 20)
			{
				r.Append(frac20[num % 100]);
			}
			else
			{
				r.Append(tens[num % 100 / 10]);
				r.Append(frac20[num % 10]);
			}

			r.Append(Case(num, one, two, five));

			if (r.Length != 0) r.Append(" ");
			return r.ToString();
		}

		static string Case(int val, string one, string two, string five)
		{
			int t = (val % 100 > 20) ? val % 10 : val % 20;

			switch (t)
			{
				case 1: return one;
				case 2:
				case 3:
				case 4: return two;
				default: return five;
			}
		}

		public static string ToRussianWords(this decimal val)
		{
			int n = (int)val;
			int remainder = (int)((val - n + 0.005M) * 100);
			if (n == 0)
				return "ноль";
			StringBuilder r = new StringBuilder();

			if (n % 1000 != 0)
				r.Append(RusNumber.Str(n, true, "", "", ""));

			n /= 1000;

			r.Insert(0, RusNumber.Str(n, false, "тысяча", "тысячи", "тысяч"));
			n /= 1000;

			r.Insert(0, RusNumber.Str(n, true, "миллион", "миллиона", "миллионов"));
			n /= 1000;

			r.Insert(0, RusNumber.Str(n, true, "миллиард", "миллиарда", "миллиардов"));
			n /= 1000;

			r.Insert(0, RusNumber.Str(n, true, "триллион", "триллиона", "триллионов"));
			n /= 1000;

			r.Insert(0, RusNumber.Str(n, true, "триллиард", "триллиарда", "триллиардов"));

			return r.ToString();
		}
	}
}
