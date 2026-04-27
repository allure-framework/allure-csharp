using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.Samples.StoryAttributeOnMethod
{
    public class TestsClass
    {
        [Fact]
        [AllureStory("foo")]
        public void TestMethod() { }
    }
}



