using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.SetSeverityFromSetUp
{
    [AllureNUnit]
    public class TestsClass
    {
        [SetUp]
        public void SetUp()
        {
            AllureApi.SetSeverity(SeverityLevel.critical);
        }

        [Test]
        public void TestMethod() { }
    }
}
