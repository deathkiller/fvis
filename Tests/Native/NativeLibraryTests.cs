using System;
using fVis.Callbacks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fVis.Native.Tests
{
    [TestClass]
    public class NativeLibraryTests
    {
        [TestMethod]
        public void Resolve_NotExists()
        {
            NativeLibrary library = new NativeLibrary("libmcr.dll");

            OperatorCallbacks.OperatorFunction result = library.Resolve<OperatorCallbacks.OperatorFunction>("_not_exists_");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Resolve_NotExists_2()
        {
            NativeLibrary library = new NativeLibrary("libmcr.dll");

            Delegate result = library.Resolve("_not_exists_", typeof(OperatorCallbacks.OperatorFunction));

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Resolve_Pow()
        {
            NativeLibrary library = new NativeLibrary("libmcr.dll");

            OperatorCallbacks.OperatorFunction pow = library.Resolve<OperatorCallbacks.OperatorFunction>("operator_pow");
            double result = pow(2, 3);

            Assert.AreEqual(Math.Pow(2, 3), result);
        }

        [TestMethod]
        public void Resolve_Pow_2()
        {
            NativeLibrary library = new NativeLibrary("libmcr.dll");

            Delegate pow = library.Resolve("operator_pow", typeof(OperatorCallbacks.OperatorFunction));
            double result = (double)pow.DynamicInvoke(new object[] { 2.0, 3.0 });

            Assert.AreEqual(Math.Pow(2, 3), result);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void Resolve_NotDelegate()
        {
            NativeLibrary library = new NativeLibrary("libmcr.dll");

            NativeLibrary pow = library.Resolve<NativeLibrary>("operator_pow");
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void Resolve_NotDelegate_2()
        {
            NativeLibrary library = new NativeLibrary("libmcr.dll");

            Delegate pow = library.Resolve("operator_pow", typeof(NativeLibrary));
        }


        [TestMethod]
        public void ProcedureExists()
        {
            NativeLibrary library = new NativeLibrary("libmcr.dll");

            bool result = library.ProcedureExists("operator_pow");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ProcedureExists_2()
        {
            NativeLibrary library = new NativeLibrary("libmcr.dll");

            bool result = library.ProcedureExists("_not_exists_");

            Assert.IsFalse(result);
        }
    }
}