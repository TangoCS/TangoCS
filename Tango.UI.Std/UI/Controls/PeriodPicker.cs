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
