using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using fVis.Native;

namespace fVis.Callbacks
{
    /// <summary>
    /// Provides mechanism to call native mathematical libraries with different bitness
    /// </summary>
    public class NativeOperatorRemotingCallbacks : DotNetOperatorCallbacks
    {
        private NativeLibraryRemoting library;
        private readonly string[] missingCallbacks;

        /// <summary>
        /// List of functions that are not included in selected mathematical library
        /// </summary>
        public string[] MissingCallbacks
        {
            get { return missingCallbacks; }
        }

        public NativeOperatorRemotingCallbacks(string path)
        {
            library = new NativeLibraryRemoting(path);

            extension_info info;
            try {
                info = library.Invoke<get_extension_info_t, extension_info>("get_extension_info");
            } catch {
                library.Dispose();
                library = null;
                throw new InvalidOperationException();
            }

            if (info.library_name != IntPtr.Zero) {
                implementationName = library.ResolveStringAnsi(info.library_name);

                if (info.version_build != 0) {
                    implementationName += " v" + info.version_major + "." + info.version_minor + "." + info.version_build;
                } else if (info.version_major != 0 || info.version_minor != 0) {
                    implementationName += " v" + info.version_major + "." + info.version_minor;
                }

                if ((info.flags & extension_flags.AVX2) != 0) {
                    implementationName += " [AVX2]";
                } else if ((info.flags & extension_flags.AVX) != 0) {
                    implementationName += " [AVX]";
                } else if ((info.flags & extension_flags.SSE2) != 0) {
                    implementationName += " [SSE2]";
                } else if ((info.flags & extension_flags.SSE) != 0) {
                    implementationName += " [SSE]";
                }
            } else {
                implementationName = "Unknown";
            }

            List<string> missingCallbacksList = new List<string>();

            Constant e = null;
            Resolve(ref e, ConstantE, "constant_e", missingCallbacksList);
            if (e != null) {
                constant_e = e();
            }

            Constant pi = null;
            Resolve(ref pi, ConstantPi, "constant_pi", missingCallbacksList);
            if (pi != null) {
                constant_pi = pi();
            }

            Resolve(ref add, OperatorAdd, "operator_add", missingCallbacksList);
            Resolve(ref subtract, OperatorSubtract, "operator_subtract", missingCallbacksList);
            Resolve(ref multiply, OperatorMultiply, "operator_multiply", missingCallbacksList);
            Resolve(ref divide, OperatorDivide, "operator_divide", missingCallbacksList);
            Resolve(ref pow, OperatorPow, "operator_pow", missingCallbacksList);
            Resolve(ref remainder, OperatorRemainder, "operator_remainder", missingCallbacksList);

            Resolve(ref abs, OperatorAbs, "operator_abs", missingCallbacksList);
            Resolve(ref sqrt, OperatorSqrt, "operator_sqrt", missingCallbacksList);
            Resolve(ref exp, OperatorExp, "operator_exp", missingCallbacksList);
            Resolve(ref ln, OperatorLn, "operator_ln", missingCallbacksList);
            Resolve(ref log, OperatorLog, "operator_log", missingCallbacksList);
            Resolve(ref sin, OperatorSin, "operator_sin", missingCallbacksList);
            Resolve(ref cos, OperatorCos, "operator_cos", missingCallbacksList);
            Resolve(ref tan, OperatorTan, "operator_tan", missingCallbacksList);
            Resolve(ref asin, OperatorAsin, "operator_asin", missingCallbacksList);
            Resolve(ref acos, OperatorAcos, "operator_acos", missingCallbacksList);
            Resolve(ref atan, OperatorAtan, "operator_atan", missingCallbacksList);
            Resolve(ref sinh, OperatorSinh, "operator_sinh", missingCallbacksList);
            Resolve(ref cosh, OperatorCosh, "operator_cosh", missingCallbacksList);
            Resolve(ref tanh, OperatorTanh, "operator_tanh", missingCallbacksList);

            Resolve(ref round, OperatorRound, "operator_round", missingCallbacksList);
            Resolve(ref floor, OperatorFloor, "operator_floor", missingCallbacksList);
            Resolve(ref ceil, OperatorCeil, "operator_ceil", missingCallbacksList);

            missingCallbacks = missingCallbacksList.ToArray();
        }

        protected override void Dispose(bool disposing)
        {
            if (library != null) {
                library.Dispose();
                library = null;
            }
        }

