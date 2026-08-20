using System;
using System.Threading.Tasks;
using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Fixtures.FixtureApi
{
    public class NestedAllureApiAsyncFixtures
    {
        [Fact]
        public async Task TestMethod()
        {
            var token = TestContext.Current.CancellationToken;
            await AllureApi.SetUpAsync(
                "Outer AllureApi async fixture",
                async () =>
                {
                    try
                    {
                        await AllureApi.SetUpAsync("Inner fixture", () => Task.CompletedTask, token);
                    }
                    catch (InvalidOperationException) { }
                },
                token
            );
        }
    }
}
