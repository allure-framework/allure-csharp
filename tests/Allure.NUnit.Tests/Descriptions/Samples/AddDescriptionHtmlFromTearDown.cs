using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Descriptions.Samples.AddDescriptionHtmlFromTearDown
{
    [AllureNUnit]
    public class TestsClass
    {
        [TearDown]
        public void TearDown()
        {
            AllureApi.SetDescriptionHtml("Lorem Ipsum");
        }

        [Test]
        public void TestMethod() { }
    }
}
