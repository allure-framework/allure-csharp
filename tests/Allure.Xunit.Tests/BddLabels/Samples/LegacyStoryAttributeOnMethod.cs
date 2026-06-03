using Allure.Xunit.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.BddLabels.Samples.LegacyStoryAttributeOnMethod
{
    public class TestsClass
    {
        [Fact]
        [AllureStory("foo")]
        public void TestMethod() { }
    }
}
