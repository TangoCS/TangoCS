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
                from = Context.GetDateTimeArg(ID + "_" + "dperiodfrom");
            if (to == null)
                to = Context.GetDateTimeArg(ID + "_" + "dperiodto");

			var options = new CalendarOptions { ShowButton = false,Attributes = attributes };

            //w.PushID(ID);
			w.Div(a => a.Class("periodpicker").ID(ID), () => {
				w.Div(() => {
					if (UseCalendar && ShowDays) w.Calendar(ID + "_" + "dperiodfrom", from ?? DefaultValue?.From, options);
					if (!UseCalendar || ShowTime)
						dPeriodFrom.Render(w, from ?? DefaultValue?.From);
				});
				w.Div("&ndash;");
				w.Div(() => {
					if (UseCalendar && ShowDays) w.Calendar(ID + "_" + "dperiodto", to ?? DefaultValue?.To, options);
					if (!UseCalendar || ShowTime)
						dPeriodTo.Render(w, to ?? DefaultValue?.To);
				});
				if (UseCalendar && ShowDays)
					w.Span(a => a.ID(ID + "_" + "btn" + ID).Class("cal-openbtn").Title("Календарь"), () => w.Icon("calendar"));
			});

			if (UseCalendar && ShowDays)
			{
				w.AddClientAction("daterangepickerproxy", "init", f => new {
					triggerid = f(ID + "_" + "btn" + ID),
					onselectcallback = JSOnSelectCallback
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
					var from = Context.GetDateTimeArg(ID + "_" + "dperiodfrom");
					var to = Context.GetDateTimeArg(ID + "_" + "dperiodto");
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
						///Извлечь часы и минуты из def и прибавить их к to и from 
						if (fromtime != null) from = from.Value.AddHours(DefaultValue.From.Hour).AddMinutes(DefaultValue.From.Minute);
						if (totime != null) to = to.Value.AddHours(DefaultValue.To.Hour).AddMinutes(DefaultValue.To.Minute);
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
