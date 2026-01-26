using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.ParentSuiteAttributeOnMethod
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        [AllureParentSuite("foo")]
        public void TestMethod() { }
    }
}

