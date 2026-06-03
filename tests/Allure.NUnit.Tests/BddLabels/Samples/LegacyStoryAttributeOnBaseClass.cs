using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.BddLabels.Samples.LegacyStoryAttributeOnBaseClass
{
    [AllureStory("foo")]
    public class BaseClass {}

    [AllureNUnit]
    public class TestsClass : BaseClass
    {
        [Test]
        public void TestMethod() { }
    }
}
