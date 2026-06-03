using Allure.Xunit.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.BddLabels.Samples.LegacyEpicAttributeOnBaseClass
{
    [AllureEpic("foo")]
    public class BaseClass {}

    public class TestsClass : BaseClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
