using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.SuiteLabels.Samples.LegacySuiteAttributeOnMethod
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        [AllureSuite("foo")]
        public void TestMethod() { }
    }
}
