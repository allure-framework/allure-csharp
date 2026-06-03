using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.BddLabels.Samples.AddStoryFromSetUp
{
    [AllureNUnit]
    public class TestsClass
    {
        [SetUp]
        public void SetUp()
        {
            AllureApi.AddStory("foo");
        }

        [Test]
        public void TestMethod() { }
    }
}
