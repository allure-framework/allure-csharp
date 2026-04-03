using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Descriptions.Samples.AddDescriptionHtmlFromTest
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        public void TestMethod()
        {
            AllureApi.SetDescriptionHtml("Lorem Ipsum");
        }
    }
}
