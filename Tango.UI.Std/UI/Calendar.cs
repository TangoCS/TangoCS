using System;
using Tango.Html;

namespace Tango.UI
{
	public enum EnabledState { Enabled, ReadOnly, Disabled }

	public class CalendarOptions
	{
		public EnabledState Enabled { get; set; } = EnabledState.Enabled;
		public bool ShowTime { get; set; } = false;
		public bool UseCalendarDays { get; set; } = false;
		public Action<InputTagAttributes> Attributes { get; set; }
	}

	public static class CalendarExtension
	{
		public static void Calendar(this LayoutWriter w, InputName name, DateTime? value, CalendarOptions options)
		{
			if (options == null) options = new CalendarOptions();
			string basePath = GlobalSettings.JSPath + "calendar/";

			//c.Page.RegisterScript("calendar-setup", basePath + "calendar-setup_stripped.js");
			if (value == DateTime.MinValue) value = null;

			w.TextBox(name, options.ShowTime ? value.DateTimeToString() : value.DateToString(), a =>
				a.Data("format", "dd.MM.yyyy").Placeholder("ДД.ММ.ГГГГ").Style("width:" + (options.ShowTime ? "130px" : "100px"))
				.Data("calendar", "")
				.Data("showtime", options.ShowTime).Data("usecalendardays", options.UseCalendarDays)
				.Disabled(options.Enabled == EnabledState.Disabled)
				.Readonly(options.Enabled == EnabledState.ReadOnly)
				.Set(options.Attributes)
			);
			if (options.Enabled == EnabledState.Enabled)
			{
				w.Span(a => a.ID("btn" + name.ID).Class("cal-openbtn").Title("Календарь"), () => w.Icon("calendar"));

				//w.Includes.Add("calendar/calendar_stripped.js");
				//w.Includes.Add("calendar/lang/calendar-ru.js");
				//w.Includes.Add("calendar/calendarcontrol.js");

				w.AddClientAction("Calendar", "setup", f => new {
					inputField = f(name.ID),
					button = f("btn" + name.ID),
					showOthers = true,
					weekNumbers = false,
					showTime = options.ShowTime,
					ifFormat = options.ShowTime ? "%d.%m.%Y %H:%M" : "%d.%m.%Y",
					timeFormat = "24",
					dateStatusFunc = options.UseCalendarDays ? "jscal_calendarDate" : null
				});
				w.AddClientAction("calendarcontrol", "init", f => f(name.ID));
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

        public static void Calendar(this LayoutWriter w, InputName name, DateTime? value = null, EnabledState enabled = EnabledState.Enabled, bool showTime = false, bool useCalendarDaysInJSCalendar = false)
        {
            w.Calendar(name, value,  new CalendarOptions { Enabled = enabled, ShowTime = showTime, UseCalendarDays = useCalendarDaysInJSCalendar });
        }
    }

	
}