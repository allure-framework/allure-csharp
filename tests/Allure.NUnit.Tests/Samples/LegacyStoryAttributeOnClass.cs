using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.LegacyStoryAttributeOnClass
{
    [AllureNUnit]
    [AllureStory("foo")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
