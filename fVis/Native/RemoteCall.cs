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

namespace fVis.Native
{
    public enum RemoteCallType
    {
        Close,
        Invoke,
        InvokeWithRef,
        ProcedureExists,
        ResolveStringAnsi
    }

    [Serializable]
    public class RemoteCall
    {
        public Type Delegate { get; set; }
        public string Name { get; set; }
        public object[] Parameters { get; set; }
        public RemoteCallType Type { get; set; }
    }

    [Serializable]
    public class RemoteCallResult
    {
        public object Result { get; set; }
        public object[] Parameters { get; set; }
        public Exception Exception { get; set; }
    }
}