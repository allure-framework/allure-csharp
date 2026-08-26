using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.BddLabels.StoryApi
{
    [AllureStory("Foo")]
    public class IInterface { }

    public class AttributeOnInterface : IInterface
    {
        [Fact]
        public void TestMethod() { }
    }
}
