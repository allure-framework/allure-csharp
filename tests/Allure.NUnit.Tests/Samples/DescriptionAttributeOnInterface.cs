using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.DescriptionAttributeOnInterface
{
    [AllureDescription("Lorem Ipsum")]
    public interface IMetadataInterface { }

    [AllureNUnit]
    public class TestsClass : IMetadataInterface
    {
        [Test]
        public void TestMethod() { }
    }
}
