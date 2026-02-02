using Allure.Xunit.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.Samples.LegacyEpicAttributeOnClass
{
    [AllureEpic("foo")]
    public class TestsClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
