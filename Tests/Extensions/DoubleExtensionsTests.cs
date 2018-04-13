using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fVis.Extensions.Tests
{
    [TestClass]
    public class DoubleExtensionsTests
    {
        [TestMethod]
        public void SubtractDivideLosslessTest()
        {
            int x = 50;
            long subtract = 20;
            double divide = 2;

            double result = x.SubtractDivideLossless(subtract, divide);
            double expected = (x - subtract) / divide;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void MultiplyAddLosslessToIntTest()
        {
            double x = -5.0;
            double multiply = 123.0;
            long add = -1000000;

            int result = x.MultiplyAddLosslessToInt(multiply, add);
            Assert.AreEqual((int)((x * multiply) + add), result);
        }

        [TestMethod]
        public void NegMultiplyAddLosslessTest()
        {
            double x = 5.0;
            double multiply = 2.0;
            long add = 10;

            double result = x.NegMultiplyAddLossless(multiply, add);
            double expected = (-x * multiply) + add;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NegMultiplyAddLosslessToIntTest()
        {
            double x = -5.0;
            double multiply = 123.0;
            long add = -1000000;

            int result = x.NegMultiplyAddLosslessToInt(multiply, add);
            Assert.AreEqual((int)((-x * multiply) + add), result);
        }
    }
}