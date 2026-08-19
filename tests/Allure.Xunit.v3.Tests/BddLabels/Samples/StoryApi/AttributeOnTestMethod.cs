using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.BddLabels.StoryApi
{
    public class AttributeOnTestMethod
    {
        [Fact]
        [AllureStory("Foo")]
        public void TestMethod() { }
    }
}
