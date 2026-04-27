using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.Samples.StoryAttributeOnClass
{
    [AllureStory("foo")]
    public class TestsClass
    {
        [Fact]
        public void TestMethod() { }
    }
}



