using Allure.Xunit.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.Samples.LegacyStoryAttributeOnMethod
{
    public class TestsClass
    {
        [Fact]
        [AllureStory("foo")]
        public void TestMethod() { }
    }
}
