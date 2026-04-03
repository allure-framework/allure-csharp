using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.BddLabels.Samples.LegacyEpicAttributeOnBaseClass
{
    [AllureEpic("foo")]
    public class BaseClass {}

    [AllureNUnit]
    public class TestsClass : BaseClass
    {
        [Test]
        public void TestMethod() { }
    }
}
