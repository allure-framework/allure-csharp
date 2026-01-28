using Allure.Net.Commons.Attributes;
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
