﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Tango.Html;

namespace Tango.UI.Controls
{
	public interface IDefaultPeriodPickerRanges
	{
		Dictionary<string, DateTime[]> GetRanges(int minutesStep);
	}

	public class PeriodPicker : ViewComponent, IFieldValueProvider<PeriodValue>
	{
		[Inject]
		protected IDefaultPeriodPickerRanges DefaultPeriodPickerRanges { get; set; }

		public class PeriodPickerOptions
		{
			public Action<TagAttributes> Attributes { get; set; }
			public CalendarOptions FromCalendarOptions { get; set; } = new CalendarOptions { ShowButton = false };
			public DateLists.DateListsOptions FromTimeOptions { get; set; }
			public CalendarOptions ToCalendarOptions { get; set; } = new CalendarOptions { ShowButton = false };
			public DateLists.DateListsOptions ToTimeOptions { get; set; }
		}

		DateLists dPeriodFrom;
		DateLists dPeriodTo;

		public (string From, string To) ParmID => (ID + "_" + "dperiodfrom", ID + "_" + "dperiodto");
		
		public int MinYear { get; set; }
		public bool ShowDays { get; set; } = true;
		public bool ShowTime { get; set; } = false;
		public bool UseCalendar { get; set; } = true;

		public PeriodValue DefaultValue { get; set; }

		public int MinutesStep { get; set; } = 30;

		public Dictionary<string, DateTime[]> Ranges { get; set; }

		public event Action<ApiResponse> Change;
		public void OnChange(ApiResponse response) => Change?.Invoke(response);

		public string CalendarButtonHint { get; set; } = "Календарь";

		public override void OnInit()
		{
			dPeriodFrom = CreateControl<DateLists>(ID + "_" + (UseCalendar ? $"dperiodfromtime" : $"dperiodfrom"), c =>
			{
				c.ShowDays = ShowDays && !UseCalendar;
				c.ShowTime = ShowTime;
				c.TimeOnly = UseCalendar && ShowDays;
				c.MinutesStep = MinutesStep;
				if (DefaultValue != null) c.DefaultValue = DefaultValue.From;
			});
			dPeriodTo = CreateControl<DateLists>(ID + "_" + (UseCalendar ? $"dperiodtotime" : $"dperiodto"), c =>
			{
				c.ShowDays = ShowDays && !UseCalendar;
				c.ShowTime = ShowTime;
				c.TimeOnly = UseCalendar && ShowDays;
				c.MinutesStep = MinutesStep;
				if (DefaultValue != null) c.DefaultValue = DefaultValue.To;
			});
		}

