using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.BddLabels.Samples.LegacyEpicAttributeOnClass
{
    [AllureNUnit]
    [AllureEpic("foo")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
