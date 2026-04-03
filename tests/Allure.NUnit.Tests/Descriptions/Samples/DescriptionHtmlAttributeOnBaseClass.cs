using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Descriptions.Samples.DescriptionHtmlAttributeOnBaseClass
{
    [AllureDescriptionHtml("Lorem Ipsum")]
    public class TestClassBase { }

    [AllureNUnit]
    public class TestsClass : TestClassBase
    {
        [Test]
        public void TestMethod() { }
    }
}
