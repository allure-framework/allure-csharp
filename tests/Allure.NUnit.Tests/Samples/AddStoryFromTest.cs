using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.AddStoryFromTest
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

