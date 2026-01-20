using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.OneNUnitTestCaseWithOneParameter
{
    [AllureNUnit]
    public class TestsClass
    {
        [TestCase("bar")]
        public void TestMethod(string foo) { }
    }
}

