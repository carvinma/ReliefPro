using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReliefProMain;

namespace ReliefProUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            double fact = Algorithm.GetQ(1, 1, 1);
            double target=3.6;
            Assert.AreEqual(target, fact);
        }
    }
}
