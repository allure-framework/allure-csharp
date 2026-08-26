using Allure;
using Xunit;
using System.Threading.Tasks;

namespace Allure.Xunit.v3.Tests.Samples.Tags.TagApi
{
    public class AsyncCallFromMethod
    {
        [Fact]
        public async Task TestMethod()
        {
            await AllureApi.AddTagsAsync(
                ["foo", "bar", "baz"],
                TestContext.Current.CancellationToken
            );
        }
    }
}
