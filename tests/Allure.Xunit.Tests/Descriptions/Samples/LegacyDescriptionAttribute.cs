using Allure.Xunit.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.Descriptions.Samples.LegacyDescriptionAttribute
{
    public class TestsClass
    {
        [Fact]
        [AllureDescription("Lorem Ipsum")]
        public void TestMethod() { }
    }
}
