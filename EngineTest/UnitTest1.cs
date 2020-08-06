using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var sum = 10 + 20;
            Assert.AreEqual(30, sum);
        }
    }
}
