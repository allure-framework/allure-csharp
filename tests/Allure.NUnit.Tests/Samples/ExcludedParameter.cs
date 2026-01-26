using System;
using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.ExcludedParameter
{
    [AllureNUnit]
    public class TestsClass
    {
        [TestCase(0)]
        [TestCase(0)]
        public void TestMethod(int _)
        {
            AllureApi.AddTestParameter("timestamp", DateTime.Now, true);
        }
    }
}

