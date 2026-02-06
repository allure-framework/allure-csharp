using Allure.Xunit.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.Samples.LegacyTagAttributes
{
    [AllureTag("foo")]
    public class BaseClass { }

    [AllureTag("bar")]
    public class TestsClass : BaseClass
    {
        [Fact]
        [AllureTag("baz", "qux")]
        public void TestMethod() { }
    }
}
