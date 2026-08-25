using Allure;
using Xunit;
using System.Threading.Tasks;

namespace Allure.Xunit.v3.Tests.Samples.SuiteLabels.ParentSuiteApi
{
    public class AsyncCallFromMethod
    {
        [Fact]
        public async Task TestMethod()
        {
            await AllureApi.AddParentSuiteAsync("Parent Suite", TestContext.Current.CancellationToken);
        }
    }
}
