using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Descriptions.Samples.LegacyDescriptionAttributeHtml
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        [AllureDescription("Lorem Ipsum", true)]
        public void TestMethod() { }
    }
}
