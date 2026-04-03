using Allure.Xunit.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.BddLabels.Samples.LegacyEpicAttributeOnMethod
{
    public class TestsClass
    {
        [Fact]
        [AllureEpic("foo")]
        public void TestMethod() { }
    }
}
