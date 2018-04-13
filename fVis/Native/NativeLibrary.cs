using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using fVis.Utils;

namespace fVis.Native
{
    /// <summary>
    /// Provides mechanism to load native libraries and call its functions
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    public sealed class NativeLibrary : Disposable
    {
        private IntPtr hModule;

        public NativeLibrary(string path)
        {
            if (!Path.IsPathRooted(path)) {
                string pathRoot = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
                string pathBitness = Path.Combine(pathRoot, Environment.Is64BitProcess ? "x64" : "x86", path);
                if (File.Exists(pathBitness)) {
                    path = pathBitness;
                } else {
                    path = Path.Combine(pathRoot, path);
                }
            }

            hModule = LoadLibraryEx(path, IntPtr.Zero, 0x00000008 /*LOAD_WITH_ALTERED_SEARCH_PATH*/);
            if (hModule == IntPtr.Zero) {
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode == 0xc1) {
                    throw new BadImageFormatException();
                }

                throw new Win32Exception("LoadLibraryEx() returned NULL");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (hModule != IntPtr.Zero) {
                FreeLibrary(hModule);
                hModule = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Get delegate of function specified by name
        /// </summary>
        /// <param name="procedureName">Name of function</param>
        /// <param name="delegateType">Delegate type</param>
        /// <returns>Delegate</returns>
        public Delegate Resolve(string procedureName, Type delegateType)
        {
            if (!delegateType.IsSubclassOf(typeof(Delegate))) {
                throw new InvalidOperationException("Supplied type must be a delegate type");
            }

            IntPtr ptr = GetProcAddress(hModule, procedureName);
            return (ptr == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer(ptr, delegateType));
        }

        /// <summary>
        /// Get delegate of function specified by name
        /// </summary>
        /// <typeparam name="T">Delegate type</typeparam>
        /// <param name="procedureName">Name of function</param>
        /// <returns>Delegate</returns>
        public T Resolve<T>(string procedureName) where T : class
        {
            Type type = typeof(T);
            if (!type.IsSubclassOf(typeof(Delegate))) {
                throw new InvalidOperationException("Supplied type must be a delegate type");
            }

            IntPtr ptr = GetProcAddress(hModule, procedureName);
            return (ptr == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer(ptr, type) as T);
        }

        /// <summary>
        /// Check if specified function exists
        /// </summary>
        /// <param name="procedureName">Name of function</param>
        /// <returns>Returns true if exists; false, otherwise</returns>
        public bool ProcedureExists(string procedureName)
        {
            IntPtr ptr = GetProcAddress(hModule, procedureName);
            return (ptr != IntPtr.Zero);
        }

        #region Native Library
        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, uint dwFlags);

        [DllImport("kernel32")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32")]
        private static extern bool FreeLibrary(IntPtr hModule);
        #endregion
    }
}