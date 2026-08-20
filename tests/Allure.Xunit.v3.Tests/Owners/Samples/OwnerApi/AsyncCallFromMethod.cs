using Allure;
using Xunit;
using System.Threading.Tasks;

namespace Allure.Xunit.v3.Tests.Samples.Owners.OwnerApi
{
    public class AsyncCallFromMethod
    {
        [Fact]
        public async Task TestMethod()
        {
            await AllureApi.SetOwnerAsync("John Doe", TestContext.Current.CancellationToken);
        }
    }
}
