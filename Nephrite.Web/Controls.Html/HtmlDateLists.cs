using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Text;

namespace Nephrite.Web.Controls
{
	public class HtmlDateLists
	{
		public bool ShowTime { get; set; }
		public bool ShowDays { get; set; }
		public int MinYear { get; set; }
		public int MaxYear { get; set; }
		public string Name { get; set; }
		public string ID { get; set; }
		public bool Enabled { get; set; }

		public HtmlDateLists()
		{
			Enabled = true;
		}

		bool manualSet = false;
		DateTime? date;
		public DateTime? Date
		{
			get
			{
				if (manualSet)
					return date;
				try
				{
					var f = HttpContext.Current.Request.Form;
					int y = f[Name + "_y"].ToInt32(0);
					int m = f[Name + "_m"].ToInt32(0);
					int d = ShowDays ? f[Name + "_d"].ToInt32(0) : 1;
					if (!ShowTime && y > 1900 && m > 0 && m <= 12 && d > 0 && d <= 31)
						return new DateTime(y, m, d);
					int h = f[Name + "_h"].ToInt32(0);
					int mi = f[Name + "_i"].ToInt32(0);
					if (ShowTime && y > 1900 && m > 0 && m <= 12 && d > 0 && d <= 31 && h >= 0 && h <= 23 && mi >= 0 && mi <= 59)
						return new DateTime(y, m, d, h, mi, 0);
				}
				catch
				{
					return null;
				}
				return null;
			}
			set
			{
				date = value;
				manualSet = true;
			}
		}

		public override string ToString()
		{
			StringBuilder res = new StringBuilder();

			if (ShowDays)
			{
				ListItemCollection days = new ListItemCollection();
				days.Add(new ListItem("День", "0"));
				for (int i = 1; i < 32; i++)
				{
					days.Add(i.ToString());
				}

				DateTime? value = Date;

				res.Append((new HtmlDropDownList
				{
					Name = Name + "_d",
					Enabled = Enabled,
					SelectedValue = Date.HasValue ? Date.Value.Day.ToString() : "0",
					DataTextField = "Text",
					DataValueField = "Value",
					DataSource = days
				}).ToString());
			}

			ListItemCollection months = new ListItemCollection();

			months.Add(new ListItem { Value = "0", Text = "Месяц" });
			months.Add(new ListItem { Value = "1", Text = ShowDays ? "января" : "январь" });
			months.Add(new ListItem { Value = "2", Text = ShowDays ? "февраля" : "февраль" });
			months.Add(new ListItem { Value = "3", Text = ShowDays ? "марта" : "март" });
			months.Add(new ListItem { Value = "4", Text = ShowDays ? "апреля" : "апрель" });
			months.Add(new ListItem { Value = "5", Text = ShowDays ? "мая" : "май" });
			months.Add(new ListItem { Value = "6", Text = ShowDays ? "июня" : "июнь" });
			months.Add(new ListItem { Value = "7", Text = ShowDays ? "июля" : "июль" });
			months.Add(new ListItem { Value = "8", Text = ShowDays ? "августа" : "август" });
			months.Add(new ListItem { Value = "9", Text = ShowDays ? "сентября" : "сентябрь" });
			months.Add(new ListItem { Value = "10", Text = ShowDays ? "октября" : "октябрь" });
			months.Add(new ListItem { Value = "11", Text = ShowDays ? "ноября" : "ноябрь" });
			months.Add(new ListItem { Value = "12", Text = ShowDays ? "декабря" : "декабрь" });

			res.Append("&nbsp;");
			res.Append((new HtmlDropDownList
			{
				Name = Name + "_m",
				Enabled = Enabled,
				SelectedValue = Date.HasValue ? Date.Value.Month.ToString() : "0",
				DataTextField = "Text",
				DataValueField = "Value",
				DataSource = months
			}).ToString());


			if (MinYear == 0) MinYear = 1900;
			if (MaxYear == 0) MaxYear = DateTime.Now.Year + 1;
			ListItemCollection years = new ListItemCollection();
			years.Add(new ListItem("Год", "0"));
			for (int i = MaxYear; i >= MinYear; i--)
			{
				years.Add(i.ToString());
			}
			res.Append("&nbsp;");
			res.Append((new HtmlDropDownList
			{
				Name = Name + "_y",
				Enabled = Enabled,
				SelectedValue = Date.HasValue ? Date.Value.Year.ToString() : "0",
				DataTextField = "Text",
				DataValueField = "Value",
				DataSource = years
			}).ToString());

			if (ShowTime)
			{
				ListItemCollection hours = new ListItemCollection();
				hours.Add(new ListItem("Час", "-1"));
				for (int i = 0; i < 24; i++)
					hours.Add(new ListItem(i.ToString("00"), i.ToString()));
				res.Append("&nbsp;");
				res.Append((new HtmlDropDownList
				{
					Name = Name + "_h",
					Enabled = Enabled,
					SelectedValue = Date.HasValue ? Date.Value.Hour.ToString() : "-1",
					DataTextField = "Text",
					DataValueField = "Value",
					DataSource = hours
				}).ToString());

				ListItemCollection minutes = new ListItemCollection();
				minutes.Add(new ListItem("Мин", "-1"));
				for (int i = 0; i < 60; i += 5)
					minutes.Add(new ListItem(i.ToString("00"), i.ToString()));
				res.Append("&nbsp;");
				res.Append((new HtmlDropDownList
				{
					Name = Name + "_i",
					Enabled = Enabled,
					SelectedValue = Date.HasValue ? Date.Value.Minute.ToString() : "-1",
					DataTextField = "Text",
					DataValueField = "Value",
					DataSource = minutes
				}).ToString());
			}

			return res.ToString();
		}
	}
}