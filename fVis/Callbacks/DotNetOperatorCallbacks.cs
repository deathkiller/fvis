using System;

namespace fVis.Callbacks
{
    /// <summary>
    /// Provides default "double" implementation from .NET Framework
    /// </summary>
    public class DotNetOperatorCallbacks : OperatorCallbacks
    {
        public DotNetOperatorCallbacks()
        {
            implementationName = ".NET (double)";

            constant_e = Math.E;
            constant_pi = Math.PI;

            add = OperatorAdd;
            subtract = OperatorSubtract;
            multiply = OperatorMultiply;
            divide = OperatorDivide;
            pow = Math.Pow;
            remainder = OperatorRemainder;

            abs = Math.Abs;
            sqrt = Math.Sqrt;
            exp = Math.Exp;
            ln = Math.Log;
            log = Math.Log10;
            sin = Math.Sin;
            cos = Math.Cos;
            tan = Math.Tan;
            asin = Math.Asin;
            acos = Math.Acos;
            atan = Math.Atan;
            sinh = Math.Sinh;
            cosh = Math.Cosh;
            tanh = Math.Tanh;

            round = Math.Round;
            floor = Math.Floor;
            ceil = Math.Ceiling;
        }

        private static double OperatorAdd(double x, double y)
        {
            return x + y;
        }

        private static double OperatorSubtract(double x, double y)
        {
            return x - y;
        }

        private static double OperatorMultiply(double x, double y)
        {
            return x * y;
        }

        private static double OperatorDivide(double x, double y)
        {
            return x / y;
        }

        private static double OperatorRemainder(double x, double y)
        {
            return x % y;
        }
    }
}