using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.BddLabels.Samples.StoryAttributeOnClass
{
    [AllureNUnit]
    [AllureStory("foo")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
