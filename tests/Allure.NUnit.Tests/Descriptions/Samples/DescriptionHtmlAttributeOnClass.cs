using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Descriptions.Samples.DescriptionHtmlAttributeOnClass
{
    [AllureNUnit]
    [AllureDescriptionHtml("Lorem Ipsum")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
