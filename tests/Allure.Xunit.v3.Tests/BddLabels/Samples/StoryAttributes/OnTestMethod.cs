using Xunit;
using Allure.Net.Commons.Attributes;

namespace Allure.Xunit.v3.Tests.StoryLabels.Samples.StoryAttributes
{
    public class OnTestMethod
    {
        [Fact]
        [AllureStory("Foo")]
        public void TestMethod() { }
    }
}
