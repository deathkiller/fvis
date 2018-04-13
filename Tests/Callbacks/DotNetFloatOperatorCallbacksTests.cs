using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fVis.Callbacks.Tests
{
    [TestClass()]
    public class DotNetFloatOperatorCallbacksTests
    {
        [TestMethod]
        public void Add()
        {
            double x = 1;
            double y = 2;
            DotNetFloatOperatorCallbacks callbacks = new DotNetFloatOperatorCallbacks();

            double result = callbacks.Add(x, y);

            Assert.AreEqual((float)(x + y), result);
        }

        [TestMethod]
        public void Subtract()
        {
            double x = 1;
            double y = 2;
            DotNetFloatOperatorCallbacks callbacks = new DotNetFloatOperatorCallbacks();

            double result = callbacks.Subtract(x, y);

            Assert.AreEqual((float)(x - y), result);
        }

        [TestMethod]
        public void Multiply()
        {
            double x = 1;
            double y = 2;
            DotNetFloatOperatorCallbacks callbacks = new DotNetFloatOperatorCallbacks();

            double result = callbacks.Multiply(x, y);

            Assert.AreEqual((float)(x * y), result);
        }

        [TestMethod]
        public void Divide()
        {
            double x = 1;
            double y = 2;
            DotNetFloatOperatorCallbacks callbacks = new DotNetFloatOperatorCallbacks();

            double result = callbacks.Divide(x, y);

            Assert.AreEqual((float)(x / y), result);
        }

        [TestMethod]
        public void Pow()
        {
            double x = 1;
            double y = 2;
            DotNetFloatOperatorCallbacks callbacks = new DotNetFloatOperatorCallbacks();

            double result = callbacks.Pow(x, y);

            Assert.AreEqual((float)Math.Pow(x, y), result);
        }

        [TestMethod]
        public void Remainder()
        {
            double x = 1;
            double y = 2;
            DotNetFloatOperatorCallbacks callbacks = new DotNetFloatOperatorCallbacks();

            double result = callbacks.Remainder(x, y);

            Assert.AreEqual((float)(x % y), result);
        }

        [TestMethod]
        public void Abs()
        {
            double x = -1;
            DotNetFloatOperatorCallbacks callbacks = new DotNetFloatOperatorCallbacks();

            double result = callbacks.Abs(x);

            Assert.AreEqual((float)Math.Abs(x), result);
        }

        [TestMethod]
        public void Sqrt()
        {
            double x = 1;
            DotNetFloatOperatorCallbacks callbacks = new DotNetFloatOperatorCallbacks();

            double result = callbacks.Sqrt(x);

            Assert.AreEqual((float)Math.Sqrt(x), result);
        }

        [TestMethod]
        public void Exp()
        {
            double x = 1;
            DotNetFloatOperatorCallbacks callbacks = new DotNetFloatOperatorCallbacks();

            double result = callbacks.Exp(x);

            Assert.AreEqual((float)Math.Exp(x), result);
        }

        [TestMethod]
        public void Ln()
        {
            double x = 1;
            DotNetFloatOperatorCallbacks callbacks = new DotNetFloatOperatorCallbacks();

            double result = callbacks.Ln(x);

            Assert.AreEqual((float)Math.Log(x), result);
        }

        [TestMethod]
        public void Log()
        {
            double x = 1;
            DotNetFloatOperatorCallbacks callbacks = new DotNetFloatOperatorCallbacks();

            double result = callbacks.Log(x);

            Assert.AreEqual((float)Math.Log10(x), result);
        }

        [TestMethod]
        public void Sin()
        {
            double x = 1;
            DotNetFloatOperatorCallbacks callbacks = new DotNetFloatOperatorCallbacks();

            double result = callbacks.Sin(x);

            Assert.AreEqual((float)Math.Sin(x), result);
        }

        [TestMethod]
        public void Cos()
        {
            double x = 1;
            DotNetFloatOperatorCallbacks callbacks = new DotNetFloatOperatorCallbacks();

            double result = callbacks.Cos(x);

            Assert.AreEqual((float)Math.Cos(x), result);
        }

        [TestMethod]
        public void Tan()
        {
            double x = 1;
            DotNetFloatOperatorCallbacks callbacks = new DotNetFloatOperatorCallbacks();

            double result = callbacks.Tan(x);

            Assert.AreEqual((float)Math.Tan(x), result);
        }

        [TestMethod]
        public void Asin()
        {
            double x = 1;
            DotNetFloatOperatorCallbacks callbacks = new DotNetFloatOperatorCallbacks();

            double result = callbacks.Asin(x);

            Assert.AreEqual((float)Math.Asin(x), result);
        }

        [TestMethod]
        public void Acos()
        {
            double x = 1;
            DotNetFloatOperatorCallbacks callbacks = new DotNetFloatOperatorCallbacks();

            double result = callbacks.Acos(x);

            Assert.AreEqual((float)Math.Acos(x), result);
        }

        [TestMethod]
        public void Atan()
        {
            double x = 1;
            DotNetFloatOperatorCallbacks callbacks = new DotNetFloatOperatorCallbacks();

            double result = callbacks.Atan(x);

            Assert.AreEqual((float)Math.Atan(x), result);
        }

        [TestMethod]
        public void Sinh()
        {
            double x = 1;
            DotNetFloatOperatorCallbacks callbacks = new DotNetFloatOperatorCallbacks();

            double result = callbacks.Sinh(x);

            Assert.AreEqual((float)Math.Sinh(x), result);
        }

        [TestMethod]
        public void Cosh()
        {
            double x = 1;
            DotNetFloatOperatorCallbacks callbacks = new DotNetFloatOperatorCallbacks();

            double result = callbacks.Cosh(x);

            Assert.AreEqual((float)Math.Cosh(x), result);
        }

        [TestMethod]
        public void Tanh()
        {
            double x = 1;
            DotNetFloatOperatorCallbacks callbacks = new DotNetFloatOperatorCallbacks();

            double result = callbacks.Tanh(x);

            Assert.AreEqual((float)Math.Tanh(x), result);
        }

        [TestMethod]
        public void Round()
        {
            double x = 1.5;
            DotNetFloatOperatorCallbacks callbacks = new DotNetFloatOperatorCallbacks();

            double result = callbacks.Round(x);

            Assert.AreEqual((float)Math.Round(x), result);
        }

        [TestMethod]
        public void Floor()
        {
            double x = 1.5;
            DotNetFloatOperatorCallbacks callbacks = new DotNetFloatOperatorCallbacks();

            double result = callbacks.Floor(x);

            Assert.AreEqual((float)Math.Floor(x), result);
        }

        [TestMethod]
        public void Ceil()
        {
            double x = 1.5;
            DotNetFloatOperatorCallbacks callbacks = new DotNetFloatOperatorCallbacks();

            double result = callbacks.Ceil(x);

            Assert.AreEqual((float)Math.Ceiling(x), result);
        }
    }
}