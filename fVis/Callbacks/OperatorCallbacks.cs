using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using fVis.Utils;

namespace fVis.Callbacks
{
    /// <summary>
    /// Provides mechanism to call different implementations of the same mathematical functions
    /// </summary>
    public abstract class OperatorCallbacks : Disposable
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate double Constant();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate double OperatorFunction(double x, double y);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate double OperatorUnaryFunction(double x);

        protected string implementationName;

        protected double constant_e;
        protected double constant_pi;

        protected OperatorFunction add;
        protected OperatorFunction subtract;
        protected OperatorFunction multiply;
        protected OperatorFunction divide;
        protected OperatorFunction pow;
        protected OperatorFunction remainder;

        protected OperatorUnaryFunction abs;
        protected OperatorUnaryFunction sqrt;
        protected OperatorUnaryFunction exp;
        protected OperatorUnaryFunction ln;
        protected OperatorUnaryFunction log;
        protected OperatorUnaryFunction sin;
        protected OperatorUnaryFunction cos;
        protected OperatorUnaryFunction tan;
        protected OperatorUnaryFunction asin;
        protected OperatorUnaryFunction acos;
        protected OperatorUnaryFunction atan;
        protected OperatorUnaryFunction sinh;
        protected OperatorUnaryFunction cosh;
        protected OperatorUnaryFunction tanh;

        protected OperatorUnaryFunction round;
        protected OperatorUnaryFunction floor;
        protected OperatorUnaryFunction ceil;

        /// <summary>
        /// Name of mathematical library
        /// </summary>
        public string ImplementationName
        {
            get { return implementationName; }
        }

        public double E
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return constant_e; }
        }

        public double PI
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return constant_pi; }
        }

        public OperatorFunction Add
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return add; }
        }

        public OperatorFunction Subtract
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return subtract; }
        }

        public OperatorFunction Multiply
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return multiply; }
        }

        public OperatorFunction Divide
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return divide; }
        }

        public OperatorFunction Pow
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return pow; }
        }

        public OperatorFunction Remainder
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return remainder; }
        }

        public OperatorUnaryFunction Abs
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return abs; }
        }

        public OperatorUnaryFunction Sqrt
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return sqrt; }
        }

        public OperatorUnaryFunction Exp
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return exp; }
        }

        public OperatorUnaryFunction Ln
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return ln; }
        }

        public OperatorUnaryFunction Log
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return log; }
        }

        public OperatorUnaryFunction Sin
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return sin; }
        }

        public OperatorUnaryFunction Cos
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return cos; }
        }

        public OperatorUnaryFunction Tan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return tan; }
        }

        public OperatorUnaryFunction Asin
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return asin; }
        }

        public OperatorUnaryFunction Acos
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return acos; }
        }

        public OperatorUnaryFunction Atan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return atan; }
        }

        public OperatorUnaryFunction Sinh
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return sinh; }
        }

        public OperatorUnaryFunction Cosh
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return cosh; }
        }

        public OperatorUnaryFunction Tanh
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return tanh; }
        }

        public OperatorUnaryFunction Round
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return round; }
        }

        public OperatorUnaryFunction Floor
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return floor; }
        }

        public OperatorUnaryFunction Ceil
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return ceil; }
        }
    }
}