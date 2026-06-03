using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.BddLabels.Samples.BddHierarchyAttributeOnInterface
{
    [AllureBddHierarchy("foo", "bar", "baz")]
    public interface IMetadataInterface {}

    [AllureNUnit]
    public class TestsClass : IMetadataInterface
    {
        [Test]
        public void TestMethod() { }
    }
}
