using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Nephrite.Web.Controls
{
    public class ZoneCalendar : CompositeControl
    {
        JSCalendar jsCal;
        TextBox hours;
        TextBox minutes;
        DropDownList ddlZone;

        List<IN_TimeZone> zones;

        protected override void CreateChildControls()
        {
            Controls.Clear();

            jsCal = new JSCalendar();
            
            Controls.Add(jsCal);
            Controls.Add(new LiteralControl(" "));

            hours = new TextBox { EnableViewState = false, Width = Unit.Pixel(25), ToolTip = "Часы" };

            Controls.Add(hours);
            Controls.Add(new LiteralControl(" : "));

            minutes = new TextBox { EnableViewState = false, Width = Unit.Pixel(25), ToolTip = "Минуты" };

            Controls.Add(minutes);

            Controls.Add(new LiteralControl(" "));

            ddlZone = new DropDownList { ToolTip = "Часовой пояс", EnableViewState = false, Width = Unit.Pixel(300) };
            ddlZone.Style.Add(HtmlTextWriterStyle.Padding, "2px");

            zones = ((IDC_TimeZone)A.Model).IN_TimeZone.Where(o => !o.IsDeleted).OrderBy(o => o.GMTOffset).ToList();
            ddlZone.DataTextField = "Title";
            ddlZone.DataValueField = "TimeZoneID";
            ddlZone.DataBindOnce(zones.Select(o => new
                {
                    o.TimeZoneID,
                    Title = "(GMT" + (o.GMTOffset >= 0 ? "+" : "") + o.GMTOffset.ToString("00") + ") " + o.Title
                }), true);

            Controls.Add(ddlZone);
        }

        public void SetDefault(int timeZoneID)
        {
			ddlZone.SetValue(timeZoneID);
        }

        public void SetDefault(DateTime date)
        {
            jsCal.Date = date;
        }

        public void SetDefault(DateTime date, int timeZoneID)
        {
            SetDefault(timeZoneID);
            SetDefault(date);
        }

        public ZoneDateTime? ZoneDateTime
        {
            get 
            {
                if (jsCal.Date.HasValue && ddlZone.SelectedValue.ToInt32() > 0)
                    return new ZoneDateTime(jsCal.Date.Value.AddHours(hours.Text.ToInt32(0)).AddMinutes(minutes.Text.ToInt32(0)), ddlZone.SelectedValue.ToInt32(0), false);
                else
                    return null;
            }
            set 
            {
                if (value.HasValue)
                {
                    jsCal.Date = value.Value.LocalDateTime;
                    if (ddlZone.Items.FindByValue(value.Value.TimeZoneID.ToString()) == null)
                    {
                        var z = zones.Where(o => o.GMTOffset == value.Value.GMTOffset).SingleOrDefault();
                        if (z != null)
                            ddlZone.SetValue(z.TimeZoneID);
                    }
                    else
                        ddlZone.SetValue(value.Value.TimeZoneID);
                    hours.Text = value.Value.LocalDateTime.Hour.ToString();
                    minutes.Text = value.Value.LocalDateTime.Minute.ToString("00");
                }
                else
                {
                    jsCal.Date = null;
                    hours.Text = "";
                    minutes.Text = "";
                    ddlZone.ClearSelection();
                }
            }
        }

        
    }

	/// <summary>
	/// Дата-время с часовым поясом
	/// </summary>
	public struct ZoneDateTime
	{
		/// <summary>
		/// Локальное дата-время
		/// </summary>
		public readonly DateTime LocalDateTime;

		/// <summary>
		/// Часовой пояс
		/// </summary>
		public readonly int GMTOffset;

		/// <summary>
		/// Ид часового пояса
		/// </summary>
		public int TimeZoneID
		{
			get { return timeZone.TimeZoneID; }
		}

		/// <summary>
		/// Наименование
		/// </summary>
		public string TimeZoneTitle
		{
			get { return timeZone.Title; }
		}

		readonly IN_TimeZone timeZone;
		/// <summary>
		/// Создать объект дата-время с часовым поясом
		/// </summary>
		/// <param name="dateTime">Дата-время</param>
		/// <param name="timeZoneVersionID">Ид версии часового пояса</param>
		/// <param name="isUtc">Признак "универсальное"</param>
		public ZoneDateTime(DateTime dateTime, int timeZoneID, bool isUtc)
		{
			timeZone = ((IDC_TimeZone)A.Model).IN_TimeZone.Single(o => o.TimeZoneID == timeZoneID);
			GMTOffset = timeZone.GMTOffset;
			if (isUtc)
			{
				LocalDateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.CreateCustomTimeZone("1", new TimeSpan(timeZone.GMTOffset, 0, 0), "", "", "", TimeZoneInfo.Local.GetAdjustmentRules()));
			}
			else
			{
				LocalDateTime = dateTime;
			}
		}

		public ZoneDateTime(DateTime dateTime, int timeZoneID)
		{
			timeZone = ((IDC_TimeZone)A.Model).IN_TimeZone.Single(o => o.TimeZoneID == timeZoneID);
			GMTOffset = timeZone.GMTOffset;
			LocalDateTime = dateTime;
		}

		/// <summary>
		/// Универсальное дата-время
		/// </summary>
		public DateTime UniversalDateTime
		{
			get
			{
				return TimeZoneInfo.ConvertTimeToUtc(LocalDateTime, TimeZoneInfo);
			}
		}

		TimeZoneInfo TimeZoneInfo
		{
			get
			{
				return TimeZoneInfo.CreateCustomTimeZone("1", new TimeSpan(GMTOffset, 0, 0), "", "", "", TimeZoneInfo.Local.GetAdjustmentRules());
			}
		}

		public override string ToString()
		{
			int ds = TimeZoneInfo.IsDaylightSavingTime(LocalDateTime) ? 1 : 0;
			return LocalDateTime.ToString("dd.MM.yyyy HH:mm") + " (GMT" + (GMTOffset + ds > 0 ? "+" : "-") + (GMTOffset + ds).ToString("00") + ":00) " + timeZone.Title;
		}
	}

	public interface IDC_TimeZone : IDataContext
	{
		IQueryable<IN_TimeZone> IN_TimeZone { get; }
	}

	public interface IN_TimeZone
	{
		int TimeZoneID { get; set; }
		int LastModifiedUserID { get; set; }
		bool IsDeleted { get; set; }
		System.DateTime LastModifiedDate { get; set; }
		string Title { get; set; }
		int GMTOffset { get; set; }
		string Comment { get; set; }
	}
}
