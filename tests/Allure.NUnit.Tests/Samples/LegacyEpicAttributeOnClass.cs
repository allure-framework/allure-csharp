using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.EpicAttributeOnClass
{
    [AllureNUnit]
    [AllureEpic("foo")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}

