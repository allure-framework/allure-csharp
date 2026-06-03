using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.SuiteLabels.Samples.SubSuiteAttributeOnClass
{
    [AllureNUnit]
    [AllureSubSuite("foo")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
