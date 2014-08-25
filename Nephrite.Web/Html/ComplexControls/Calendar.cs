using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using Nephrite.Web.CalendarDays;

namespace Nephrite.Web.Html
{
	public static class CalendarExtension
	{
		public static void Calendar(this HtmlControl c, string name, DateTime? value = null, bool enabled = true, bool showTime = false)
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
}