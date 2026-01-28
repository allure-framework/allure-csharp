using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.SuiteAttributeOnClass
{
    [AllureNUnit]
    [AllureSuite("foo")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
