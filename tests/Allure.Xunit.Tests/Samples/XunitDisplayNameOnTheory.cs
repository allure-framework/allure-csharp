using Allure.Xunit.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.Samples.XunitDisplayNameOnTheory
{
    public class TestsClass
    {
        [Theory(DisplayName = "Lorem Ipsum")]
        [InlineData(1)]
        public void TestMethod(int foo) { }
    }
}
