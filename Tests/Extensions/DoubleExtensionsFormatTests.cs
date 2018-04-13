using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fVis.Extensions.Tests
{
    [TestClass]
    public class DoubleExtensionsFormatTests
    {
        [TestMethod]
        public void ToBits_Zero()
        {
            const double input = 0.0;

            string result = input.ToBits();

            Assert.AreEqual("0 00000000000 0000000000000000000000000000000000000000000000000000", result);
        }

        [TestMethod]
        public void ToBits_PositiveIntegral()
        {
            const double input = 50.0;

            string result = input.ToBits();

            Assert.AreEqual("0 10000000100 1001000000000000000000000000000000000000000000000000", result);
        }

        [TestMethod]
        public void ToBits_PositiveDecimal()
        {
            const double input = 0.003;

            string result = input.ToBits();

            Assert.AreEqual("0 01111110110 1000100100110111010010111100011010100111111011111010", result);
        }

        [TestMethod]
        public void ToBits_NaN()
        {
            const double input = double.NaN;

            string result = input.ToBits();

            Assert.AreEqual("1 11111111111 1000000000000000000000000000000000000000000000000000", result);
        }

        [TestMethod]
        public void ToBits_PositiveInfinity()
        {
            const double input = double.PositiveInfinity;

            string result = input.ToBits();

            Assert.AreEqual("0 11111111111 0000000000000000000000000000000000000000000000000000", result);
        }

        [TestMethod]
        public void ToBits_NegativeInfinity()
        {
            const double input = double.NegativeInfinity;

            string result = input.ToBits();

            Assert.AreEqual("1 11111111111 0000000000000000000000000000000000000000000000000000", result);
        }

        [TestMethod]
        public void ToBits_Epsilon()
        {
            const double input = double.Epsilon;

            string result = input.ToBits();

            Assert.AreEqual("0 00000000000 0000000000000000000000000000000000000000000000000001", result);
        }

        [TestMethod]
        public void ToExactString_Zero()
        {
            const double input = 0.0;

            string result = input.ToExactString();

            Assert.AreEqual(" 0", result);
        }

        [TestMethod]
        public void ToExactString_PositiveDecimal()
        {
            const double input = +0.0001050000000000000043541559247017858069739304482936859130859375;

            string result = input.ToExactString();

            Assert.AreEqual("+0.0001050000000000000043541559247017858069739304482936859130859375", result);
        }

        [TestMethod]
        public void ToExactString_MinusOne()
        {
            string result = (-1.0).ToExactString();
            Assert.AreEqual("-1", result);
        }

        [TestMethod]
        public void ToExactString_NegativeIntegral()
        {
            string result = (-3001000.0).ToExactString();
            Assert.AreEqual("-3001000", result);
        }

        [TestMethod]
        public void ToExactString_PositiveIntegral()
        {
            string result = (+12300.001229999999850406311452388763427734375).ToExactString();
            Assert.AreEqual("+12300.001229999999850406311452388763427734375", result);
        }

        [TestMethod]
        public void ToExactString_NaN()
        {
            const double input = double.NaN;

            string result = input.ToExactString();

            Assert.AreEqual("NaN", result);
        }

        [TestMethod]
        public void ToExactString_PositiveInfinity()
        {
            const double input = double.PositiveInfinity;

            string result = input.ToExactString();

            Assert.AreEqual("+∞", result);
        }

        [TestMethod]
        public void ToExactString_NegativeInfinity()
        {
            const double input = double.NegativeInfinity;

            string result = input.ToExactString();

            Assert.AreEqual("-∞", result);
        }
    }
}