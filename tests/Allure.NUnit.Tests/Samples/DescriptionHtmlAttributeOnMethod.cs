using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.DescriptionHtmlAttributeOnMethod
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        [AllureDescriptionHtml("Lorem Ipsum")]
        public void TestMethod() { }
    }
}
