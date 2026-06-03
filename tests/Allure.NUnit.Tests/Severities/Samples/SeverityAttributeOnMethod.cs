using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Severities.Samples.SeverityAttributeOnMethod
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        [AllureSeverity(SeverityLevel.critical)]
        public void TestMethod() { }
    }
}
