using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Nephrite.Web.Controls
{
	public class DateLists : System.Web.UI.UserControl
	{
		public DateLists()
		{
			ShowTime = false;
			ShowDays = true;
		}

		public bool AutoPostBack { get; set; }
		public bool ShowTime { get; set; }
		public bool ShowDays { get; set; }
		public int MinYear { get; set; }
		public int MaxYear { get; set; }

		DropDownList Day = new DropDownList();
		DropDownList Month = new DropDownList();
		DropDownList Year = new DropDownList();
		DropDownList Hour = new DropDownList();
		DropDownList Minute = new DropDownList();

		protected override void CreateChildControls()
		{
			Controls.Clear();

			Day.EnableViewState = false;
			Month.EnableViewState = false;
			Year.EnableViewState = false;
			Hour.EnableViewState = false;
			Minute.EnableViewState = false;
			if (ShowDays)
			{
				Controls.Add(Day);
				Controls.Add(new LiteralControl("&nbsp;"));
				Day.AutoPostBack = AutoPostBack;
				Day.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);
			}
			Controls.Add(Month);
			Month.AutoPostBack = AutoPostBack;
			Month.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);
			Controls.Add(new LiteralControl("&nbsp;"));
			Controls.Add(Year);
			Year.AutoPostBack = AutoPostBack;
			Year.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);
			if (ShowTime)
			{
				Controls.Add(new LiteralControl("&nbsp;"));
				Controls.Add(Hour);
				Hour.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);
				Hour.AutoPostBack = AutoPostBack;
				Controls.Add(new LiteralControl("&nbsp;"));
				Controls.Add(Minute);
				Minute.AutoPostBack = AutoPostBack;
				Minute.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);
			}

			Fill();
		}

		public event EventHandler Change;
		protected internal virtual void OnChange(EventArgs e)
		{
			if (Change != null)
				Change(this, e);
		}

		void SelectedIndexChanged(object sender, EventArgs e)
		{
			OnChange(e);
		}

		void Fill()
		{
			if (Month.Items.Count == 0)
			{
				Month.Items.Add(new ListItem { Value = "0", Text = "Месяц" });
				Month.Items.Add(new ListItem { Value = "1", Text = ShowDays ? "января" : "январь" });
				Month.Items.Add(new ListItem { Value = "2", Text = ShowDays ? "февраля" : "февраль" });
				Month.Items.Add(new ListItem { Value = "3", Text = ShowDays ? "марта" : "март" });
				Month.Items.Add(new ListItem { Value = "4", Text = ShowDays ? "апреля" : "апрель" });
				Month.Items.Add(new ListItem { Value = "5", Text = ShowDays ? "мая" : "май" });
				Month.Items.Add(new ListItem { Value = "6", Text = ShowDays ? "июня" : "июнь" });
				Month.Items.Add(new ListItem { Value = "7", Text = ShowDays ? "июля" : "июль" });
				Month.Items.Add(new ListItem { Value = "8", Text = ShowDays ? "августа" : "август" });
				Month.Items.Add(new ListItem { Value = "9", Text = ShowDays ? "сентября" : "сентябрь" });
				Month.Items.Add(new ListItem { Value = "10", Text = ShowDays ? "октября" : "октябрь" });
				Month.Items.Add(new ListItem { Value = "11", Text = ShowDays ? "ноября" : "ноябрь" });
				Month.Items.Add(new ListItem { Value = "12", Text = ShowDays ? "декабря" : "декабрь" });
			}

			if (Day.Items.Count == 0 && ShowDays)
			{
				Day.Items.Add(new ListItem("День", "0"));
				for (int i = 1; i < 32; i++)
				{
					Day.Items.Add(i.ToString());
				}
			}


			if (MinYear == 0) MinYear = 1900;
			if (MaxYear == 0) MaxYear = DateTime.Now.Year + 1;
			if (Year.Items.Count == 0)
			{
				Year.Items.Add(new ListItem("Год", "0"));
				for (int i = MaxYear; i >= MinYear; i--)
				{
					Year.Items.Add(i.ToString());
				}
			}

			if (ShowTime)
			{
				if (Hour.Items.Count == 0)
				{
					Hour.Items.Add(new ListItem("Час", "-1"));
					for (int i = 0; i < 24; i++)
						Hour.Items.Add(new ListItem(i.ToString("00"), i.ToString()));

					Minute.Items.Add(new ListItem("Мин", "-1"));
					for (int i = 0; i < 60; i += 5)
						Minute.Items.Add(new ListItem(i.ToString("00"), i.ToString()));
				}
			}
			else
			{
				Hour.Visible = false;
				Minute.Visible = false;
			}
		}

		public DateTime? Date
		{
			get
			{
				EnsureChildControls();
				if (HasValue)
				{
					DateTime dt = new DateTime(
						Year.SelectedValue.ToInt32(1900),
						Month.SelectedValue.ToInt32(1),
						ShowDays ? Day.SelectedValue.ToInt32(1) : 1
						);
					if (ShowTime)
					{
						if (Hour.SelectedValue.ToInt32(0) > 0)
						{
							dt = dt.AddHours(Hour.SelectedValue.ToInt32(0));
							if (Minute.SelectedValue.ToInt32(0) > 0)
								dt = dt.AddMinutes(Minute.SelectedValue.ToInt32(0));
						}
					}
					return dt;
				}
				else
					return null;
			}
			set
			{
				EnsureChildControls();
				Fill();
				if (value.HasValue)
				{
					DateTime dt = value.Value;
					if (ShowDays) Day.SelectedValue = dt.Day.ToString();
					Month.SelectedValue = dt.Month.ToString();
					Year.SelectedValue = dt.Year.ToString();
					if (ShowTime)
					{
						Hour.SelectedValue = dt.Hour.ToString();
						Minute.SelectedValue = dt.Minute.ToString();
					}
				}
				else
				{
					if (ShowDays) Day.SelectedValue = "";
					Month.SelectedValue = "";
					Year.SelectedValue = "";
					if (ShowTime) Hour.SelectedValue = "-1";
					if (ShowTime) Minute.SelectedValue = "-1";
				}
			}
		}

		public bool HasValue
		{
			get
			{
				int d = ShowDays ? Day.SelectedValue.ToInt32(0) : 1;
				int m = Month.SelectedValue.ToInt32(0);
				int y = Year.SelectedValue.ToInt32(0);
				DateTime dt;
				return (d > 0 && m > 0 && y > 0 && DateTime.TryParse(y.ToString() + "/" + m.ToString() + "/" + d.ToString(), out dt));

			}
		}

		public bool Enabled
		{
			set
			{
				Day.Enabled = value;
				Month.Enabled = value;
				Year.Enabled = value;
				Hour.Enabled = value;
				Minute.Enabled = value;
			}
		}
	}
}