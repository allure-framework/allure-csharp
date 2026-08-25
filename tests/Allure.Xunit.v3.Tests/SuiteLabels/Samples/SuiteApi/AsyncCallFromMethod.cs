using Allure;
using Xunit;
using System.Threading.Tasks;

namespace Allure.Xunit.v3.Tests.Samples.SuiteLabels.SuiteApi
{
    public class AsyncCallFromMethod
    {
        [Fact]
        public async Task TestMethod()
        {
            await AllureApi.AddSuiteAsync("Suite", TestContext.Current.CancellationToken);
        }
    }
}
