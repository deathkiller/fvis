using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/*
namespace fVis.Callbacks.Tests
{
    [TestClass]
    public class NativeOperatorCallbacksTests
    {
        [TestMethod]
        public void Add()
        {
            double x = 1;
            double y = 2;
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            double result = callbacks.Add(x, y);

            Assert.AreEqual(x + y, result);
        }

        [TestMethod]
        public void Subtract()
        {
            double x = 1;
            double y = 2;
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            double result = callbacks.Subtract(x, y);

            Assert.AreEqual(x - y, result);
        }

        [TestMethod]
        public void Multiply()
        {
            double x = 1;
            double y = 2;
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            double result = callbacks.Multiply(x, y);

            Assert.AreEqual(x * y, result);
        }

        [TestMethod]
        public void Divide()
        {
            double x = 1;
            double y = 2;
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            double result = callbacks.Divide(x, y);

            Assert.AreEqual(x / y, result);
        }

        [TestMethod]
        public void Pow()
        {
            double x = 1;
            double y = 2;
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            double result = callbacks.Pow(x, y);

            Assert.AreEqual(Math.Pow(x, y), result);
        }

        [TestMethod]
        public void Remainder()
        {
            double x = 1;
            double y = 2;
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            double result = callbacks.Remainder(x, y);

            Assert.AreEqual(x % y, result);
        }

        [TestMethod]
        public void Abs()
        {
            double x = -1;
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            double result = callbacks.Abs(x);

            Assert.AreEqual(Math.Abs(x), result);
        }

        [TestMethod]
        public void Sqrt()
        {
            double x = 1;
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            double result = callbacks.Sqrt(x);

            Assert.AreEqual(Math.Sqrt(x), result);
        }

        [TestMethod]
        public void Exp()
        {
            double x = 1;
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            double result = callbacks.Exp(x);

            Assert.AreEqual(Math.Exp(x), result);
        }

        [TestMethod]
        public void Ln()
        {
            double x = 1;
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            double result = callbacks.Ln(x);

            Assert.AreEqual(Math.Log(x), result);
        }

        [TestMethod]
        public void Log()
        {
            double x = 1;
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            double result = callbacks.Log(x);

            Assert.AreEqual(Math.Log10(x), result);
        }

        [TestMethod]
        public void Sin()
        {
            double x = 1;
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            double result = callbacks.Sin(x);

            Assert.AreEqual(Math.Sin(x), result);
        }

        [TestMethod]
        public void Cos()
        {
            double x = 1;
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            double result = callbacks.Cos(x);

            Assert.AreEqual(Math.Cos(x), result);
        }

        [TestMethod]
        public void Tan()
        {
            double x = 1;
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            double result = callbacks.Tan(x);

            Assert.AreEqual(Math.Tan(x), result);
        }

        [TestMethod]
        public void Asin()
        {
            double x = 1;
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            double result = callbacks.Asin(x);

            Assert.AreEqual(Math.Asin(x), result);
        }

        [TestMethod]
        public void Acos()
        {
            double x = 1;
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            double result = callbacks.Acos(x);

            Assert.AreEqual(Math.Acos(x), result);
        }

        [TestMethod]
        public void Atan()
        {
            double x = 1;
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            double result = callbacks.Atan(x);

            Assert.AreEqual(Math.Atan(x), result);
        }

        [TestMethod]
        public void Sinh()
        {
            double x = 1;
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            double result = callbacks.Sinh(x);

            Assert.AreEqual(Math.Sinh(x), result);
        }

        [TestMethod]
        public void Cosh()
        {
            double x = 1;
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            double result = callbacks.Cosh(x);

            Assert.AreEqual(Math.Cosh(x), result);
        }

        [TestMethod]
        public void Tanh()
        {
            double x = 1;
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            double result = callbacks.Tanh(x);

            Assert.AreEqual(Math.Tanh(x), result);
        }

        [TestMethod]
        public void Round()
        {
            double x = 1.5;
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            double result = callbacks.Round(x);

            Assert.AreEqual(Math.Round(x), result);
        }

        [TestMethod]
        public void Floor()
        {
            double x = 1.5;
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            double result = callbacks.Floor(x);

            Assert.AreEqual(Math.Floor(x), result);
        }

        [TestMethod]
        public void Ceil()
        {
            double x = 1.5;
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            double result = callbacks.Ceil(x);

            Assert.AreEqual(Math.Ceiling(x), result);
        }

        [TestMethod]
        public void ImplementationName()
        {
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            Assert.AreEqual("Sun libmcr v0.9", callbacks.ImplementationName);
        }

        [TestMethod]
        public void MissingCallbacks()
        {
            NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks("libmcr.dll");

            Assert.IsTrue(callbacks.MissingCallbacks.Contains("constant_e"));
            Assert.IsTrue(callbacks.MissingCallbacks.Contains("constant_pi"));

            Assert.IsTrue(callbacks.MissingCallbacks.Contains("operator_add"));
            Assert.IsTrue(callbacks.MissingCallbacks.Contains("operator_subtract"));
            Assert.IsTrue(callbacks.MissingCallbacks.Contains("operator_multiply"));
            Assert.IsTrue(callbacks.MissingCallbacks.Contains("operator_divide"));
            Assert.IsFalse(callbacks.MissingCallbacks.Contains("operator_pow"));
            Assert.IsTrue(callbacks.MissingCallbacks.Contains("operator_remainder"));

            Assert.IsTrue(callbacks.MissingCallbacks.Contains("operator_abs"));
            Assert.IsTrue(callbacks.MissingCallbacks.Contains("operator_sqrt"));
            Assert.IsFalse(callbacks.MissingCallbacks.Contains("operator_exp"));
            Assert.IsFalse(callbacks.MissingCallbacks.Contains("operator_ln"));
            Assert.IsTrue(callbacks.MissingCallbacks.Contains("operator_log"));
            Assert.IsFalse(callbacks.MissingCallbacks.Contains("operator_sin"));
            Assert.IsFalse(callbacks.MissingCallbacks.Contains("operator_cos"));
            Assert.IsFalse(callbacks.MissingCallbacks.Contains("operator_tan"));
            Assert.IsTrue(callbacks.MissingCallbacks.Contains("operator_asin"));
            Assert.IsTrue(callbacks.MissingCallbacks.Contains("operator_acos"));
            Assert.IsFalse(callbacks.MissingCallbacks.Contains("operator_atan"));
            Assert.IsTrue(callbacks.MissingCallbacks.Contains("operator_sinh"));
            Assert.IsTrue(callbacks.MissingCallbacks.Contains("operator_cosh"));
            Assert.IsTrue(callbacks.MissingCallbacks.Contains("operator_tanh"));

            Assert.IsTrue(callbacks.MissingCallbacks.Contains("operator_round"));
            Assert.IsTrue(callbacks.MissingCallbacks.Contains("operator_floor"));
            Assert.IsTrue(callbacks.MissingCallbacks.Contains("operator_ceil"));
        }
    }
}
*/