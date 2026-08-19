using Allure;
using Xunit;
using System.Threading.Tasks;

namespace Allure.Xunit.v3.Tests.Samples.BddLabels.EpicApi
{
    public class AsyncCallFromMethod
    {
        [Fact]
        public async Task TestMethod()
        {
            await AllureApi.AddEpicAsync("Foo", TestContext.Current.CancellationToken);
        }
    }
}
