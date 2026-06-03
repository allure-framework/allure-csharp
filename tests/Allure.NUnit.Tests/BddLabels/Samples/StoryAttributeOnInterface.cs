using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.BddLabels.Samples.StoryAttributeOnInterface
{
    [AllureStory("foo")]
    public interface IMetadataInterface {}

    [AllureNUnit]
    public class TestsClass : IMetadataInterface
    {
        [Test]
        public void TestMethod() { }
    }
}
