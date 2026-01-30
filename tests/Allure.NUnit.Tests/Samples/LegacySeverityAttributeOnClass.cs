using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.LegacySeverityAttributeOnClass
{
    [AllureNUnit]
    [AllureSeverity(SeverityLevel.critical)]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
