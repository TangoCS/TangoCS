using System;
using System.Collections.Generic;
using Tango.Html;

namespace Tango.UI.Controls
{
	public class DateLists : ViewComponent
	{
		public bool ShowTime { get; set; }
		public bool ShowDays { get; set; } = true;
		public int MinYear { get; set; }
		public int MaxYear { get; set; }

		int Day => GetPosted<int>($"{ClientID}_day", 1);
		int Month => GetPosted<int>($"{ClientID}_month", 1);
		int Year => GetPosted<int>($"{ClientID}_year", 1900);
		int Hour => GetPosted<int>($"{ClientID}_hour");
		int Minute => GetPosted<int>($"{ClientID}_minute");

		public bool HasValue
		{
			get
			{
				int d = ShowDays ? Day : 1;
				int m = Month;
				int y = Year;
				DateTime dt;
				return (d > 0 && m > 0 && y > 0 && DateTime.TryParse(y.ToString() + "/" + m.ToString() + "/" + d.ToString(), out dt));

			}
		}

		public DateTime? Date
		{
			get
			{
				if (HasValue)
				{
					DateTime dt = new DateTime(Year, Month, ShowDays ? Day : 1);
					if (ShowTime)
					{
						var h = Hour;					
						if (h > 0)
						{
							dt = dt.AddHours(h);
							var m = Minute;
							if (m > 0)
								dt = dt.AddMinutes(m);
						}
					}
					return dt;
				}
				else
					return null;
			}
		}

		public void Render(IHtmlWriter w, DateTime? value = null)
		{
			var monthItems = new List<SelectListItem>();
			var dayItems = new List<SelectListItem>();
			var yearItems = new List<SelectListItem>();
			var hourItems = new List<SelectListItem>();
			var minuteItems = new List<SelectListItem>();

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

			yearItems.Add(new SelectListItem("Год", "0"));
			for (int i = MaxYear; i >= MinYear; i--)
			{
				yearItems.Add(new SelectListItem(i, i));
			}

			if (ShowTime)
			{
				hourItems.Add(new SelectListItem("Час", "-1"));
				for (int i = 0; i < 24; i++)
					hourItems.Add(new SelectListItem(i.ToString("00"), i.ToString()));

				minuteItems.Add(new SelectListItem("Мин", "-1"));
				for (int i = 0; i < 60; i += 5)
					minuteItems.Add(new SelectListItem(i.ToString("00"), i.ToString()));
			}

			w.Div(a => a.Class("datelists"), () => {
				if (ShowDays)
				{
					w.DropDownList($"{ClientID}_day", value?.Day.ToString(), dayItems);
					w.Write("&nbsp;");
				}

				w.DropDownList($"{ClientID}_month", value?.Month.ToString(), monthItems);
				w.Write("&nbsp;");
				w.DropDownList($"{ClientID}_year", value?.Year.ToString(), yearItems);

				if (ShowTime)
				{
					w.Write("&nbsp;");
					w.DropDownList($"{ClientID}_hour", value?.Hour.ToString(), hourItems);
					w.Write("&nbsp;");
					w.DropDownList($"{ClientID}_minute", value?.Minute.ToString(), minuteItems);
				}
			});
		}
	}
}
