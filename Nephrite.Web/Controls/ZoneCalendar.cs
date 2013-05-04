using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using Nephrite.Web.Model;

namespace Nephrite.Web.Controls
{
    public class ZoneCalendar : CompositeControl
    {
        JSCalendar jsCal;
        TextBox hours;
        TextBox minutes;
        DropDownList ddlZone;

        List<HST_N_TimeZone> zones;

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

            zones = AppWeb.DataContext.HST_N_TimeZones.
                Where(o => o.IsCurrentVersion && !o.IsDeleted).OrderBy(o => o.GMTOffset).ToList();
            ddlZone.DataTextField = "Title";
            ddlZone.DataValueField = "TimeZoneVersionID";
            ddlZone.DataBindOnce(zones.Select(o => new
                {
                    o.TimeZoneVersionID,
                    Title = "(GMT" + (o.GMTOffset >= 0 ? "+" : "") + o.GMTOffset.ToString("00") + ") " + o.Title
                }), true);

            Controls.Add(ddlZone);
        }

        public void SetDefault(int timeZoneID)
        {
            var z = zones.Where(o => o.TimeZoneID == timeZoneID).SingleOrDefault();
            if (z != null)
                ddlZone.SetValue(z.TimeZoneVersionID);
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
                    if (ddlZone.Items.FindByValue(value.Value.TimeZoneVersionID.ToString()) == null)
                    {
                        var z = zones.Where(o => o.GMTOffset == value.Value.GMTOffset).SingleOrDefault();
                        if (z != null)
                            ddlZone.SetValue(z.TimeZoneVersionID);
                    }
                    else
                        ddlZone.SetValue(value.Value.TimeZoneVersionID);
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
}
