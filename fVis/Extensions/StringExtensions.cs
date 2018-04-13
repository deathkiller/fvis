namespace fVis.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Limits length of specified string
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="maxSize">Max. string length</param>
        /// <returns>Limited string</returns>
        public static string LimitSize(this string input, int maxSize)
        {
            if (input.Length <= maxSize) {
                return input;
            }

            return input.Substring(0, maxSize) + "…";
        }
    }
}