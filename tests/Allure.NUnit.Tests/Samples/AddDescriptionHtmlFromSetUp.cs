using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.AddDescriptionHtmlFromSetUp
{
    [AllureNUnit]
    public class TestsClass
    {
        [SetUp]
        public void SetUp()
        {
            AllureApi.SetDescriptionHtml("Lorem Ipsum");
        }

        [Test]
        public void TestMethod() { }
    }
}
