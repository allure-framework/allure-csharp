using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.StoryAttributeOnClass
{
    [AllureNUnit]
    [AllureStory("foo")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}

