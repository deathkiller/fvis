using System;
using Unclassified.TxLib;

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
                        return Tx.T("time.yr days", days.ToString("N0"));
                    } else if (years < 5) {
                        return Tx.T("time.few yrs days", years.ToString("N0"), days.ToString("N0"));
                    } else {
                        return Tx.T("time.yrs days", years.ToString("N0"), days.ToString("N0"));
                    }
                } else {
                    return Tx.T("time.days hrs", time.Days.ToString("N0"), time.Hours.ToString("N0"));
                }
            } else if (time.Hours > 0) {
                return Tx.T("time.hrs mins", time.Hours.ToString("N0"), time.Minutes.ToString("N0"));
            } else if (time.Minutes > 0) {
                return Tx.T("time.mins secs", time.Minutes.ToString("N0"), time.Seconds.ToString("N0"));
            } else {
                return Tx.T("time.secs", time.Seconds.ToString("N0"));
            }
        }
    }
}