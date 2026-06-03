using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.SuiteLabels.Samples.LegacySuiteAttributeOnClass
{
    [AllureNUnit]
    [AllureSuite("foo")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
