using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fVis.Extensions.Tests
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void LimitSize_ShorterThanNeeded()
        {
            const string input = "Shorter";
            const int maxSize = 20;

            string result = input.LimitSize(maxSize);

            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void LimitSize_LargerThanNeeded()
        {
            const string input = "Larger";
            const int maxSize = 4;

            string result = input.LimitSize(maxSize);

            Assert.AreEqual("Larg…", result);
        }
    }
}