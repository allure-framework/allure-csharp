using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Tags.TagApi
{
    public class SyncCallFromMethod
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.AddTags("foo", "bar", "baz");
        }
    }
}
