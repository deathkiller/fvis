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
                        return Tx.T("time.yr days", Tx.N(days));
                    } else if (years < 5) {
                        return Tx.T("time.few yrs days", Tx.N(years), Tx.N(days));
                    } else {
                        return Tx.T("time.yrs days", Tx.N(years), Tx.N(days));
                    }
                } else {
                    return Tx.T("time.days hrs", Tx.N(time.Days), Tx.N(time.Hours));
                }
            } else if (time.Hours > 0) {
                return Tx.T("time.hrs mins", Tx.N(time.Hours), Tx.N(time.Minutes));
            } else if (time.Minutes > 0) {
                return Tx.T("time.mins secs", Tx.N(time.Minutes), Tx.N(time.Seconds));
            } else {
                return Tx.T("time.secs", Tx.N(time.Seconds));
            }
        }
    }
}