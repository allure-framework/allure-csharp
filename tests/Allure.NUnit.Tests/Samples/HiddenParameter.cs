using System;
using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        public void TestMethod()
        {
            AllureApi.AddTestParameter("foo", "bar", ParameterMode.Hidden);
        }
    }
}
