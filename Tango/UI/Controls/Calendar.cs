using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Tango.Data;
using Tango.Html;

namespace Tango.UI.Controls
{
	public static class CalendarExtension
	{
		public static void Calendar(this LayoutWriter w, string name, DateTime? value = null, bool enabled = true, bool showTime = false)
		{
			string basePath = GlobalSettings.JSPath + "Calendar/";

			//c.Page.RegisterScript("calendar-setup", basePath + "calendar-setup_stripped.js");
			if (value == DateTime.MinValue) value = null;

			w.TextBox(name, showTime ? value.DateTimeToString() : value.DateToString(), a =>
				a.ID(name).Placeholder("ДД.ММ.ГГГГ").Style("width:" + (showTime ? "130px" : "100px"))
				.OnKeyPress("return jscal_calendarHelper(event)").Disabled(!enabled)
			);
			if (enabled)
			{
				w.Img(a => a.ID("btn" + name).Title("Календарь").Src(basePath + "img.gif"));

				//w.Includes.Add("calendar/calendar_stripped.js");
				//w.Includes.Add("calendar/lang/calendar-ru.js");	

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

	
}