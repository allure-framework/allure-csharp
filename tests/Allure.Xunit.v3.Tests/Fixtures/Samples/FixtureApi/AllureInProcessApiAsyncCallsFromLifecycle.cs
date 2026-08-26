using System.Threading.Tasks;
using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Fixtures.FixtureApi
{
    public class AllureInProcessApiAsyncCallsFromLifecycle : IAsyncLifetime
    {
        public async ValueTask InitializeAsync()
        {
            await AllureInProcessApi.SetUpAsync(
                "setup",
                async (context, token) =>
                {
                    await context.SetNameAsync("AllureInProcessApi async setup", token);
                    await context.AddParameterAsync("context", "works", token);
                },
                TestContext.Current.CancellationToken
            );
        }

        [Fact]
        public void TestMethod() { }

        public async ValueTask DisposeAsync()
        {
            await AllureInProcessApi.TearDownAsync(
                "teardown",
                async (context, token) =>
                {
                    await context.SetNameAsync("AllureInProcessApi async teardown", token);
                    await context.AddParameterAsync("context", "works", token);
                },
                TestContext.Current.CancellationToken
            );
        }
    }
}
