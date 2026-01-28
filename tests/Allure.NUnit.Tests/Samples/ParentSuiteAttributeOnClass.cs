using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.ParentSuiteAttributeOnClass
{
    [AllureNUnit]
    [AllureParentSuite("foo")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
