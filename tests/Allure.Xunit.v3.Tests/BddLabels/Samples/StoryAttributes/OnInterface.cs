using Xunit;
using Allure.Net.Commons.Attributes;

namespace Allure.Xunit.v3.Tests.Samples.StoryLabels.StoryAttributes
{
    [AllureStory("Foo")]
    public class IInterface { }

    public class OnInterface : IInterface
    {
        [Fact]
        public void TestMethod() { }
    }
}
