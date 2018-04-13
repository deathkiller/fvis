using System.Globalization;
using Mpir.NET;

namespace fVis.Extensions
{
    public static partial class DoubleExtensions
    {
        private const int DefaultPrecision = 300;

        static DoubleExtensions()
        {
            try {
                mpir.mpf_set_default_prec(DefaultPrecision);
            } catch {
                // Nothing to do...
            }
        }

        public static double SubtractDivideLossless(this int x, /*double*/long subtract, double divide)
        {
            using (mpf_t x_ = new mpf_t(x))
            //using (mpf_t subtract_ = new mpf_t(subtract))
            using (mpf_t subtract_ = new mpf_t(subtract.ToString(CultureInfo.InvariantCulture)))
            using (mpf_t divide_ = new mpf_t(divide))
            using (mpf_t result_ = mpf_t.init2(DefaultPrecision)) {
                mpir.mpf_sub(result_, x_, subtract_);
                mpir.mpf_div(result_, result_, divide_);    // result = (x - subtract) / divide
                return mpir.mpf_get_d(result_);
            }
        }

        public static int MultiplyAddLosslessToInt(this double x, double multiply, /*double*/long add)
        {
            using (mpf_t x_ = new mpf_t(x))
            using (mpf_t multiply_ = new mpf_t(multiply))
            //using (mpf_t add_ = new mpf_t(add))
            using (mpf_t add_ = new mpf_t(add.ToString(CultureInfo.InvariantCulture)))
            using (mpf_t result_ = mpf_t.init2(DefaultPrecision)) {
                mpir.mpf_mul(result_, x_, multiply_);       // result = (x * multiply) + add
                mpir.mpf_add(result_, result_, add_);
                return mpir.mpf_get_si(result_);
            }
        }

        public static double NegMultiplyAddLossless(this double x, double multiply, /*double*/long add)
        {
            using (mpf_t x_ = new mpf_t(x))
            using (mpf_t multiply_ = new mpf_t(multiply))
            //using (mpf_t add_ = new mpf_t(add))
            using (mpf_t add_ = new mpf_t(add.ToString(CultureInfo.InvariantCulture)))
            using (mpf_t result_ = mpf_t.init2(DefaultPrecision)) {
                mpir.mpf_neg(result_, x_);
                mpir.mpf_mul(result_, result_, multiply_);  // result = (-x * multiply) + add
                mpir.mpf_add(result_, result_, add_);
                return mpir.mpf_get_d(result_);
            }
        }

        public static int NegMultiplyAddLosslessToInt(this double x, double multiply, /*double*/long add)
        {
            using (mpf_t x_ = new mpf_t(x))
            using (mpf_t multiply_ = new mpf_t(multiply))
            //using (mpf_t add_ = new mpf_t(add))
            using (mpf_t add_ = new mpf_t(add.ToString(CultureInfo.InvariantCulture)))
            using (mpf_t result_ = mpf_t.init2(DefaultPrecision)) {
                mpir.mpf_neg(result_, x_);
                mpir.mpf_mul(result_, result_, multiply_);  // result = (-x * multiply) + add
                mpir.mpf_add(result_, result_, add_);
                return mpir.mpf_get_si(result_);
            }
        }
    }
}