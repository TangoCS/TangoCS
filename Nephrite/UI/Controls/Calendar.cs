using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Nephrite.Data;
using Nephrite.Html;

namespace Nephrite.UI.Controls
{
	public static class CalendarExtension
	{
		public static void Calendar(this LayoutWriter w, string name, DateTime? value = null, bool enabled = true, bool showTime = false)
		{
			string basePath = GlobalSettings.JSPath + "Calendar/";

			//c.Page.RegisterScript("calendar-setup", basePath + "calendar-setup_stripped.js");

			w.TextBox(name, showTime ? value.DateTimeToString() : value.DateToString(), a =>
				a.ID(name).Placeholder("ДД.ММ.ГГГГ").Style("width:" + (showTime ? "130px" : "100px"))
				.OnKeyPress("return jscal_calendarHelper(event)").Disabled(!enabled)
			);
			if (enabled)
			{
				w.Img(a => a.ID("btn" + name).Title("Календарь").Src(basePath + "img.gif"));

				w.Includes.Add(GlobalSettings.JSPath + "calendar/calendar_stripped.js");
				w.Includes.Add(GlobalSettings.JSPath + "calendar/lang/calendar-en.js");	

				w.AddClientAction("Calendar", "setup", new {
					inputField = w.GetID(name),
					button = w.GetID("btn" + name),
					showOthers = true,
					weekNumbers = false,
					showTime = showTime,
					ifFormat = showTime ? "%d.%m.%Y %H:%M" : "%d.%m.%Y",
					timeFormat = "24",
					dateStatusFunc = ConfigurationManager.AppSettings["UseCalendarDaysInJSCalendar"] == "true" ? "jscal_calendarDate" : null
				});
			}

//				if (ConfigurationManager.AppSettings["UseCalendarDaysInJSCalendar"] == "true")
//				{
//					c.Page.RegisterStartupScript("calendar-workdays", @"
//function jscal_calendarDate_c(arr, date) {
//	for(var i = 0; i < arr.length; i++)
//		if( arr[i].toString() == date.toString())
//			return true;
//	return false;
//}
//function jscal_calendarDate(date, y, m, d){" + CalendarDayCache.ToJSArray("w", "h") + @"
//	if (jscal_calendarDate_c(w, new Date(date.getFullYear(), date.getMonth(), d)))
//		return 'cal-workingday';
//	if (jscal_calendarDate_c(h, new Date(date.getFullYear(), date.getMonth(), d)))
//		return 'cal-holiday';
//	if (date.getDay() == 0 || date.getDay() == 6)
//		return 'cal-holiday';
//	else
//		return 'cal-workingday';
//}");
//				}
			
		}
	}

	public class CalendarDayCache
	{
		static DateTime[] workingDays;
		static DateTime[] holidays;
		static DateTime lastLoad = DateTime.MinValue;
		static object locker = new object();

		static Func<IDC_CalendarDays> DataContext;

		public static void Init(Func<IDC_CalendarDays> dataContext)
		{
			DataContext = dataContext;
		}

		static void Load()
		{
			if (DateTime.Now.Subtract(lastLoad).TotalSeconds > 60)
			{
				lock (locker)
				{
					if (DateTime.Now.Subtract(lastLoad).TotalSeconds > 60)
					{
						var days = DataContext().ICalendarDay.ToArray();
						workingDays = days.Where(o => o.IsWorkingDay).Select(o => o.Date).ToArray();
						holidays = days.Where(o => !o.IsWorkingDay).Select(o => o.Date).ToArray();
						lastLoad = DateTime.Now;
					}
				}
			}
		}

		public static DateTime[] WorkingDays
		{
			get
			{
				Load();
				return workingDays;
			}
		}

		public static DateTime[] Holidays
		{
			get
			{
				Load();
				return holidays;
			}
		}

		public static string ToJSArray(string wName, string hName)
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
		ITable<ICalendarDay> ICalendarDay { get; }
	}

	public interface ICalendarDay : IEntity
	{
		int CalendarDayID { get; set; }
		DateTime Date { get; set; }
		bool IsWorkingDay { get; set; }
	}
}