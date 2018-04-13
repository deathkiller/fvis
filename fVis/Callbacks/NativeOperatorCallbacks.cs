using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using fVis.Native;

namespace fVis.Callbacks
{
    /// <summary>
    /// Provides mechanism to call native mathematical libraries
    /// </summary>
    public class NativeOperatorCallbacks : DotNetOperatorCallbacks
    {
        private NativeLibrary library;
        private readonly string[] missingCallbacks;

        public string[] MissingCallbacks
        {
            get { return missingCallbacks; }
        }

        public NativeOperatorCallbacks(string path)
        {
            library = new NativeLibrary(path);

            get_extension_info_t get_extension_info = null;
            Resolve(ref get_extension_info, "get_extension_info");
            if (get_extension_info == null) {
                library.Dispose();
                library = null;
                throw new InvalidOperationException();
            }

            extension_info info = get_extension_info();
            if (info.library_name != IntPtr.Zero) {
                implementationName = Marshal.PtrToStringAnsi(info.library_name);

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
            Resolve(ref e, "constant_e", missingCallbacksList);
            if (e != null) {
                constant_e = e();
            }

            Constant pi = null;
            Resolve(ref pi, "constant_pi", missingCallbacksList);
            if (pi != null) {
                constant_pi = pi();
            }

            Resolve(ref add, "operator_add", missingCallbacksList);
            Resolve(ref subtract, "operator_subtract", missingCallbacksList);
            Resolve(ref multiply, "operator_multiply", missingCallbacksList);
            Resolve(ref divide, "operator_divide", missingCallbacksList);
            Resolve(ref pow, "operator_pow", missingCallbacksList);
            Resolve(ref remainder, "operator_remainder", missingCallbacksList);

            Resolve(ref abs, "operator_abs", missingCallbacksList);
            Resolve(ref sqrt, "operator_sqrt", missingCallbacksList);
            Resolve(ref exp, "operator_exp", missingCallbacksList);
            Resolve(ref ln, "operator_ln", missingCallbacksList);
            Resolve(ref log, "operator_log", missingCallbacksList);
            Resolve(ref sin, "operator_sin", missingCallbacksList);
            Resolve(ref cos, "operator_cos", missingCallbacksList);
            Resolve(ref tan, "operator_tan", missingCallbacksList);
            Resolve(ref asin, "operator_asin", missingCallbacksList);
            Resolve(ref acos, "operator_acos", missingCallbacksList);
            Resolve(ref atan, "operator_atan", missingCallbacksList);
            Resolve(ref sinh, "operator_sinh", missingCallbacksList);
            Resolve(ref cosh, "operator_cosh", missingCallbacksList);
            Resolve(ref tanh, "operator_tanh", missingCallbacksList);

            Resolve(ref round, "operator_round", missingCallbacksList);
            Resolve(ref floor, "operator_floor", missingCallbacksList);
            Resolve(ref ceil, "operator_ceil", missingCallbacksList);

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
        /// <param name="procedureName">Name of function</param>
        /// <param name="missingCallbacksList">List of missing callbacks</param>
        protected void Resolve<T>(ref T callback, string procedureName, List<string> missingCallbacksList = null) where T : class
        {
            T resolvedCallback = library.Resolve<T>(procedureName);
            if (resolvedCallback == null) {
                missingCallbacksList?.Add(procedureName);
            } else {
                callback = resolvedCallback;
            }
        }

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

        [StructLayout(LayoutKind.Sequential)]
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