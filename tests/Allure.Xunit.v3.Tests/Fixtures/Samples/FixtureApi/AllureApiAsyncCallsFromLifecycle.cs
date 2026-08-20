using System.Threading.Tasks;
using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Fixtures.FixtureApi
{
    public class AllureApiAsyncCallsFromLifecycle : IAsyncLifetime
    {
        public async ValueTask InitializeAsync()
        {
            await AllureApi.SetUpAsync(
                "setup",
                async (context, token) =>
                {
                    await context.SetNameAsync("AllureApi async setup", token);
                    await context.AddParameterAsync("context", "works", token);
                },
                TestContext.Current.CancellationToken
            );
        }

        [Fact]
        public void TestMethod() { }

        public async ValueTask DisposeAsync()
        {
            await AllureApi.TearDownAsync(
                "teardown",
                async (context, token) =>
                {
                    await context.SetNameAsync("AllureApi async teardown", token);
                    await context.AddParameterAsync("context", "works", token);
                },
                TestContext.Current.CancellationToken
            );
        }
    }
}
