using System;
using Tango.Html;

namespace Tango.UI.Controls
{
	public static class CalendarExtension
	{
		public static void Calendar(this LayoutWriter w, InputName name, DateTime? value = null, bool enabled = true, bool showTime = false, bool useCalendarDaysInJSCalendar = false)
		{
			string basePath = GlobalSettings.JSPath + "calendar/";

			//c.Page.RegisterScript("calendar-setup", basePath + "calendar-setup_stripped.js");
			if (value == DateTime.MinValue) value = null;

			w.TextBox(name, showTime ? value.DateTimeToString() : value.DateToString(), a =>
				a.Data("format", "dd.MM.yyyy").Placeholder("ДД.ММ.ГГГГ").Style("width:" + (showTime ? "130px" : "100px"))
				.Data("calendar", "")
				.Data("showtime", showTime).Data("usecalendardays", useCalendarDaysInJSCalendar)
				.OnKeyPress("return calendarcontrol.keypress(event)").Disabled(!enabled)
			);
			if (enabled)
			{
				w.Img(a => a.ID("btn" + name.ID).Title("Календарь").Src(basePath + "img.gif"));

				w.Includes.Add("calendar/calendar_stripped.js");
				w.Includes.Add("calendar/lang/calendar-ru.js");
				w.Includes.Add("calendar/calendarcontrol.js");

				w.AddClientAction("Calendar", "setup", new {
					inputField = w.GetID(name.ID),
					button = w.GetID("btn" + name.ID),
					showOthers = true,
					weekNumbers = false,
					showTime = showTime,
					ifFormat = showTime ? "%d.%m.%Y %H:%M" : "%d.%m.%Y",
					timeFormat = "24",
					dateStatusFunc = useCalendarDaysInJSCalendar ? "jscal_calendarDate" : null
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

	
}