using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.BddLabels.Samples.AddStoryFromTearDown
{
    [AllureNUnit]
    public class TestsClass
    {
        [TearDown]
        public void TearDown()
        {
            AllureApi.AddStory("foo");
        }

        [Test]
        public void TestMethod() { }
    }
}
