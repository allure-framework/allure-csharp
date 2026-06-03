using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.SuiteLabels.Samples.AddSubSuiteFromTest
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        public void TestMethod()
        {
            AllureApi.AddSubSuite("foo");
        }
    }
}
