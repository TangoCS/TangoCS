using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Tango.Html;

namespace Tango.UI.Controls
{
	public class PeriodPicker : ViewComponent, IFieldValueProvider<PeriodValue>
	{
		DateLists dPeriodFrom;
		DateLists dPeriodTo;

		public (string From, string To) ParmName => (ID + "_" + "dperiodfrom", ID + "_" + "dperiodto");

		public int MinYear { get; set; }
		public bool ShowDays { get; set; } = true;
		public bool ShowTime { get; set; } = false;
		public bool UseCalendar { get; set; } = true;

		public PeriodValue DefaultValue { get; set; }

		public string JSOnSelectCallback { get; set; }

		public override void OnInit()
		{
			dPeriodFrom = CreateControl<DateLists>(ID + "_" + (UseCalendar ? $"dperiodfromtime" : $"dperiodfrom"), c => {
				c.ShowDays = ShowDays && !UseCalendar;
				c.ShowTime = ShowTime;
				c.TimeOnly = UseCalendar && ShowDays;
				c.MinutesStep = 30;
				if (DefaultValue != null) c.DefaultValue = DefaultValue.From;
			});
			dPeriodTo = CreateControl<DateLists>(ID + "_" + (UseCalendar ? $"dperiodtotime" : $"dperiodto"), c => {
				c.ShowDays = ShowDays && !UseCalendar;
				c.ShowTime = ShowTime;
				c.TimeOnly = UseCalendar && ShowDays;
				c.MinutesStep = 30;
				if (DefaultValue != null) c.DefaultValue = DefaultValue.To;
			});
		}

