using Allure.Xunit.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.Samples.LegacyLabelAttributeOnClass
{
    [AllureLabel("baseClass", "foo")]
    public class BaseClass {}

    [AllureLabel("class", "bar")]
    public class TestsClass : BaseClass
    {
        [Fact]
        [AllureLabel("method", "baz")]
        public void TestMethod() { }
    }
}
