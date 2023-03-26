using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace luanvanthacsi.Ultils
{
    public static class DateTimeExtentions
    {
        public static DateTime GetDateTimeOnly(this DateTime? dateTime) => dateTime.Value.Date;

        public static DateTime GetEndTimeNow() => new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
                        .AddDays(1).AddSeconds(-1);

        public static DateTime GetStartTimeBeforeYear(int year) => new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
                        .AddYears(-year);

        public static DateTime GetStartTimeBeforeDay(int day) => new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
                       .AddDays(-day);

        public static DateTime GetEndTimeByDate(this DateTime? dateTime) => new DateTime(dateTime.Value.Year, dateTime.Value.Month, dateTime.Value.Day)
                        .AddDays(1).AddSeconds(-1);

        public static DateTime GetStartTimeBeforeYearByDate(this DateTime? dateTime, int year) => new DateTime(dateTime.Value.Year, dateTime.Value.Month, dateTime.Value.Day)
                        .AddYears(-year);

        public static string ToShortDate(this DateTime? dateTime)
        {
            return dateTime?.ToString("dd/MM/yyyy");
        }

        public static string ToShortDate(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy");
        }

        public static string ToFullDate(this DateTime? dateTime)
        {
            return dateTime?.ToString("dd/MM/yyyy HH:mm:ss");
        }

        public static string ToFullDate(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
        }


        public static Dictionary<string, DateTime> GetDictionaryDates(DateTime startDate, DateTime endDate)
        {
            Dictionary<string, DateTime> DicYears = new Dictionary<string, DateTime>();
            for (DateTime date = startDate.Date; endDate.Date.CompareTo(date) >= 0; date = date.AddDays(1))
            {
                DicYears.Add(string.Format("Day{0:00}", date.Day), date);
            }
            return DicYears;
        }

        public static int GetDateAmount(DateTime startDate, DateTime endDate)
        {
            return (endDate.Date - startDate.Date).Days + 1;
        }

        public static DateTime GetDateTimeOnly(this DateTime dateTime) => dateTime.Date;

        public static DateTime GetEndTimeByDate(this DateTime dateTime) => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day)
                .AddDays(1).AddSeconds(-1);

        public static string GetSeniorityType(this DateTime? startDate)
        {
            string seniority = string.Empty;

            if (startDate.HasValue)
            {
                var value = DateTime.Now.Subtract(startDate.Value);
                if (value.Days > 1)
                {
                    DateTime interval = DateTime.MinValue + value;
                    string year = interval.Year != 1 ? string.Format("{0} năm, ", interval.Year - 1) : string.Empty;
                    string month = interval.Month != 1 ? string.Format("{0} tháng, ", interval.Month - 1) : string.Empty;
                    string day = interval.Day != 1 ? string.Format("{0} ngày", interval.Day - 1) : string.Empty;
                    seniority = year + month + day;
                }
            }
            return seniority;
        }

        public static string ToHourMinute(this decimal hour)
        {
            var interval = TimeSpan.FromHours((double)hour);
            return $"{interval.Days * 24 + interval.Hours} giờ {interval.Minutes} phút";
        }

        public static string MinuteToHourMinute(this double minute)
        {
            var interval = TimeSpan.FromMinutes(minute);
            return $"{interval.Days * 24 + interval.Hours} giờ {interval.Minutes} phút";
        }
        public static string FormatDayMonthYear(this DateTime? date)
        {
            return date != null ? string.Format("ngày {0:00} tháng {1:00} năm {2}", date.Value.Day, date.Value.Month, date.Value.Year) : string.Empty;
        }
        public static string FormatDayMonthYear(this DateTime date)
        {
            return string.Format("ngày {0:00} tháng {1:00} năm {2}", date.Day, date.Month, date.Year);
        }


    }
}
