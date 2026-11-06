using Allure;
using Xunit;
using System.Threading.Tasks;

namespace Allure.Xunit.v3.Tests.Samples.Documentation.DocumentationApi
{
    public class AsyncCallFromMethod
    {
        [Fact]
        public async Task TestMethod()
        {
            await AllureApi.SetDescriptionAsync("Test description", TestContext.Current.CancellationToken);
            await AllureApi.SetDescriptionHtmlAsync("<p>Test HTML</p>", TestContext.Current.CancellationToken);
            await AllureApi.SetPreconditionsAsync("Test preconditions", TestContext.Current.CancellationToken);
            await AllureApi.SetPreconditionsHtmlAsync("<p>Test preconditions HTML</p>", TestContext.Current.CancellationToken);
            await AllureApi.SetExpectedResultsAsync("Test expected results", TestContext.Current.CancellationToken);
            await AllureApi.SetExpectedResultsHtmlAsync("<p>Test expected results HTML</p>", TestContext.Current.CancellationToken);
        }
    }
}
