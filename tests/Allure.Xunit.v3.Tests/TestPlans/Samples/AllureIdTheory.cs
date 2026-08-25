using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.TestPlans.AllureIdTheory
{
    public class TestClass
    {
        [Theory]
        [InlineData("foo")]
        [InlineData("bar")]
        [AllureId(3005)]
        public void SelectedTheory(string value)
        {
            Assert.NotEmpty(value);
        }

        [Fact]
        [AllureId(3999)]
        public void UnselectedTest() { }
    }
}
