using Allure.Net.Commons;
using Xunit;

namespace Allure.Xunit.Tests.Samples.AddStoryFromTest
{
    public class TestsClass
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.AddStory("foo");
        }
    }
}



