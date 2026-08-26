using System.Threading.Tasks;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Lifecycle.AsyncTests
{
    public class AsyncTestClass1
    {
        [Fact]
        public async Task AsyncFact1()
        {
            await Task.Yield();
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("bar")]
        public async Task AsyncTheory1(string value)
        {
            await Task.Yield();
            Assert.NotEmpty(value);
        }
    }

    public class AsyncTestClass2
    {
        [Fact]
        public async Task AsyncFact2()
        {
            await Task.Yield();
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("bar")]
        public async Task AsyncTheory2(string value)
        {
            await Task.Yield();
            Assert.NotEmpty(value);
        }
    }
}
