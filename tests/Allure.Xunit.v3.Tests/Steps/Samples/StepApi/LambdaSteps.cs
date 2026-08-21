using System.Threading.Tasks;
using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Steps.StepApi
{
    public class AllureApiSyncLambdaSteps
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.Step("step", context =>
            {
                context.SetName("AllureApi sync lambda");
                context.AddParameter("context", "works");
            });
        }
    }

    public class AllureApiAsyncLambdaSteps
    {
        [Fact]
        public async Task TestMethod()
        {
            var token = TestContext.Current.CancellationToken;
            await AllureApi.StepAsync(
                "step",
                async (context, cancellationToken) =>
                {
                    await context.SetNameAsync("AllureApi async lambda", cancellationToken);
                    await context.AddParameterAsync("context", "works", cancellationToken);
                },
                token
            );
        }
    }

    public class AllureInProcessApiSyncLambdaSteps
    {
        [Fact]
        public void TestMethod()
        {
            AllureInProcessApi.Step("step", context =>
            {
                context.SetName("AllureInProcessApi sync lambda");
                context.AddParameter("context", "works");
            });
        }
    }

    public class AllureInProcessApiAsyncLambdaSteps
    {
        [Fact]
        public async Task TestMethod()
        {
            var token = TestContext.Current.CancellationToken;
            await AllureInProcessApi.StepAsync(
                "step",
                async (context, cancellationToken) =>
                {
                    await context.SetNameAsync(
                        "AllureInProcessApi async lambda",
                        cancellationToken
                    );
                    await context.AddParameterAsync("context", "works", cancellationToken);
                },
                token
            );
        }
    }
}
