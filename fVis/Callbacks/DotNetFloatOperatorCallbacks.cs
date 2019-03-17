using System;

namespace fVis.Callbacks
{
    /// <summary>
    /// Provides default "float" implementation from .NET Framework
    /// </summary>
    public class DotNetFloatOperatorCallbacks : OperatorCallbacks
    {
        public DotNetFloatOperatorCallbacks()
        {
            implementationName = ".NET (float)";

            constant_e = (float)Math.E;
            constant_pi = (float)Math.PI;

            add = OperatorPlus;
            subtract = OperatorMinus;
            multiply = OperatorMultiply;
            divide = OperatorDivide;
            pow = OperatorPow;
            remainder = OperatorRemainder;

            abs = AbsF;
            sqrt = SqrtF;
            exp = ExpF;
            ln = LogF;
            log = Log10F;
            sin = SinF;
            cos = CosF;
            tan = TanF;
            asin = AsinF;
            acos = AcosF;
            atan = AtanF;
            sinh = SinhF;
            cosh = CoshF;
            tanh = TanhF;

            round = RoundF;
            floor = FloorF;
            ceil = CeilingF;
        }

        private static double OperatorPlus(double x, double y)
        {
            return (float)x + (float)y;
        }

        private static double OperatorMinus(double x, double y)
        {
            return (float)x - (float)y;
        }

        private static double OperatorMultiply(double x, double y)
        {
            return (float)x * (float)y;
        }

        private static double OperatorDivide(double x, double y)
        {
            return (float)x / (float)y;
        }

        private static double OperatorPow(double x, double y)
        {
            return (float)Math.Pow((float)x, (float)y);
        }

        private static double OperatorRemainder(double x, double y)
        {
            return (float)x % (float)y;
        }

        private static double AbsF(double x)
        {
            return Math.Abs((float)x);
        }

        private static double SqrtF(double x)
        {
            return (float)Math.Sqrt((float)x);
        }

        private static double ExpF(double x)
        {
            return (float)Math.Exp((float)x);
        }

        private static double LogF(double x)
        {
            return (float)Math.Log((float)x);
        }

        private static double Log10F(double x)
        {
            return (float)Math.Log10((float)x);
        }

        private static double SinF(double x)
        {
            return (float)Math.Sin((float)x);
        }

        private static double CosF(double x)
        {
            return (float)Math.Cos((float)x);
        }

        private static double TanF(double x)
        {
            return (float)Math.Tan((float)x);
        }

        private static double AsinF(double x)
        {
            return (float)Math.Asin((float)x);
        }

        private static double AcosF(double x)
        {
            return (float)Math.Acos((float)x);
        }

        private static double AtanF(double x)
        {
            return (float)Math.Atan((float)x);
        }

        private static double SinhF(double x)
        {
            return (float)Math.Sinh((float)x);
        }

        private static double CoshF(double x)
        {
            return (float)Math.Cosh((float)x);
        }

        private static double TanhF(double x)
        {
            return (float)Math.Tanh((float)x);
        }

        private static double RoundF(double x)
        {
            return (float)Math.Round((float)x);
        }

        private static double FloorF(double x)
        {
            return (float)Math.Floor((float)x);
        }

        private static double CeilingF(double x)
        {
            return (float)Math.Ceiling((float)x);
        }
    }
}