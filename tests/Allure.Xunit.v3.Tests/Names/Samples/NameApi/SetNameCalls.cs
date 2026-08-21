using System.Threading.Tasks;
using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Names.NameApi
{
    public class SyncSetNameOnTest
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.SetName("Sync test name via SetName");
        }
    }

    public class AsyncSetNameOnTest
    {
        [Fact]
        public async Task TestMethod()
        {
            await AllureApi.SetNameAsync(
                "Async test name via SetName",
                TestContext.Current.CancellationToken
            );
        }
    }

    public class SyncSetNameOnFixture
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.SetUp(
                "Original fixture",
                () => AllureApi.SetName("Sync fixture name via SetName")
            );
        }
    }

    public class AsyncSetNameOnFixture
    {
        [Fact]
        public async Task TestMethod()
        {
            var token = TestContext.Current.CancellationToken;
            await AllureApi.SetUpAsync(
                "Original fixture",
                () => AllureApi.SetNameAsync("Async fixture name via SetName", token),
                token
            );
        }
    }

    public class SyncSetNameOnStep
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.SetUp(
                "Original fixture",
                () => AllureApi.Step(
                    "Original step",
                    () => AllureApi.SetName("Sync step name via SetName")
                )
            );
        }
    }

    public class AsyncSetNameOnStep
    {
        [Fact]
        public async Task TestMethod()
        {
            var token = TestContext.Current.CancellationToken;
            await AllureApi.SetUpAsync(
                "Original fixture",
                () => AllureApi.StepAsync(
                    "Original step",
                    () => AllureApi.SetNameAsync("Async step name via SetName", token),
                    token
                ),
                token
            );
        }
    }
}
