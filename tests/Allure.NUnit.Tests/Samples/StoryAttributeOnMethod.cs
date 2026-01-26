using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.StoryAttributeOnMethod
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        [AllureStory("foo")]
        public void TestMethod() { }
    }
}

