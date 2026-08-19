using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.StoryLabels.StoryAttributes
{
    public class OnTestMethod
    {
        [Fact]
        [AllureStory("Foo")]
        public void TestMethod() { }
    }
}
