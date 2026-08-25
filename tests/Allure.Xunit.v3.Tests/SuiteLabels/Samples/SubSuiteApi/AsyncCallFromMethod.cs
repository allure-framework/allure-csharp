using Allure;
using Xunit;
using System.Threading.Tasks;

namespace Allure.Xunit.v3.Tests.Samples.SuiteLabels.SubSuiteApi
{
    public class AsyncCallFromMethod
    {
        [Fact]
        public async Task TestMethod()
        {
            await AllureApi.AddSubSuiteAsync("Sub Suite", TestContext.Current.CancellationToken);
        }
    }
}
