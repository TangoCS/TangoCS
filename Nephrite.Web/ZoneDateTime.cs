using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web.Model;

namespace Nephrite.Web
{
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
        /// Ид версии часового пояса
        /// </summary>
        public int TimeZoneVersionID
        {
            get { return timeZone.TimeZoneVersionID; }
        }

        /// <summary>
        /// Наименование
        /// </summary>
        public string TimeZoneTitle
        {
            get { return timeZone.Title; }
        }

        readonly HST_N_TimeZone timeZone;
        /// <summary>
        /// Создать объект дата-время с часовым поясом
        /// </summary>
        /// <param name="dateTime">Дата-время</param>
        /// <param name="timeZoneVersionID">Ид версии часового пояса</param>
        /// <param name="isUtc">Признак "универсальное"</param>
        public ZoneDateTime(DateTime dateTime, int timeZoneVersionID, bool isUtc)
        {
            timeZone = AppWeb.DataContext.HST_N_TimeZones.Single(o => o.TimeZoneVersionID == timeZoneVersionID);
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
            timeZone = AppWeb.DataContext.HST_N_TimeZones.Single(o => o.TimeZoneID == timeZoneID && o.IsCurrentVersion);
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
}
