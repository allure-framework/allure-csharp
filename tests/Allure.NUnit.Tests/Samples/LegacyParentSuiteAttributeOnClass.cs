using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.LegacyParentSuiteAttributeOnClass
{
    [AllureNUnit]
    [AllureParentSuite("foo")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
