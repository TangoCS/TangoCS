using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Nephrite.Html
{
	public static class CalendarExtension
	{
		public static void Calendar(this HtmlWriter c, string name, DateTime? value = null, bool enabled = true, bool showTime = false)
		{
			string basePath = Settings.JSPath + "Calendar/";

			c.Page.RegisterScript("calendar", basePath + "calendar_stripped.js");
			c.Page.RegisterScript("calendar-ru", basePath + "lang/calendar-ru.js");
			c.Page.RegisterScript("calendar-setup", basePath + "calendar-setup_stripped.js");

			c.TextBox(name, showTime ? value.DateTimeToString() : value.DateToString(), (a) =>
			{
				a.ID = name;
				a.Placeholder = "ДД.ММ.ГГГГ";
				a.Style = "width:" + (showTime ? "130px" : "100px");
				a.OnKeyPress = "return jscal_calendarHelper(event)";
				a.Disabled = !enabled;
			});
			if (enabled)
			{
				c.Button("btn" + name, "Календарь", (a) =>
				{
					a.ID = "btn" + name;
					a.Src = basePath + "img.gif";
				});
			}

			if (enabled)
			{
				StringBuilder init = new StringBuilder();
				init.AppendFormat(@"if (document.getElementById('{0}') != null) Calendar.setup(
                    {
                      inputField  : ""{0}"",
                      button      : ""btn{0}"",
                      showOthers  : true,
                      weekNumbers : false,", name);
				if (ConfigurationManager.AppSettings["UseCalendarDaysInJSCalendar"] == "true")
					init.Append("dateStatusFunc : jscal_calendarDate,");

				if (showTime)
				{
					init.Append(@"
                      ifFormat    : ""%d.%m.%Y %H:%M"",
                      showsTime   : true,
                      timeFormat  : ""24""
                    }
                 );");
				}
				else
				{
					init.Append(@"
                      ifFormat    : ""%d.%m.%Y""
                    }
                 );");
				}
				c.Page.RegisterStartupScript("calendar-" + name, init.ToString());

				if (ConfigurationManager.AppSettings["UseCalendarDaysInJSCalendar"] == "true")
				{
					c.Page.RegisterStartupScript("calendar-workdays", @"
function jscal_calendarDate_c(arr, date) {
	for(var i = 0; i < arr.length; i++)
		if( arr[i].toString() == date.toString())
			return true;
	return false;
}
function jscal_calendarDate(date, y, m, d){" + CalendarDayCache.ToJSArray("w", "h") + @"
	if (jscal_calendarDate_c(w, new Date(date.getFullYear(), date.getMonth(), d)))
		return 'cal-workingday';
	if (jscal_calendarDate_c(h, new Date(date.getFullYear(), date.getMonth(), d)))
		return 'cal-holiday';
	if (date.getDay() == 0 || date.getDay() == 6)
		return 'cal-holiday';
	else
		return 'cal-workingday';
}");
				}
			}
		}
	}

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
						var days = ((IDC_CalendarDays)A.Model).ICalendarDay.ToArray();
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
		ITable<ICalendarDay> ICalendarDay { get; }
	}

	public interface ICalendarDay : IEntity
	{
		int CalendarDayID { get; set; }
		DateTime Date { get; set; }
		bool IsWorkingDay { get; set; }
	}
}