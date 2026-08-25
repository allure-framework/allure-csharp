using Allure;
using Xunit;
using System.Threading.Tasks;

namespace Allure.Xunit.v3.Tests.Samples.BddLabels.StoryApi
{
    public class AsyncCallFromMethod
    {
        [Fact]
        public async Task TestMethod()
        {
            await AllureApi.AddStoryAsync("Foo", TestContext.Current.CancellationToken);
        }
    }
}
