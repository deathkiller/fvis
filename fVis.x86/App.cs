using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using fVis.Native;

namespace fVis.x86
{
    [SuppressUnmanagedCodeSecurity]
    internal class App
    {
        private static readonly BinaryFormatter formatter = new BinaryFormatter();

        private static readonly Dictionary<string, Delegate> delegateCache = new Dictionary<string, Delegate>();

        [HandleProcessCorruptedStateExceptions]
        private static void Main(string[] args)
        {
            if (args.Length != 2) {
                if (!Environment.Is64BitOperatingSystem) {
                    // Load 'Any CPU' version on 32-bit system
                    string path = Assembly.GetExecutingAssembly().Location;
                    string ext = Path.GetExtension(path);
                    path = path.Remove(path.Length - (ext.Length + 4), 4);

                    string argsString = "";
                    for (int i = 0; i < args.Length; i++) {
                        argsString = "\"" + args[i].Replace("\"", "\"\"") + "\" ";
                    }

                    Process p = Process.Start(path, argsString);
                    p?.Dispose();
                    return;
                }

                fVis.App.Main(args);
                return;
            }

            string libraryName = args[0];
            string token = args[1];

            try {
                using (NamedPipeServerStream pipe = new NamedPipeServerStream(token, PipeDirection.InOut, 1, PipeTransmissionMode.Message)) {
                    pipe.WaitForConnection();

                    try {
                        RemoteCall data = (RemoteCall)formatter.Deserialize(pipe);

                        using (NativeLibrary library = new NativeLibrary(libraryName)) {
                            while (data.Type != RemoteCallType.Close) {
                                switch (data.Type) {
                                    case RemoteCallType.Invoke:
                                        Invoke(data, pipe, library);
                                        break;
                                    case RemoteCallType.InvokeWithRef:
                                        InvokeWithRef(data, pipe, library);
                                        break;
                                    case RemoteCallType.ProcedureExists:
                                        ProcedureExists(data, pipe, library);
                                        break;
                                    case RemoteCallType.ResolveStringAnsi:
                                        ResolveStringAnsi(data, pipe);
                                        break;
                                }

                                data = (RemoteCall)formatter.Deserialize(pipe);
                            }
                        }
                    } catch (Exception e) {
                        WriteExceptionToClient(pipe, e);
                    }
                }
            } catch (IOException) {
                // TODO: Probably broken pipe
            }
        }

        [HandleProcessCorruptedStateExceptions]
        private static void Invoke(RemoteCall data, Stream pipe, NativeLibrary library)
        {
            try {
                Delegate method;
                if (!delegateCache.TryGetValue(data.Name, out method)) {
                    delegateCache.Add(data.Name, method = library.Resolve(data.Name, data.Delegate));
                }

                // Invoke requested method
                object result = method.DynamicInvoke(data.Parameters);

                RemoteCallResult callResult = new RemoteCallResult {
                    Result = result
                };

                // Write result back to the client
                formatter.Serialize(pipe, callResult);
            } catch (Exception e) {
                WriteExceptionToClient(pipe, e);
            }
        }

        [HandleProcessCorruptedStateExceptions]
        private static void InvokeWithRef(RemoteCall data, Stream pipe, NativeLibrary library)
        {
            try {
                Delegate method;
                if (!delegateCache.TryGetValue(data.Name, out method)) {
                    delegateCache.Add(data.Name, method = library.Resolve(data.Name, data.Delegate));
                }

                // Invoke requested method
                object result = method.DynamicInvoke(data.Parameters);

                RemoteCallResult callResult = new RemoteCallResult {
                    Result = result,
                    Parameters = data.Parameters
                };

                // Write result back to the client
                formatter.Serialize(pipe, callResult);
            } catch (Exception e) {
                WriteExceptionToClient(pipe, e);
            }
        }

        private static void ProcedureExists(RemoteCall data, Stream pipe, NativeLibrary library)
        {
            try {
                // Check if procedure with given name exists in target library
                bool exists = library.ProcedureExists(data.Name);

                RemoteCallResult callResult = new RemoteCallResult {
                    Result = exists
                };

                // Write result back to the client
                formatter.Serialize(pipe, callResult);
            } catch (Exception e) {
                WriteExceptionToClient(pipe, e);
            }
        }

        private static void ResolveStringAnsi(RemoteCall data, Stream pipe)
        {
            try {
                // Read Ansi string from memory of current process
                string stringAnsi = Marshal.PtrToStringAnsi((IntPtr)data.Parameters[0]);

                RemoteCallResult callResult = new RemoteCallResult {
                    Result = stringAnsi
                };

                // Write result back to the client
                formatter.Serialize(pipe, callResult);
            } catch (Exception e) {
                WriteExceptionToClient(pipe, e);
            }
        }

        private static void WriteExceptionToClient(Stream pipe, Exception e)
        {
            formatter.Serialize(pipe, new RemoteCallResult {
                Exception = new InvalidOperationException("An error occured while calling a native library function", e)
            });
        }
    }
}