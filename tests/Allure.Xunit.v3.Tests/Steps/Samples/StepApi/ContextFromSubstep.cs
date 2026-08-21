using System.Threading.Tasks;
using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Steps.StepApi
{
    public class SyncContextOperationFromSubstep
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.Step("parent", parentContext =>
            {
                AllureApi.Step("Sync child", () =>
                {
                    parentContext.SetName("Sync parent renamed from child");
                    parentContext.AddParameter("substep context", "works");
                });
            });
        }
    }

    public class AsyncContextOperationFromSubstep
    {
        [Fact]
        public async Task TestMethod()
        {
            var token = TestContext.Current.CancellationToken;
            await AllureInProcessApi.StepAsync(
                "parent",
                async (parentContext, cancellationToken) =>
                {
                    await AllureInProcessApi.StepAsync(
                        "Async child",
                        async (_, childToken) =>
                        {
                            await parentContext.SetNameAsync(
                                "Async parent renamed from child",
                                childToken
                            );
                            await parentContext.AddParameterAsync(
                                "substep context",
                                "works",
                                childToken
                            );
                        },
                        cancellationToken
                    );
                },
                token
            );
        }
    }
}
