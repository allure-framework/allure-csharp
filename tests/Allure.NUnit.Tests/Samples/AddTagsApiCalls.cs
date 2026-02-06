using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.AddTagsApiCalls
{
    [AllureNUnit]
    public class TestsClass
    {
        [SetUp]
        public void SetUp()
        {
            AllureApi.AddTags("foo");
        }

        [Test]
        public void TestMethod()
        {
            AllureApi.AddTags("bar", "baz");
        }

        [TearDown]
        public void TearDown()
        {
            AllureApi.AddTags("qux");
        }
    }
}
