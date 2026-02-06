using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.Samples.StoryAttributeOnInterface
{
    [AllureStory("foo")]
    public interface IMetadataInterface {}

    public class TestsClass : IMetadataInterface
    {
        [Fact]
        public void TestMethod() { }
    }
}
