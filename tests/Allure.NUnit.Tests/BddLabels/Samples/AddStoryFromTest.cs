using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.BddLabels.Samples.AddStoryFromTest
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        public void TestMethod()
        {
            AllureApi.AddStory("foo");
        }
    }
}
