using Xunit;
using Allure.Net.Commons.Attributes;

namespace Allure.Xunit.Tests.Samples.StoryAttributes
{
    [AllureStory("Foo")]
    public class OnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
