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

		public override void OnInit()
		{
			dPeriodFrom = CreateControl<DateLists>(UseCalendar ? $"dperiodfromtime" : $"dperiodfrom", c => {
				c.ShowDays = ShowDays && !UseCalendar;
				c.ShowTime = ShowTime;
				c.TimeOnly = UseCalendar && ShowDays;
				c.MinutesStep = 30;
				if (DefaultValue != null) c.DefaultValue = DefaultValue.From;
			});
			dPeriodTo = CreateControl<DateLists>(UseCalendar ? $"dperiodtotime" : $"dperiodto", c => {
				c.ShowDays = ShowDays && !UseCalendar;
				c.ShowTime = ShowTime;
				c.TimeOnly = UseCalendar && ShowDays;
				c.MinutesStep = 30;
				if (DefaultValue != null) c.DefaultValue = DefaultValue.To;
			});
		}

		public void Render(LayoutWriter w, DateTime? from = null, DateTime? to = null)
		{
			dPeriodFrom.MinYear = MinYear;
			dPeriodTo.MinYear = MinYear;

			dPeriodFrom.MaxYear = DateTime.Today.Year;
			dPeriodTo.MaxYear = DateTime.Today.Year;

            if (from == null)
                from = Context.GetDateTimeArg("dperiodfrom");
            if (to == null)
                to = Context.GetDateTimeArg("dperiodto");

            w.PushID(ID);
			w.Div(a => a.Class("periodpicker").ID(), () => {
				w.Div("с");
				if (UseCalendar && ShowDays) w.Calendar("dperiodfrom", from ?? DefaultValue?.From);
				if (!UseCalendar || ShowTime)
					dPeriodFrom.Render(w, from ?? DefaultValue?.From);
				w.Div("по");
				if (UseCalendar && ShowDays) w.Calendar("dperiodto", to ?? DefaultValue?.To);
				if (!UseCalendar || ShowTime)
					dPeriodTo.Render(w, to ?? DefaultValue?.To);
			});
			w.PopID();
		}

		public PeriodValue Value
		{
			get
			{
				if (UseCalendar && ShowDays)
				{
					var from = Context.GetDateTimeArg("dperiodfrom");
					var to = Context.GetDateTimeArg("dperiodto");
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

		public Expression<Func<T, bool>> GetPredicate<T>(Expression<Func<T, DateTime?>> selector)
		{
			//var arr = AsArray();

			//if (arr.Length == 0)
			//	return o => true;

			ParameterExpression pe_o = Expression.Parameter(typeof(T), "o");
			UnaryExpression ue_c = Expression.Convert(pe_o, typeof(T));
			MemberExpression me = Expression.Property(ue_c, ((MemberExpression)selector.Body).Member.Name);

			List<Expression> expressions = new List<Expression>();

			//foreach (var p in arr)
			//{
				ConstantExpression ce_valFrom = Expression.Constant(Value.From, typeof(DateTime));
				ConstantExpression ce_valTo = Expression.Constant(Value.To, typeof(DateTime));

				Expression GreaterThan = Expression.GreaterThanOrEqual(me, ce_valFrom);
				Expression lessThan = Expression.LessThanOrEqual(me, ce_valTo);
				expressions.Add(Expression.And(lessThan, GreaterThan));
			//}

			Expression body = expressions[0];
			for (int i = 1; i < expressions.Count; i++)
				body = Expression.Or(body, expressions[i]);
			return Expression.Lambda<Func<T, bool>>(body, pe_o);
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
