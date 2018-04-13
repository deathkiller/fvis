using System;

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
                        return "1 rok a " + days.ToString("N0") + " dní";
                    } else if (years < 5) {
                        return years.ToString("N0") + " roky a " + days.ToString("N0") + " dní";
                    } else {
                        return years.ToString("N0") + " let a " + days.ToString("N0") + " dní";
                    }
                } else {
                    return time.Days.ToString("N0") + " dní a " + time.Hours.ToString("N0") + " hodin";
                }
            } else if (time.Hours > 0) {
                return time.Hours.ToString("N0") + " hodin a " + time.Minutes.ToString("N0") + " minut";
            } else if (time.Minutes > 0) {
                return time.Minutes.ToString("N0") + " minut a " + time.Seconds.ToString("N0") + " sekund";
            } else {
                return time.Seconds.ToString("N0") + " sekund";
            }
        }
    }
}