using Allure;
using Xunit;
using System.Threading.Tasks;

namespace Allure.Xunit.v3.Tests.Samples.BddLabels.FeatureApi
{
    public class AsyncCallFromMethod
    {
        [Fact]
        public async Task TestMethod()
        {
            await AllureApi.AddFeatureAsync("Foo", TestContext.Current.CancellationToken);
        }
    }
}
