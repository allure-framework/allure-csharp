using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.StoryLabels.StoryAttributes
{
    [AllureStory("Foo")]
    public class BaseClass { }

    public class OnBaseClass : BaseClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
