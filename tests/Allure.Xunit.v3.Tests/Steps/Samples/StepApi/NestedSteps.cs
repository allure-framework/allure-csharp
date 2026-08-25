using System.Threading.Tasks;
using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Steps.StepApi
{
    public class NestedAllureApiSyncSteps
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.Step(
                "Outer sync step",
                () => AllureApi.Step("Inner sync step")
            );
        }
    }

    public class NestedAllureApiAsyncSteps
    {
        [Fact]
        public async Task TestMethod()
        {
            var token = TestContext.Current.CancellationToken;
            await AllureApi.StepAsync(
                "Outer async step",
                () => AllureApi.StepAsync("Inner async step", token),
                token
            );
        }
    }
}
