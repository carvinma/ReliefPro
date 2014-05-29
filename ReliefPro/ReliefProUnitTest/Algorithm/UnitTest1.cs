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
            double result = Algorithm.GetQ(1, 1, 1);
            double fact=3.6;
            Assert.AreEqual(result,fact);
        }
    }
}
