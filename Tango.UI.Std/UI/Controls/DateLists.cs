using System;
using System.Collections.Generic;
using Tango.Html;

namespace Tango.UI.Controls
{
	public class DateLists : ViewComponent, IFieldValueProvider<DateTime?>, IFieldValueProvider<DateTime>
	{
		public DateTime DefaultValue { get; set; }
		public bool ShowTime { get; set; }
		public bool ShowDays { get; set; } = true;
		public bool TimeOnly { get; set; } = false;
		public int MinYear { get; set; }
		public int MaxYear { get; set; }
		public int MinutesStep { get; set; } = 5;

		int Day => ShowDays ? Context.GetArg($"{ID}_day", DefaultValue.Day) : 1;
		int Month => Context.GetArg($"{ID}_month", DefaultValue.Month);
		int Year => Context.GetArg($"{ID}_year", DefaultValue.Year);
		int Hour => ShowTime ? Context.GetArg($"{ID}_hour", DefaultValue.Hour) : 0;
		int Minute => ShowTime ? Context.GetArg($"{ID}_minute", DefaultValue.Minute) : 0;

		public bool HasValue => Day > 0 && Month > 0 && Year > 0 && Hour >= 0 && Minute >= 0 && Day <= DateTime.DaysInMonth(Year, Month);

		public DateTime? Value => HasValue ? new DateTime(Year, Month, Day, Hour, Minute, 0) : (DateTime?)null;

		DateTime IFieldValueProvider<DateTime>.Value => Value ?? DateTime.MinValue;

		public void Render(HtmlWriter w, DateTime? value = null, Action<SelectTagAttributes> attributes = null)
		{
			var monthItems = new List<SelectListItem>();
			var dayItems = new List<SelectListItem>();
			var yearItems = new List<SelectListItem>();
			var hourItems = new List<SelectListItem>();
			var minuteItems = new List<SelectListItem>();

			if (!TimeOnly)
			{
				if (ShowDays)
				{
					dayItems.Add(new SelectListItem("День", "0"));
					for (int i = 1; i < 32; i++)
					{
						dayItems.Add(new SelectListItem(i, i));
					}
				}
			
				if (MinYear == 0) MinYear = 1900;
				if (MaxYear == 0) MaxYear = DateTime.Now.Year + 1;

				monthItems.Add(new SelectListItem { Value = "0", Text = "Месяц" });
				monthItems.Add(new SelectListItem { Value = "1", Text = ShowDays ? "января" : "январь" });
				monthItems.Add(new SelectListItem { Value = "2", Text = ShowDays ? "февраля" : "февраль" });
				monthItems.Add(new SelectListItem { Value = "3", Text = ShowDays ? "марта" : "март" });
				monthItems.Add(new SelectListItem { Value = "4", Text = ShowDays ? "апреля" : "апрель" });
				monthItems.Add(new SelectListItem { Value = "5", Text = ShowDays ? "мая" : "май" });
				monthItems.Add(new SelectListItem { Value = "6", Text = ShowDays ? "июня" : "июнь" });
				monthItems.Add(new SelectListItem { Value = "7", Text = ShowDays ? "июля" : "июль" });
				monthItems.Add(new SelectListItem { Value = "8", Text = ShowDays ? "августа" : "август" });
				monthItems.Add(new SelectListItem { Value = "9", Text = ShowDays ? "сентября" : "сентябрь" });
				monthItems.Add(new SelectListItem { Value = "10", Text = ShowDays ? "октября" : "октябрь" });
				monthItems.Add(new SelectListItem { Value = "11", Text = ShowDays ? "ноября" : "ноябрь" });
				monthItems.Add(new SelectListItem { Value = "12", Text = ShowDays ? "декабря" : "декабрь" });

				yearItems.Add(new SelectListItem("Год", "0"));
				for (int i = MaxYear; i >= MinYear; i--)
				{
					yearItems.Add(new SelectListItem(i, i));
				}
			}

			if (ShowTime || TimeOnly)
			{
				hourItems.Add(new SelectListItem("Час", "-1"));
				for (int i = 0; i < 24; i++)
					hourItems.Add(new SelectListItem(i.ToString("00"), i.ToString()));

				minuteItems.Add(new SelectListItem("Мин", "-1"));
				for (int i = 0; i < 60; i += MinutesStep)
					minuteItems.Add(new SelectListItem(i.ToString("00"), i.ToString()));
			}

			w.Div(a => a.Class("datelists").ID(ID), () => {
				if (!TimeOnly)
				{
					if (ShowDays)
					{
						w.DropDownList($"{ID}_day", value?.Day.ToString(), dayItems, a => a.Class("days"));
						w.Write("&nbsp;");
					}

					w.DropDownList($"{ID}_month", value?.Month.ToString(), monthItems, a => a.Class("months").Set(attributes));
					w.Write("&nbsp;");
					w.DropDownList($"{ID}_year", value?.Year.ToString(), yearItems, a => a.Class("years").Set(attributes));
				}

				if (ShowTime || TimeOnly)
				{
					w.Write("&nbsp;");
					w.DropDownList($"{ID}_hour", value?.Hour.ToString(), hourItems, a => a.Class("hours"));
					w.Write(":");
					w.DropDownList($"{ID}_minute", value?.Minute.ToString(), minuteItems, a => a.Class("minutes"));
				}
			});
		}
	}
}
