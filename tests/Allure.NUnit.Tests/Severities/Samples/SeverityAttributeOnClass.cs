using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Severities.Samples.SeverityAttributeOnClass
{
    [AllureNUnit]
    [AllureSeverity(SeverityLevel.critical)]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
