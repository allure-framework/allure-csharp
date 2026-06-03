using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Descriptions.Samples.DescriptionAttributeOnMethod
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        [AllureDescription("Lorem Ipsum")]
        public void TestMethod() { }
    }
}
