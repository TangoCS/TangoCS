using System;
using System.Collections.Generic;
using Tango.Html;

namespace Tango.UI.Controls
{
	public class DateListsOptions
	{
		public bool ShowTime { get; set; }
		public bool ShowDays { get; set; } = true;
		public int MinYear { get; set; }
		public int MaxYear { get; set; }
	}

	public static class DateListsExtensions
	{
		public static void DateLists(this IHtmlWriter w, string name, DateTime? value, DateListsOptions options = null)
		{
			if (options == null) options = new DateListsOptions();

			var monthItems = new List<SelectListItem>();
			var dayItems = new List<SelectListItem>();
			var yearItems = new List<SelectListItem>();
			var hourItems = new List<SelectListItem>();
			var minuteItems = new List<SelectListItem>();

			monthItems.Add(new SelectListItem { Value = "0", Text = "Месяц" });
			monthItems.Add(new SelectListItem { Value = "1", Text = options.ShowDays ? "января" : "январь" });
			monthItems.Add(new SelectListItem { Value = "2", Text = options.ShowDays ? "февраля" : "февраль" });
			monthItems.Add(new SelectListItem { Value = "3", Text = options.ShowDays ? "марта" : "март" });
			monthItems.Add(new SelectListItem { Value = "4", Text = options.ShowDays ? "апреля" : "апрель" });
			monthItems.Add(new SelectListItem { Value = "5", Text = options.ShowDays ? "мая" : "май" });
			monthItems.Add(new SelectListItem { Value = "6", Text = options.ShowDays ? "июня" : "июнь" });
			monthItems.Add(new SelectListItem { Value = "7", Text = options.ShowDays ? "июля" : "июль" });
			monthItems.Add(new SelectListItem { Value = "8", Text = options.ShowDays ? "августа" : "август" });
			monthItems.Add(new SelectListItem { Value = "9", Text = options.ShowDays ? "сентября" : "сентябрь" });
			monthItems.Add(new SelectListItem { Value = "10", Text = options.ShowDays ? "октября" : "октябрь" });
			monthItems.Add(new SelectListItem { Value = "11", Text = options.ShowDays ? "ноября" : "ноябрь" });
			monthItems.Add(new SelectListItem { Value = "12", Text = options.ShowDays ? "декабря" : "декабрь" });

			if (options.ShowDays)
			{
				dayItems.Add(new SelectListItem("День", "0"));
				for (int i = 1; i < 32; i++)
				{
					dayItems.Add(new SelectListItem(i, i));
				}
			}

			if (options.MinYear == 0) options.MinYear = 1900;
			if (options.MaxYear == 0) options.MaxYear = DateTime.Now.Year + 1;

			yearItems.Add(new SelectListItem("Год", "0"));
			for (int i = options.MaxYear; i >= options.MinYear; i--)
			{
				yearItems.Add(new SelectListItem(i, i));
			}

			if (options.ShowTime)
			{
				hourItems.Add(new SelectListItem("Час", "-1"));
				for (int i = 0; i < 24; i++)
					hourItems.Add(new SelectListItem(i.ToString("00"), i.ToString()));

				minuteItems.Add(new SelectListItem("Мин", "-1"));
				for (int i = 0; i < 60; i += 5)
					minuteItems.Add(new SelectListItem(i.ToString("00"), i.ToString()));
			}

			w.Div(a => a.Class("datelists"), () => {
				if (options.ShowDays)
				{
					w.DropDownList($"{name}_day", value?.Day.ToString(), dayItems);
					w.Write("&nbsp;");
				}

				w.DropDownList($"{name}_month", value?.Month.ToString(), monthItems);
				w.Write("&nbsp;");
				w.DropDownList($"{name}_year", value?.Year.ToString(), yearItems);

				if (options.ShowTime)
				{
					w.Write("&nbsp;");
					w.DropDownList($"{name}_hour", value?.Hour.ToString(), hourItems);
					w.Write("&nbsp;");
					w.DropDownList($"{name}_minute", value?.Minute.ToString(), minuteItems);
				}
			});
		}
	}
}
