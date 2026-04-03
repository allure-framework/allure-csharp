using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.BddLabels.Samples.EpicAttributeOnClass
{
    [AllureNUnit]
    [AllureEpic("foo")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