        /// <summary>
        /// Tries to find specified function in mathematical library
        /// </summary>
        /// <typeparam name="T">Delegate type</typeparam>
        /// <param name="callback">Callback variable</param>
        /// <param name="proxyCallback">Proxy method</param>
        /// <param name="procedureName">Name of function</param>
        /// <param name="missingCallbacksList">List of missing callbacks</param>
        protected void Resolve<T>(ref T callback, T proxyCallback, string procedureName, List<string> missingCallbacksList = null) where T : class
        {
            bool procedureExists = library.ProcedureExists(procedureName);
            if (procedureExists) {
                callback = proxyCallback;
            } else {
                missingCallbacksList?.Add(procedureName);
            }
        }

        #region Proxy Methods
        private double ConstantE()
        {
            return library.Invoke<Constant, double>("constant_e");
        }

        private double ConstantPi()
        {
            return library.Invoke<Constant, double>("constant_pi");
        }

        private double OperatorAdd(double x, double y)
        {
            return library.Invoke<OperatorFunction, double>("operator_add", x, y);
        }

        private double OperatorSubtract(double x, double y)
        {
            return library.Invoke<OperatorFunction, double>("operator_subtract", x, y);
        }

        private double OperatorMultiply(double x, double y)
        {
            return library.Invoke<OperatorFunction, double>("operator_multiply", x, y);
        }

        private double OperatorDivide(double x, double y)
        {
            return library.Invoke<OperatorFunction, double>("operator_divide", x, y);
        }

        private double OperatorPow(double x, double y)
        {
            return library.Invoke<OperatorFunction, double>("operator_pow", x, y);
        }

        private double OperatorRemainder(double x, double y)
        {
            return library.Invoke<OperatorFunction, double>("operator_remainder", x, y);
        }

        private double OperatorAbs(double x)
        {
            return library.Invoke<OperatorUnaryFunction, double>("operator_abs", x);
        }

        private double OperatorSqrt(double x)
        {
            return library.Invoke<OperatorUnaryFunction, double>("operator_sqrt", x);
        }

        private double OperatorExp(double x)
        {
            return library.Invoke<OperatorUnaryFunction, double>("operator_exp", x);
        }

        private double OperatorLn(double x)
        {
            return library.Invoke<OperatorUnaryFunction, double>("operator_ln", x);
        }

        private double OperatorLog(double x)
        {
            return library.Invoke<OperatorUnaryFunction, double>("operator_log", x);
        }

        private double OperatorSin(double x)
        {
            return library.Invoke<OperatorUnaryFunction, double>("operator_sin", x);
        }

        private double OperatorCos(double x)
        {
            return library.Invoke<OperatorUnaryFunction, double>("operator_cos", x);
        }

        private double OperatorTan(double x)
        {
            return library.Invoke<OperatorUnaryFunction, double>("operator_tan", x);
        }

        private double OperatorAsin(double x)
        {
            return library.Invoke<OperatorUnaryFunction, double>("operator_asin", x);
        }

        private double OperatorAcos(double x)
        {
            return library.Invoke<OperatorUnaryFunction, double>("operator_acos", x);
        }

        private double OperatorAtan(double x)
        {
            return library.Invoke<OperatorUnaryFunction, double>("operator_atan", x);
        }

        private double OperatorSinh(double x)
        {
            return library.Invoke<OperatorUnaryFunction, double>("operator_sinh", x);
        }

        private double OperatorCosh(double x)
        {
            return library.Invoke<OperatorUnaryFunction, double>("operator_cosh", x);
        }

        private double OperatorTanh(double x)
        {
            return library.Invoke<OperatorUnaryFunction, double>("operator_tanh", x);
        }

        private double OperatorRound(double x)
        {
            return library.Invoke<OperatorUnaryFunction, double>("operator_round", x);
        }

        private double OperatorFloor(double x)
        {
            return library.Invoke<OperatorUnaryFunction, double>("operator_floor", x);
        }

        private double OperatorCeil(double x)
        {
            return library.Invoke<OperatorUnaryFunction, double>("operator_ceil", x);
        }
        #endregion

        #region Native Methods
        [Flags]
        private enum extension_flags
        {
            None = 0,
            SSE = 1 << 0,
            SSE2 = 1 << 1,
            AVX = 1 << 2,
            AVX2 = 1 << 3,
        }

        [StructLayout(LayoutKind.Sequential), Serializable]
        private struct extension_info
        {
            public IntPtr library_name;

            public ushort version_major;
            public ushort version_minor;
            public ushort version_build;

            public extension_flags flags;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate extension_info get_extension_info_t();
        #endregion
    }
}