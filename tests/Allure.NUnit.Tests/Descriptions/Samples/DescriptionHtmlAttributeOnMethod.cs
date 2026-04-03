using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Descriptions.Samples.DescriptionHtmlAttributeOnMethod
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        [AllureDescriptionHtml("Lorem Ipsum")]
        public void TestMethod() { }
    }
}
