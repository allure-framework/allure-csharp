using System.Threading.Tasks;
using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Names.NameApi
{
    public class SyncSetTestName
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.SetTestName("Sync test name");
        }
    }

    public class AsyncSetTestName
    {
        [Fact]
        public async Task TestMethod()
        {
            await AllureApi.SetTestNameAsync(
                "Async test name",
                TestContext.Current.CancellationToken
            );
        }
    }
}
