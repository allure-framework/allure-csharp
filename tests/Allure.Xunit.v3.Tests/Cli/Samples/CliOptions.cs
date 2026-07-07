using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Cli.CliOptions
{
    public class TestClass
    {
        [Fact]
        public void FirstTest() { }

        [Fact]
        public void SecondTest() { }

        [Fact]
        [AllureId(3001)]
        public void AllureIdTest() { }

        [Theory]
        [InlineData("foo")]
        [InlineData("bar")]
        public void ParameterizedTheory(string value)
        {
            Assert.NotEmpty(value);
        }

        [Theory]
        [InlineData("baz")]
        public void GenericTheory<T>(T value)
        {
            Assert.NotNull(value);
        }
    }
}
