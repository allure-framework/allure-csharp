using Allure;
using Xunit;
using System.Threading.Tasks;

namespace Allure.Xunit.v3.Tests.Samples.Descriptions.DescriptionApi
{
    public class AsyncCallFromMethod
    {
        [Fact]
        public async Task TestMethod()
        {
            await AllureApi.SetDescriptionAsync("Test description", TestContext.Current.CancellationToken);
            await AllureApi.SetDescriptionHtmlAsync("<p>Test HTML</p>", TestContext.Current.CancellationToken);
        }
    }
}
