using System;
using System.Threading.Tasks;
using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Fixtures.FixtureApi
{
    public class NestedAllureInProcessApiAsyncFixtures
    {
        [Fact]
        public async Task TestMethod()
        {
            var token = TestContext.Current.CancellationToken;
            await AllureInProcessApi.SetUpAsync(
                "Outer AllureInProcessApi async fixture",
                async (_, _) =>
                {
                    try
                    {
                        await AllureInProcessApi.SetUpAsync(
                            "Inner fixture",
                            _ => Task.CompletedTask,
                            token
                        );
                    }
                    catch (InvalidOperationException) { }
                },
                token
            );
        }
    }
}
