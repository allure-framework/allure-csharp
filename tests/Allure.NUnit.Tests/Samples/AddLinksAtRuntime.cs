using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.AddLinksAtRuntime
{
    [AllureNUnit]
    public class TestsClass
    {
        [SetUp]
        public void SetUp()
        {
            AllureApi.AddLink("url-1");
        }

        [Test]
        public void TestMethod()
        {
            AllureApi.AddLink("name-2", "url-2");
            AllureApi.AddLinks(
                new Link(){ url = "url-3", name = "name-3", type = "type-3" }
            );
        }

        [TearDown]
        public void TearDown()
        {
            AllureApi.AddLink("name-4", "type-4", "url-4");
        }
    }
}
