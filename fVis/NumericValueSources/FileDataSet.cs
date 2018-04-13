using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using fVis.Utils;

namespace fVis.NumericValueSources
{
    /// <summary>
    /// Provides mechanism to use file data set as mathematical function
    /// </summary>
    public unsafe class FileDataSet : Disposable, INumericValueSource
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct Value
        {
            public double X;
            public double Y;
        }

        private MemoryMappedFile file;
        private MemoryMappedViewAccessor view;
        private readonly Value* values;
        private readonly ulong count;

        public FileDataSet(string path, ulong offset, ulong count)
        {
            file = MemoryMappedFile.CreateFromFile(path, FileMode.Open);
            view = file.CreateViewAccessor();

            byte* ptr = null;
            view.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
            values = (Value*)(ptr + offset);

            this.count = count;
        }

        protected override void Dispose(bool disposing)
        {
            if (view != null) {
                view.SafeMemoryMappedViewHandle.ReleasePointer();
                view.Dispose();
                view = null;
            }

            if (file != null) {
                file.Dispose();
                file = null;
            }
        }

        public double Evaluate(double x)
        {
            if (view != null && x >= values[0].X && x <= values[count - 1].X) {
                ulong min = 0;
                ulong max = count - 1;
                while (min <= max) {
                    ulong mid = (min + max) / 2;
                    if (min >= max) {
                        if (x < values[mid].X && mid > 0) {
                            return values[mid - 1].Y;
                        }
                        return values[mid].Y;
                    }

                    if (values[mid].X == x) {
                        return values[mid].Y;
                    } else if (values[mid].X > x) {
                        max = mid - 1;
                    } else {
                        min = mid + 1;
                    }
                }
            }

            return double.NaN;
        }
    }
}