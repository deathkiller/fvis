//
// Method ToExactString is taken from article by Jon Skeet and modified.
// http://csharpindepth.com/Articles/General/FloatingPoint.aspx
//

using System;
using System.Globalization;
using System.Text;

namespace fVis.Extensions
{
    public static partial class DoubleExtensions
    {
        /// <summary>
        /// Converts number to bit representation string.
        /// </summary>
        /// <param name="x">Input number</param>
        /// <returns>String with bit representation</returns>
        public static unsafe string ToBits(this double x)
        {
            const int BitCount = 8 * sizeof(double);

            // Cast double to long (64-bit)
            ulong bits = *(ulong*)&x;

            StringBuilder sb = new StringBuilder(BitCount + 2);
            for (int i = BitCount - 1; i >= 0; i--) {
                sb.Append(((bits >> i) & 1) > 0 ? "1" : "0");

                // Separators for double (64 bits)
                if (i == 63 || i == 52) {
                    sb.Append(" ");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts number to exact value string.
        /// </summary>
        /// <param name="x">Input number</param>
        /// <returns>Exact value string</returns>
        public static unsafe string ToExactString(this double x)
        {
            if (double.IsNaN(x)) {
                return "NaN";
            }
            if (double.IsPositiveInfinity(x)) {
                return "+∞";
            }
            if (double.IsNegativeInfinity(x)) {
                return "-∞";
            }

            // Cast double to long (64-bit) and extract all parts
            long bits = *(long*)&x;
            // Sign is the most significant bit, it's the same in long and double
            bool negative = (bits < 0);
            // Exponent is 11 bits long
            int exponent = (int)((bits >> 52) & 0x7ffL);
            // Mantisa is 52 bits long
            long mantisa = (bits & 0xfffffffffffffL);

            if (exponent == 0) {
                // Subnormal numbers; exponent is effectively one higher,
                // but there's no extra normalisation bit in the mantissa
                exponent++;
            } else {
                // Normal numbers; leave exponent as it is but add extra
                // bit to the front of the mantissa
                mantisa = mantisa | (1L << 52);
            }

            // Subtract bias from exponent
            exponent -= 1023;
            // Subtract number of mantisa bits from exponent
            exponent -= 52;

            if (mantisa == 0) {
                return " 0";
            }

            // Normalize
            while ((mantisa & 1) == 0) {
                // Mantissa is even
                mantisa >>= 1;
                exponent++;
            }

            // Construct a new decimal expansion with the mantissa
            ArbitraryDecimal ad = new ArbitraryDecimal(mantisa);

            if (exponent < 0) {
                // If the exponent is less than 0, we need to repeatedly
                // divide by 2 - which is the equivalent of multiplying
                // by 5 and dividing by 10.
                for (int i = 0; i < -exponent; i++) {
                    ad.Multiply(5);
                }
                ad.Shift(-exponent);
            } else {
                // Otherwise, we need to repeatedly multiply by 2
                for (int i = 0; i < exponent; i++) {
                    ad.Multiply(2);
                }
            }

            // Finally, return the string with an appropriate sign
            return (negative ? "-" : "+") + ad;
        }

        private class ArbitraryDecimal
        {
            private byte[] digits;
            private int decimalPoint;

            public ArbitraryDecimal(long x)
            {
                string tmp = x.ToString(CultureInfo.InvariantCulture);
                digits = new byte[tmp.Length];
                for (int i = 0; i < tmp.Length; i++) {
                    digits[i] = (byte)(tmp[i] - '0');
                }

                Normalize();
            }

            /// <summary>
            /// Multiplies the current expansion by the given amount, which should only be 2 or 5
            /// </summary>
            public void Multiply(int amount)
            {
                byte[] result = new byte[digits.Length + 1];
                for (int i = digits.Length - 1; i >= 0; i--) {
                    int resultDigit = digits[i] * amount + result[i + 1];
                    result[i] = (byte)(resultDigit / 10);
                    result[i + 1] = (byte)(resultDigit % 10);
                }

                if (result[0] != 0) {
                    digits = result;
                } else {
                    Array.Copy(result, 1, digits, 0, digits.Length);
                }

                Normalize();
            }

            /// <summary>
            /// Shifts the decimal point; a negative value makes
            /// the decimal expansion bigger (as fewer digits come after the
            /// decimal place) and a positive value makes the decimal
            /// expansion smaller.
            /// </summary>
            public void Shift(int amount)
            {
                decimalPoint += amount;
            }

            /// <summary>
            /// Removes leading/trailing zeroes from the expansion
            /// </summary>
            private void Normalize()
            {
                int first;
                for (first = 0; first < digits.Length; first++) {
                    if (digits[first] != 0) {
                        break;
                    }
                }
                int last;
                for (last = digits.Length - 1; last >= 0; last--) {
                    if (digits[last] != 0) {
                        break;
                    }
                }

                if (first == 0 && last == digits.Length - 1) {
                    return;
                }

                byte[] tmp = new byte[last - first + 1];
                for (int i = 0; i < tmp.Length; i++) {
                    tmp[i] = digits[i + first];
                }

                decimalPoint -= digits.Length - (last + 1);
                digits = tmp;
            }

            /// <summary>
            /// Converts the value to a proper decimal string representation
            /// </summary>
            public override string ToString()
            {
                char[] digitString = new char[digits.Length];
                for (int i = 0; i < digits.Length; i++) {
                    digitString[i] = (char)(digits[i] + '0');
                }

                // Simplest case - nothing after the decimal point,
                // and last real digit is non-zero, eg value=35
                if (decimalPoint == 0) {
                    return new string(digitString);
                }

                // Fairly simple case - nothing after the decimal
                // point, but some 0s to add, eg value=350
                if (decimalPoint < 0) {
                    return new string(digitString) + new string('0', -decimalPoint);
                }

                // Nothing before the decimal point, eg 0.035
                if (decimalPoint >= digitString.Length) {
                    return "0." + new string('0', (decimalPoint - digitString.Length)) + new string(digitString);
                }

                // Most complicated case - part of the string comes
                // before the decimal point, part comes after it, eg 3.5
                return new string(digitString, 0, digitString.Length - decimalPoint) + "." + new string(digitString, digitString.Length - decimalPoint, decimalPoint);
            }
        }
    }
}