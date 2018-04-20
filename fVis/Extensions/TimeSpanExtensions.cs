using System;
using l10n = fVis.Properties.Resources;

namespace fVis.Extensions
{
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Converts TimeSpan to human-readable string
        /// </summary>
        /// <param name="time">time span</param>
        /// <returns>human-readable string</returns>
        public static string ToTextString(this TimeSpan time)
        {
            if (time.Days > 0) {
                if (time.Days > 365) {
                    int years = (time.Days / 365);
                    int days = (time.Days % 365);

                    if (years == 1) {
                        return string.Format(l10n.TimeOneYearAndDays, days.ToString("N0"));
                    } else if (years < 5) {
                        return string.Format(l10n.TimeFewYearsAndDays, years.ToString("N0"), days.ToString("N0"));
                    } else {
                        return string.Format(l10n.TimeYearsAndDays, years.ToString("N0"), days.ToString("N0"));
                    }
                } else {
                    return string.Format(l10n.TimeDaysAndHours, time.Days.ToString("N0"), time.Hours.ToString("N0"));
                }
            } else if (time.Hours > 0) {
                return string.Format(l10n.TimeHoursAndMinutes, time.Hours.ToString("N0"), time.Minutes.ToString("N0"));
            } else if (time.Minutes > 0) {
                return string.Format(l10n.TimeMinutesAndSeconds, time.Minutes.ToString("N0"), time.Seconds.ToString("N0"));
            } else {
                return string.Format(l10n.TimeSeconds, time.Seconds.ToString("N0"));
            }
        }
    }
}