		public void Render(LayoutWriter w, DateTime? from = null, DateTime? to = null, Action<InputTagAttributes> attributes = null)
		{
			dPeriodFrom.MinYear = MinYear;
			dPeriodTo.MinYear = MinYear;

			dPeriodFrom.MaxYear = DateTime.Today.Year;
			dPeriodTo.MaxYear = DateTime.Today.Year;

            if (from == null)
                from = Context.GetDateTimeArg(ParmName.From);
			if (from == null)
				from = DefaultValue?.From;

			if (to == null)
                to = Context.GetDateTimeArg(ParmName.To);
			if (to == null)
				to = DefaultValue?.To;

			var options = new CalendarOptions { ShowButton = false, Attributes = a => { attributes?.Invoke(a); a.OnInput(JSOnSelectCallback); } };
			var dlOptions = new DateLists.DateListsOptions { HourAttributes = a => a.OnChange(JSOnSelectCallback), MinuteAttributes = a => a.OnChange(JSOnSelectCallback) };
            //w.PushID(ID);
			w.Div(a => a.Class("periodpicker").ID(ID), () => {
				w.Div(() => {
					if (UseCalendar && ShowDays) w.Calendar(ParmName.From, from, options);
					if (!UseCalendar || ShowTime)
						dPeriodFrom.Render(w, from, dlOptions);
				});
				w.Div("&ndash;");
				w.Div(() => {
					if (UseCalendar && ShowDays) w.Calendar(ParmName.To, to, options);
					if (!UseCalendar || ShowTime)
						dPeriodTo.Render(w, to, dlOptions);
				});
				if (UseCalendar && ShowDays)
					w.Span(a => a.ID(ID + "_btn").Class("cal-openbtn").Title("Календарь"), () => w.Icon("calendar"));
			});

			if (UseCalendar && ShowDays)
			{
				w.AddClientAction("daterangepickerproxy", "init", f => new {
					triggerid = f(ID + "_btn"),
					onselectcallback = JSOnSelectCallback,
					pickerparms = new {
						showDropdowns = true,
						timePicker = ShowTime,
						timePicker24Hour = ShowTime,
						timePickerIncrement = 30,
						ranges = new Dictionary<string, DateTime[]> {
							["Сегодня"] = new DateTime[] { new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day), new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day).AddDays(1).AddMinutes(-30) },
							["Вчера"] = new DateTime[] { new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day).AddDays(-1), new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day).AddMinutes(-30) },
							["Текущий месяц"] = new DateTime[] { new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddMinutes(-30) },
							["Предыдущий месяц"] = new DateTime[] { new DateTime(DateTime.Today.AddMonths(-1).Year, DateTime.Today.AddMonths(-1).Month, 1), new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMinutes(-30) }
						},
						locale = new {
							format = ShowTime ? "DD.MM.YYYY HH:mm" : "DD.MM.YYYY",
							separator = " - ",
							applyLabel = "Применить",
							cancelLabel = "Отмена",
							fromLabel = "С",
							toLabel = "По",
							customRangeLabel = "Пользовательский",
							weekLabel = "W",
							daysOfWeek = new string[] { "Вс", "Пн", "Вт", "Ср", "Чт", "Пт", "Сб" },
							monthNames = new string[] { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" },
							firstDay = 1
						},
						showCustomRangeLabel = false,
						alwaysShowCalendars = true,
						startDate = from,
						endDate = to
					}
				});
			}

			//w.PopID();
		}

		public PeriodValue Value
		{
			get
			{
				if (UseCalendar && ShowDays)
				{
					var from = Context.GetDateTimeArg(ParmName.From);
					var to = Context.GetDateTimeArg(ParmName.To);
					var fromtime = dPeriodFrom.Value?.TimeOfDay;
					var totime = dPeriodTo.Value?.TimeOfDay;

					if (from == null || to == null) return null;

					if (ShowTime)
					{
						if (fromtime != null) from = from.Value.Add(fromtime.Value);
						if (totime != null) to = to.Value.Add(totime.Value);
					}
					else
					{						
						if (DefaultValue != null) from = from.Value.AddHours(DefaultValue.From.Hour).AddMinutes(DefaultValue.From.Minute);
						if (DefaultValue != null) to = to.Value.AddHours(DefaultValue.To.Hour).AddMinutes(DefaultValue.To.Minute);
					}

					return new PeriodValue(from.Value, to.Value);
				}
				else
				{
					var from = dPeriodFrom.Value;
					var to = dPeriodTo.Value;
					if (from.HasValue && to.HasValue)
						return new PeriodValue(from.Value, to.Value);
				}
				return null;
			}
		}
	}

	public class DateTimePicker : ViewComponent, IFieldValueProvider<DateTime>
	{
		DateLists dFrom;

		public int MinYear { get; set; }

		public DateTime DefaultValue { get; set; }

		public override string ID { 
			get => base.ID; 
			set 
			{ 
				if (dFrom != null)
					dFrom.ID = value + "_dperiodfromtime"; 
				base.ID = value; 
			} 
		}

		public override void OnInit()
		{
			dFrom = CreateControl<DateLists>(ID + "_dperiodfromtime", c => {
				c.ShowDays = false;
				c.ShowTime = true;
				c.TimeOnly = true;
				c.MinutesStep = 30;
				c.DefaultValue = DefaultValue;
			});
		}

		public void Render(LayoutWriter w, DateTime? from = null, DateTime? to = null, Action<InputTagAttributes> attributes = null)
		{
			dFrom.MinYear = MinYear;
			dFrom.MaxYear = DateTime.Today.Year;

			if (from == null)
				from = Context.GetDateTimeArg(ID + "_" + "dperiodfrom");

			var options = new CalendarOptions { ShowButton = false, Attributes = attributes };

			w.Div(a => a.Class("datetimepicker").ID(ID), () => {
				w.Div(() => {
					w.Calendar(ID + "_dperiodfrom", from ?? DefaultValue, options);
					dFrom.Render(w, from ?? DefaultValue);
				});
				w.Span(a => a.ID(ID + "_btn").Class("cal-openbtn").Title("Календарь"), () => w.Icon("calendar"));
			});

			w.AddClientAction("Calendar", "setup", f => new {
				inputField = f(ID + "_dperiodfrom"),
				button = f(ID + "_btn"),
				showOthers = true,
				weekNumbers = false,
				showTime = options.ShowTime,
				ifFormat = options.ShowTime ? "%d.%m.%Y %H:%M" : "%d.%m.%Y",
				timeFormat = "24",
				dateStatusFunc = options.UseCalendarDays ? "jscal_calendarDate" : null
			});
		}

		public DateTime Value
		{
			get
			{
				var from = Context.GetDateTimeArg(ID + "_" + "dperiodfrom");
				var fromtime = dFrom.Value?.TimeOfDay;

				if (from == null) return DefaultValue;
				if (fromtime != null) from = from.Value.Add(fromtime.Value);

				return from.Value;
			}
		}
	}

	public class PeriodValue
	{
		public DateTime From { get; }
		public DateTime To { get; }

		public PeriodValue(DateTime from, DateTime to)
		{
			From = from;
			To = to;
		}
	}
}
