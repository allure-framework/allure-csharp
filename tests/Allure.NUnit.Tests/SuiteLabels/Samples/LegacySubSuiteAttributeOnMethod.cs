using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.SuiteLabels.Samples.LegacySubSuiteAttributeOnMethod
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        [AllureSubSuite("foo")]
        public void TestMethod() { }
    }
}
