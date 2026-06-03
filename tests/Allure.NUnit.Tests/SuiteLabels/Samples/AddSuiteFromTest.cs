using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.SuiteLabels.Samples.AddSuiteFromTest
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        public void TestMethod()
        {
            AllureApi.AddSuite("foo");
        }
    }
}
