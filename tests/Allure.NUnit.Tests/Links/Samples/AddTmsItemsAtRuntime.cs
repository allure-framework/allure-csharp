using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Links.Samples.AddTmsItemsAtRuntime
{
    [AllureNUnit]
    public class TestsClass
    {
        [SetUp]
        public void SetUp()
        {
            AllureApi.AddTmsItem("url-1");
        }

        [Test]
        public void TestMethod()
        {
            AllureApi.AddTmsItem("name-2", "url-2");
        }

        [TearDown]
        public void TearDown()
        {
            AllureApi.AddTmsItem("name-3", "url-3");
        }
    }
}
