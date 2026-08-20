using Allure;
using Xunit;
using System.Threading.Tasks;

namespace Allure.Xunit.v3.Tests.Samples.CustomLabels.LabelApi
{
    public class AsyncCallFromMethod
    {
        [Fact]
        public async Task TestMethod()
        {
            await AllureApi.AddLabelAsync("foo", "bar", TestContext.Current.CancellationToken);
        }
    }
}
