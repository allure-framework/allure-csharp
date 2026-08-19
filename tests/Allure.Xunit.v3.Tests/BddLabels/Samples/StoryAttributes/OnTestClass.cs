using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.StoryLabels.StoryAttributes
{
    [AllureStory("Foo")]
    public class OnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
