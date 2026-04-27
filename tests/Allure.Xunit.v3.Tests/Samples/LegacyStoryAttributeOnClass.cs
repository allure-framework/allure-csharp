using Allure.Xunit.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.Samples.LegacyStoryAttributeOnClass
{
    [AllureStory("foo")]
    public class TestsClass
    {
        [Fact]
        public void TestMethod() { }
    }
}



