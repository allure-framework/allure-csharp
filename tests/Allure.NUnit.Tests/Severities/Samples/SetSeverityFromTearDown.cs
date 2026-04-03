using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Severities.Samples.SetSeverityFromTearDown
{
    [AllureNUnit]
    public class TestsClass
    {
        [TearDown]
        public void TearDown()
        {
            AllureApi.SetSeverity(SeverityLevel.critical);
        }

        [Test]
        public void TestMethod() { }
    }
}
