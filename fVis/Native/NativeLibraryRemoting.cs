//
// Uses parts of LegacyWrapper released under MIT license.
// https://github.com/CodefoundryDE/LegacyWrapper
//
// Copyright(c) 2015 Franz Wimmer
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using fVis.Utils;

namespace fVis.Native
{
    /// <summary>
    /// Provides mechanism to load native libraries with different bitness and call its functions
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    public class NativeLibraryRemoting : Disposable
    {
        private readonly BinaryFormatter formatter;
        private readonly NamedPipeClientStream pipe;
        private readonly Process process;

        public NativeLibraryRemoting(string libraryName)
        {
            // Resolve path to x86 server
            string path = Assembly.GetEntryAssembly().Location;
            string ext = Path.GetExtension(path);

            // Last 4 chars of filename contains bitness information
            string bitness = path.Substring(path.Length - (ext.Length + 4), 4);
            if (bitness == ".x86") {
                throw new InvalidOperationException();
            }

            path = path.Remove(path.Length - 4) + ".x86" + ext;

            // Generate new token
            string token = Guid.NewGuid().ToString("B");

            // Pass library name and token to child process (server)
            process = Process.Start(path, "\"" + libraryName.Replace("\"", "\"\"") + "\" " + token);

            formatter = new BinaryFormatter();

            pipe = new NamedPipeClientStream(".", token, PipeDirection.InOut);
            pipe.Connect();
            pipe.ReadMode = PipeTransmissionMode.Message;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                Close();
                pipe.Dispose();
                process.Dispose();
            }
        }

        protected virtual void Close()
        {
            RemoteCall info = new RemoteCall {
                Type = RemoteCallType.Close
            };

            lock (pipe) {
                try {
                    formatter.Serialize(pipe, info);
                } catch {
                    // This means the server eventually crashed and doesn't need a clean shutdown anyways
                }
            }

            if (pipe.IsConnected) {
                pipe.Close();
            }
        }

        /// <summary>
        /// Invokes function specified by name
        /// </summary>
        /// <typeparam name="T">Delegate type</typeparam>
        /// <typeparam name="TResult">Type of return value</typeparam>
        /// <param name="procedureName">Name of function</param>
        /// <param name="args">Arguments</param>
        /// <returns>Return value</returns>
        public TResult Invoke<T, TResult>(string procedureName, params object[] args) where T : class
        {
            RemoteCall info = new RemoteCall {
                Type = RemoteCallType.Invoke,
                Name = procedureName,
                Parameters = args,
                Delegate = typeof(T)
            };

            RemoteCallResult callResult;
            lock (pipe) {
                // Write request to the server
                formatter.Serialize(pipe, info);

                // Receive result from the server
                callResult = (RemoteCallResult)formatter.Deserialize(pipe);
            }

            if (callResult.Exception != null) {
                throw callResult.Exception;
            }

            return (TResult)callResult.Result;
        }

        /// <summary>
        /// Invokes function specified by name, changed arguments are forwarded back to parent process
        /// </summary>
        /// <typeparam name="T">Delegate type</typeparam>
        /// <typeparam name="TResult">Type of return value</typeparam>
        /// <param name="procedureName">Name of function</param>
        /// <param name="args">Arguments</param>
        /// <returns>Return value</returns>
        public TResult Invoke<T, TResult>(string procedureName, ref object[] args) where T : class
        {
            RemoteCall info = new RemoteCall {
                Type = RemoteCallType.InvokeWithRef,
                Name = procedureName,
                Parameters = args,
                Delegate = typeof(T)
            };

            RemoteCallResult callResult;
            lock (pipe) {
                // Write request to the server
                formatter.Serialize(pipe, info);

                // Receive result from the server
                callResult = (RemoteCallResult)formatter.Deserialize(pipe);
            }

            if (callResult.Exception != null) {
                throw callResult.Exception;
            }

            // Exchange ref params
            if (args.Length != callResult.Parameters.Length) {
                throw new InvalidDataException("Returned parameters differ in length from passed parameters");
            }

            args = callResult.Parameters;

            return (TResult)callResult.Result;
        }

        /// <summary>
        /// Check if specified function exists
        /// </summary>
        /// <param name="procedureName">Name of function</param>
        /// <returns>Returns true if exists; false, otherwise</returns>
        public bool ProcedureExists(string procedureName)
        {
            RemoteCall info = new RemoteCall {
                Type = RemoteCallType.ProcedureExists,
                Name = procedureName
            };

            RemoteCallResult callResult;
            lock (pipe) {
                // Write request to the server
                formatter.Serialize(pipe, info);

                // Receive result from the server
                callResult = (RemoteCallResult)formatter.Deserialize(pipe);
            }

            if (callResult.Exception != null) {
                throw callResult.Exception;
            }

            return (bool)callResult.Result;
        }

        /// <summary>
        /// Read string specified by pointer from remoting process
        /// </summary>
        /// <param name="ptr">Pointer to string</param>
        /// <returns>String</returns>
        public string ResolveStringAnsi(IntPtr ptr)
        {
            RemoteCall info = new RemoteCall {
                Type = RemoteCallType.ResolveStringAnsi,
                Parameters = new object[] { ptr }
            };

            RemoteCallResult callResult;
            lock (pipe) {
                // Write request to the server
                formatter.Serialize(pipe, info);

                // Receive result from the server
                callResult = (RemoteCallResult)formatter.Deserialize(pipe);
            }

            return (string)callResult.Result;
        }
    }
}