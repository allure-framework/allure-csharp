using System.Threading.Tasks;
using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Names.NameApi
{
    public class SyncSetFixtureName
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.SetUp(
                "Original fixture",
                () => AllureApi.SetFixtureName("Sync fixture name")
            );
        }
    }

    public class AsyncSetFixtureName
    {
        [Fact]
        public async Task TestMethod()
        {
            var token = TestContext.Current.CancellationToken;
            await AllureApi.SetUpAsync(
                "Original fixture",
                () => AllureApi.SetFixtureNameAsync("Async fixture name", token),
                token
            );
        }
    }
}
