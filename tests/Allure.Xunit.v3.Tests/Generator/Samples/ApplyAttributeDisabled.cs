using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Generator.ApplyAttributeDisabled
{
    public class TestClass
    {
        [Theory]
        [InlineData("value-1")]
        [AllureTag("tag-from-attribute")]
        public void TestMethod(string argument)
        {
            _ = argument;
        }
    }
}
