using System.Threading.Tasks;
using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Fixtures.FixtureApi
{
    public class AttributeOnAsyncLifetime : IAsyncLifetime
    {
        [AllureSetUp("InitializeAsync setup")]
        public async ValueTask InitializeAsync()
        {
            await Task.Yield();
        }

        [Fact]
        public void TestMethod() { }

        [AllureTearDown("IAsyncLifetime teardown")]
        public async ValueTask DisposeAsync()
        {
            await Task.Yield();
        }
    }
}
