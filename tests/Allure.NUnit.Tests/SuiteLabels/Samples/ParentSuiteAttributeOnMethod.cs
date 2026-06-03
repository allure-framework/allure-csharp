using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.SuiteLabels.Samples.ParentSuiteAttributeOnMethod
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        [AllureParentSuite("foo")]
        public void TestMethod() { }
    }
}
