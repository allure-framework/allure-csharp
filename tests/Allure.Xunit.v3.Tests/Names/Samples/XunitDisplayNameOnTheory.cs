using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Names.XunitDisplayNameOnTheory
{
    public class TestClass
    {
        [Theory(DisplayName = "Lorem Ipsum")]
        [InlineData("foo")]
        public void TestMethod(string value)
        {
            _ = value;
        }
    }
}