		public void Render(LayoutWriter w, DateTime? from = null, DateTime? to = null, PeriodPickerOptions options = null)
		{
			dPeriodFrom.MinYear = MinYear;
			dPeriodTo.MinYear = MinYear;

			dPeriodFrom.MaxYear = DateTime.Today.Year;
			dPeriodTo.MaxYear = DateTime.Today.Year;

			if (from == null)
			{
				from = Context.GetDateTimeArg(ParmID.From);
				from = from?.AddHours(Context.GetIntArg(ParmID.From + "time_hour", 0)).AddMinutes(Context.GetIntArg(ParmID.From + "time_minute", 0));
			}
			if (from == null)
				from = DefaultValue?.From;

			if (to == null)
			{
				to = Context.GetDateTimeArg(ParmID.To);
				to = to?.AddHours(Context.GetIntArg(ParmID.To + "time_hour", 0)).AddMinutes(Context.GetIntArg(ParmID.To + "time_minute", 0));
			}
			if (to == null)
				to = DefaultValue?.To;
				
			if (options == null)
				options = new PeriodPickerOptions();
			options.FromCalendarOptions.ShowButton = false;
			options.ToCalendarOptions.ShowButton = false;
			//w.PushID(ID);
			w.Div(a => {
				a.Class("periodpicker").ID(ID).Set(options.Attributes);
				if (Change != null) a.DataEvent(OnChange).DataRef(ParentElement, ID).Data(DataCollection);
			}, () => {
				w.Div(() =>
				{
					if (UseCalendar && ShowDays) w.Calendar(ParmID.From, from, options.FromCalendarOptions);
					if (!UseCalendar || ShowTime)
						dPeriodFrom.Render(w, from, options.FromTimeOptions);
				});
				w.Div("&ndash;");
				w.Div(() =>
				{
					if (UseCalendar && ShowDays) w.Calendar(ParmID.To, to, options.ToCalendarOptions);
					if (!UseCalendar || ShowTime)
						dPeriodTo.Render(w, to, options.ToTimeOptions);
				});
				if (UseCalendar && ShowDays)
					w.Span(a => a.ID(ID + "_btn").Class("cal-openbtn").Title(CalendarButtonHint), () => w.Icon("calendar"));
			});

			if (UseCalendar && ShowDays)
			{
				if (Ranges == null)
					Ranges = DefaultPeriodPickerRanges?.GetRanges(MinutesStep);

				w.AddClientAction("daterangepickerproxy", "init", f => new
				{
					triggerid = f(ID + "_btn"),
					pickerparms = new
					{
						showDropdowns = true,
						timePicker = ShowTime,
						timePicker24Hour = ShowTime,
						timePickerIncrement = MinutesStep,
						ranges = Ranges,
						locale = new
						{
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
						startDate = from ?? DateTime.Today,
						endDate = to ?? DateTime.Today.AddDays(1).AddMinutes(-1)
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
					var from = Context.GetDateTimeArg(ParmID.From);
					var to = Context.GetDateTimeArg(ParmID.To);

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

	public class DateTimePicker : ViewComponent, IFieldValueProvider<DateTime?>
	{
		
		public class DateTimePickerOptions : CalendarOptions
		{
			public Action<TagAttributes> CalendarAttributes { get; set; }
			public Action<SelectTagAttributes> HourAttributes { get; set; }
			public Action<SelectTagAttributes> MinuteAttributes { get; set; }
		}
		
		DateTime Date => Context.GetDateTimeArg($"{ID}_dperiodfrom", DateTime.MinValue);
		int Hour => Context.GetArg($"{ID}_dperiodfromtime_hour", DefaultValue.Hour) ;
		int Minute => Context.GetArg($"{ID}_dperiodfromtime_minute", DefaultValue.Minute);

		public bool HasValue => Date > DateTime.MinValue && Hour >= 0 && Minute >= 0;
		DateTime? IFieldValueProvider<DateTime?>.Value => HasValue ? 
			new DateTime(Date.Year, Date.Month, Date.Day, Hour, Minute, 0) :
			DefaultValue > DateTime.MinValue ? DefaultValue : (DateTime?)null;

		DateLists dFrom;

		public int MinYear { get; set; }

		public DateTime DefaultValue { get; set; }
		public int MinMinute { get; set; } = 0;
		public int MaxMinute { get; set; } = 59;
		public int MinutesStep { get; set; } = 30;

		public override string ID
		{
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
			dFrom = CreateControl<DateLists>(ID + "_dperiodfromtime", c =>
			{
				c.ShowDays = false;
				c.ShowTime = true;
				c.TimeOnly = true;
				c.MinMinute = MinMinute;
				c.MaxMinute = MaxMinute;
				c.MinutesStep = MinutesStep;
				c.DefaultValue = DefaultValue;
			});
		}

		public void Render(LayoutWriter w, DateTime? value = null, DateTimePickerOptions dateTimePickerOptions = null)
		{
			dFrom.MinYear = MinYear;
			dFrom.MaxYear = DateTime.Today.Year;

			if (value == null)
				value = Context.GetDateTimeArg(ID + "_" + "dperiodfrom");

			if (dateTimePickerOptions == null)
				dateTimePickerOptions = new DateTimePickerOptions { ShowButton = false};

			DateLists.DateListsOptions options = new DateLists.DateListsOptions
			{
				Enabled = dateTimePickerOptions.Enabled,
				HourAttributes = dateTimePickerOptions.HourAttributes,
				MinuteAttributes = dateTimePickerOptions.MinuteAttributes
			};

			CalendarOptions calendarOptions = new CalendarOptions
			{
				Enabled = dateTimePickerOptions.Enabled,
				ShowButton = dateTimePickerOptions.ShowButton,
				ShowTime = dateTimePickerOptions.ShowTime,
				UseCalendarDays = dateTimePickerOptions.UseCalendarDays,
				Attributes = dateTimePickerOptions.Attributes,
				JsDisabledDates = dateTimePickerOptions.JsDisabledDates,
				JsDisabledPeriod = dateTimePickerOptions.JsDisabledPeriod
			};

			w.Div(a => a.Class("datetimepicker").ID(ID), () =>
			{
				w.Div(() =>
				{
					w.Calendar(ID + "_dperiodfrom", value ?? DefaultValue, calendarOptions);
					dFrom.Render(w, value ?? DefaultValue, options);
				});
				//if (dateTimePickerOptions.Enabled == EnabledState.Enabled)
				w.Span(a => a.ID(ID + "_btn").Class("cal-openbtn").Title("Календарь").Set(dateTimePickerOptions.CalendarAttributes), () => w.Icon("calendar"));
			});

			if (dateTimePickerOptions.Enabled == EnabledState.Enabled)
			{
				w.AddClientAction("Calendar", "setup", f => new {
					inputField = f(ID + "_dperiodfrom"),
					button = f(ID + "_btn"),
					showOthers = true,
					weekNumbers = false,
					showTime = dateTimePickerOptions.ShowTime,
					ifFormat = dateTimePickerOptions.ShowTime ? "%d.%m.%Y %H:%M" : "%d.%m.%Y",
					timeFormat = "24",
					dateStatusFunc = dateTimePickerOptions.UseCalendarDays ? "jscal_calendarDate" : null
				});
			}
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

		public override string ToString()
		{
			return From.Hour == 0 && To.Hour == 0 && From.Minute == 0 && To.Minute == 0 ? $"{From.DateToString()}-{To.DateToString()}" : $"{From.DateTimeToString()}-{To.DateTimeToString()}";
		}
	}

	public static class PeriodPickerExtensions
	{
		public static void Period(this LayoutWriter w, DateTime startDate, DateTime finishDate)
		{
			w.Div(a => {
				a.Class("periodpicker-readonly");
			}, () => {
				w.Div(() => {
					w.Span(startDate.ToString("dd.MM.yyyy"));
				});
				w.Div("&ndash;");
				w.Div(() => {
					w.Span(finishDate.ToString("dd.MM.yyyy"));
				});
				w.Span(a => a.Class("cal-openbtn"), () => w.Icon("calendar"));
			});
		}
	}
}
