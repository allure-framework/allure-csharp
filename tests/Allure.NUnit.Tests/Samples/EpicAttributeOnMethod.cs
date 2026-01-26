using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.EpicAttributeOnMethod
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        [AllureEpic("foo")]
        public void TestMethod() { }
    }
}

