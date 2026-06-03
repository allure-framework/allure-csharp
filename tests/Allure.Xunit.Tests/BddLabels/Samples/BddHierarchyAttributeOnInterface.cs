using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.BddLabels.Samples.BddHierarchyAttributeOnInterface
{
    [AllureBddHierarchy("foo", "bar", "baz")]
    public interface IMetadataInterface {}

    public class TestsClass : IMetadataInterface
    {
        [Fact]
        public void TestMethod() { }
    }
}
