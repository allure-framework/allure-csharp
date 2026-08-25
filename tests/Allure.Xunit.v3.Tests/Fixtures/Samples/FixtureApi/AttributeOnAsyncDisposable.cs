using System;
using System.Threading.Tasks;
using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Fixtures.FixtureApi
{
    public sealed class AttributeOnAsyncDisposable : IAsyncDisposable
    {
        [Fact]
        public void TestMethod() { }

        [AllureTearDown("IAsyncDisposable teardown")]
        public async ValueTask DisposeAsync()
        {
            await Task.Yield();
        }
    }
}
