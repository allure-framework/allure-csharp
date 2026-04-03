using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.SuiteLabels.Samples.LegacySubSuiteAttributeOnClass
{
    [AllureNUnit]
    [AllureSubSuite("foo")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
