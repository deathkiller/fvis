using fVis.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fVis.NumericValueSources.Tests
{
    [TestClass]
    public class MemoryDataSetTests
    {
        [TestMethod]
        public void Evaluate_Exact()
        {
            MemoryDataSet.Value[] values = {
                new MemoryDataSet.Value(1, 1),
                new MemoryDataSet.Value(2, 5),
                new MemoryDataSet.Value(4, 6),
                new MemoryDataSet.Value(10, 1),
                new MemoryDataSet.Value(15, 0)
            };

            MemoryDataSet sds = new MemoryDataSet(values);

            for (int i = 0; i < values.Length; i++) {
                double y = sds.Evaluate(values[i].X);
                Assert.AreEqual(values[i].Y, y);
            }
        }

        [TestMethod]
        public void Evaluate_Between()
        {
            MemoryDataSet.Value[] values = {
                new MemoryDataSet.Value(1, 2),
                new MemoryDataSet.Value(2, 4),
                new MemoryDataSet.Value(3, 6)
            };

            MemoryDataSet sds = new MemoryDataSet(values);

            for (int i = 1; i < values.Length; i++) {
                double y = sds.Evaluate((values[i - 1].X + values[i].X) / 2);
                Assert.AreEqual(values[i - 1].Y, y);
            }
        }

        [TestMethod]
        public void Evaluate_Overflow()
        {
            MemoryDataSet.Value[] values = {
                new MemoryDataSet.Value(1, 2),
                new MemoryDataSet.Value(2, 4)
            };

            MemoryDataSet sds = new MemoryDataSet(values);

            double result1 = sds.Evaluate(values[0].X - 0.00000001f);
            Assert.AreEqual(double.NaN, result1);

            double result2 = sds.Evaluate(values[values.Length - 1].X + 0.00000001f);
            Assert.AreEqual(double.NaN, result2);
        }

        [TestMethod]
        public void Evaluate_Exact_2()
        {
            BigArray<MemoryDataSet.Value> values = new BigArray<MemoryDataSet.Value>(5);
            values[0] = new MemoryDataSet.Value(1, 1);
            values[1] = new MemoryDataSet.Value(2, 5);
            values[2] = new MemoryDataSet.Value(4, 6);
            values[3] = new MemoryDataSet.Value(10, 1);
            values[4] = new MemoryDataSet.Value(15, 0);

            MemoryDataSet sds = new MemoryDataSet(values);

            for (ulong i = 0; i < values.Length; i++) {
                double y = sds.Evaluate(values[i].X);
                Assert.AreEqual(values[i].Y, y);
            }
        }

        [TestMethod]
        public void Evaluate_Between_2()
        {
            BigArray<MemoryDataSet.Value> values = new BigArray<MemoryDataSet.Value>(3);
            values[0] = new MemoryDataSet.Value(1, 2);
            values[1] = new MemoryDataSet.Value(2, 4);
            values[2] = new MemoryDataSet.Value(3, 6);

            MemoryDataSet sds = new MemoryDataSet(values);

            for (uint i = 1; i < values.Length; i++) {
                double y = sds.Evaluate((values[i - 1].X + values[i].X) / 2);
                Assert.AreEqual(values[i - 1].Y, y);
            }
        }

        [TestMethod]
        public void Evaluate_Overflow_2()
        {
            BigArray<MemoryDataSet.Value> values = new BigArray<MemoryDataSet.Value>(2);
            values[0] = new MemoryDataSet.Value(1, 2);
            values[1] = new MemoryDataSet.Value(2, 4);

            MemoryDataSet sds = new MemoryDataSet(values);

            double result1 = sds.Evaluate(values[0].X - 0.00000001f);
            Assert.AreEqual(double.NaN, result1);

            double result2 = sds.Evaluate(values[values.Length - 1].X + 0.00000001f);
            Assert.AreEqual(double.NaN, result2);
        }
    }
}