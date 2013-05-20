using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace Nephrite.Web.CalendarDays
{
	internal class CalendarDayCache
	{
		static DateTime[] workingDays;
		static DateTime[] holidays;
		static DateTime lastLoad = DateTime.MinValue;
		static object locker = new object();
		static void Init()
		{
			if (DateTime.Now.Subtract(lastLoad).TotalSeconds > 60)
			{
				lock (locker)
				{
					if (DateTime.Now.Subtract(lastLoad).TotalSeconds > 60)
					{
						var days = ((IDC_CalendarDays)Base.Model).CalendarDay.ToArray();
						workingDays = days.Where(o => o.IsWorkingDay).Select(o => o.Date).ToArray();
						holidays = days.Where(o => !o.IsWorkingDay).Select(o => o.Date).ToArray();
						lastLoad = DateTime.Now;
					}
				}
			}
		}

		internal static DateTime[] WorkingDays
		{
			get
			{
				Init();
				return workingDays;
			}
		}

		internal static DateTime[] Holidays
		{
			get
			{
				Init();
				return holidays;
			}
		}

		internal static string ToJSArray(string wName, string hName)
		{
			StringBuilder sb = new StringBuilder(1000);
			var h = Holidays;
			sb.AppendFormat("var {0}=new Array({1});", hName, h.Length);
			for (int i = 0; i < h.Length; i++)
				sb.AppendFormat("{0}[{1}]=new Date({2},{3},{4});", hName, i, h[i].Year, h[i].Month - 1, h[i].Day);
			var w = WorkingDays;
			sb.AppendFormat("var {0}=new Array({1});", wName, w.Length);
			for (int i = 0; i < w.Length; i++)
				sb.AppendFormat("{0}[{1}]=new Date({2},{3},{4});", wName, i, w[i].Year, w[i].Month - 1, w[i].Day);
			return sb.ToString();
		}
	}

	public interface IDC_CalendarDays : IDataContext
	{
		IQueryable<ICalendarDay> CalendarDay { get; }
	}

	public interface ICalendarDay : IEntity
	{
		int CalendarDayID { get; set; }
		System.DateTime Date { get; set; }
		bool IsWorkingDay { get; set; }
	}
}
