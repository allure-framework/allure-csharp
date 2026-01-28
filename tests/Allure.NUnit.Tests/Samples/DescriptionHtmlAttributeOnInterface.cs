using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.DescriptionHtmlAttributeOnInterface
{
    [AllureDescriptionHtml("Lorem Ipsum")]
    public interface IMetadataInterface { }

    [AllureNUnit]
    public class TestsClass : IMetadataInterface
    {
        [Test]
        public void TestMethod() { }
    }
}
