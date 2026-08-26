using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.BddLabels.StoryApi
{
    public class SyncCallFromMethod
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.AddStory("Foo");
        }
    }
}
