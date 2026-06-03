using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Descriptions.Samples.DescriptionAttributeOnClass
{
    [AllureNUnit]
    [AllureDescription("Lorem Ipsum")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
