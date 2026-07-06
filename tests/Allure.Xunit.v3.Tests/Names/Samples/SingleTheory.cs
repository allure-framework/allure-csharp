using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Names.SingleTheory
{
    public class TestClass
    {
        [Theory]
        [InlineData("foo")]
        public void TestMethod(string value)
        {
            _ = value;
        }
    }
}
