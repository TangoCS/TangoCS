using System;
using System.Collections.Generic;
using System.Linq;

namespace Nephrite.RussianStrings
{
	public static partial class RussianExtensions
	{
		public static string DayOfWeekRussian(this DateTime? src)
		{
			if (!src.HasValue)
				return String.Empty;

			switch (src.Value.DayOfWeek)
			{
				case DayOfWeek.Friday:
					return "пятница";
				case DayOfWeek.Monday:
					return "понедельник";
				case DayOfWeek.Saturday:
					return "суббота";
				case DayOfWeek.Sunday:
					return "воскресенье";
				case DayOfWeek.Thursday:
					return "четверг";
				case DayOfWeek.Tuesday:
					return "вторник";
				case DayOfWeek.Wednesday:
					return "среда";
				default:
					return "";
			}
		}

		public static string DayOfWeekRussian(this DateTime src)
		{
			return ((DateTime?)src).DayOfWeekRussian();
		}

		public static string DateToRussianString(this DateTime src, bool dayInQuotes)
		{
			string res = DateToRussianDayMonth(src, dayInQuotes);
			res += " " + src.ToString("yyyy") + " г.";
			return res;
		}

		public static string DateToRussianString(this DateTime? src, bool dayInQuotes)
		{
			if (src.HasValue)
				return src.Value.DateToRussianString(dayInQuotes);
			return "";
		}

		public static string DateToRussianDayMonth(this DateTime src, bool dayInQuotes)
		{
			string res = dayInQuotes ? "«" : "";
			res += src.Day.ToString();
			res += dayInQuotes ? "» " : " ";

			switch (src.Month)
			{
				case 1:
					res += "января";
					break;
				case 2:
					res += "февраля";
					break;
				case 3:
					res += "марта";
					break;
				case 4:
					res += "апреля";
					break;
				case 5:
					res += "мая";
					break;
				case 6:
					res += "июня";
					break;
				case 7:
					res += "июля";
					break;
				case 8:
					res += "августа";
					break;
				case 9:
					res += "сентября";
					break;
				case 10:
					res += "октября";
					break;
				case 11:
					res += "ноября";
					break;
				case 12:
					res += "декабря";
					break;
			}
			return res;
		}

		static string GetNumber1String(int n)
		{

			switch (n)
			{
				case 1:
					return "первого";
				case 2:
					return "второго";
				case 3:
					return "третьего";
				case 4:
					return "четвертого";
				case 5:
					return "пятого";
				case 6:
					return "шестого";
				case 7:
					return "седьмого";
				case 8:
					return "восьмого";
				case 9:
					return "девятого";
				default:
					return "";
			}
		}

		static string GetNumber2String(int n)
		{
			switch (n / 10)
			{
				case 2:
					return "двадцать";
				case 3:
					return "тридцать";
				case 4:
					return "сорок";
				case 5:
					return "пятьдесят";
				case 6:
					return "шестьдесят";
				case 7:
					return "семьдесят";
				case 8:
					return "восемьдесят";
				case 9:
					return "девяносто";
				default:
					return "";
			}
		}

		static string GetNumber3String(int n)
		{
			switch (n)
			{
				case 100:
					return "сто";
				case 200:
					return "двести";
				case 300:
					return "триста";
				case 400:
					return "четыреста";
				case 500:
					return "пятьсот";
				case 600:
					return "шестьсот";
				case 700:
					return "семьсот";
				case 800:
					return "восемьсот";
				case 900:
					return "девятьсот";
				default:
					return "";
			}
		}

		public static string GetNumberString(this int n)
		{
			if (n < 10)
			{
				return GetNumber1String(n);
			}
			if (n >= 10 && n < 20)
			{
				switch (n)
				{
					case 10:
						return "десятого";
					case 11:
						return "одиннадцатого";
					case 12:
						return "двенадцатого";
					case 13:
						return "тринадцатого";
					case 14:
						return "четырнадцатого";
					case 15:
						return "пятнадцатого";
					case 16:
						return "шестнадцатого";
					case 17:
						return "семнадцатого";
					case 18:
						return "восемнадцатого";
					case 19:
						return "девятнадцатого";
				}
			}
			if (n >= 20 && n < 100)
			{
				switch (n)
				{
					case 20:
						return "двадцатого";
					case 30:
						return "тридцатого";
					case 40:
						return "сорокового";
					case 50:
						return "пятидесятого";
					case 60:
						return "шестидесятого";
					case 70:
						return "семидесятого";
					case 80:
						return "восьмидесятого";
					case 90:
						return "девяностого";
				}
				return GetNumber2String(n) + " " + GetNumber1String(n - 10 * (n / 10));
			}

			if (n >= 100 && n < 1000)
			{
				switch (n)
				{
					case 100:
						return "сотого";
					case 200:
						return "двухсотого";
					case 300:
						return "трехсотого";
					case 400:
						return "четырехсотого";
					case 500:
						return "пятисотого";
					case 600:
						return "шестисотого";
					case 700:
						return "семисотого";
					case 800:
						return "восьмисотого";
					case 900:
						return "девятисотого";
				}
				return GetNumber3String(100 * (n / 100)) + " " + GetNumberString(n - 100 * (n / 100));
			}

			if (n >= 1000 && n < 10000)
			{
				if (n == 1000)
					return "тысячного";
				if (n < 2000)
					return "одна тысяча " + GetNumberString(n - 1000);
				if (n < 3000)
					return "две тысячи " + GetNumberString(n - 2000);
				if (n < 4000)
					return "три тысячи " + GetNumberString(n - 3000);
				if (n < 5000)
					return "четыре тысячи " + GetNumberString(n - 4000);
				if (n < 6000)
					return "пять тысяч " + GetNumberString(n - 5000);
			}
			return "";
		}
	}
}