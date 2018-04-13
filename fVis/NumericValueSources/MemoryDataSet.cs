using fVis.Utils;

namespace fVis.NumericValueSources
{
    /// <summary>
    /// Provides mechanism to use in-memory data set as mathematical function
    /// </summary>
    public class MemoryDataSet : INumericValueSource
    {
        public struct Value
        {
            public double X;
            public double Y;

            public Value(double x, double y)
            {
                X = x;
                Y = y;
            }
        }

        private readonly BigArray<Value> values;

        public MemoryDataSet(Value[] values)
        {
            this.values = new BigArray<Value>((ulong)values.Length);

            for (int i = 0; i < values.Length; i++) {
                this.values[(ulong)i] = values[i];
            }
        }

        public MemoryDataSet(BigArray<Value> values)
        {
            this.values = values;
        }

        public double Evaluate(double x)
        {
            if (x >= values[0].X && x <= values[values.Length - 1].X) {
                ulong min = 0;
                ulong max = values.Length - 1;
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