using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.BddLabels.Samples.BddHierarchyAttributeOnClass
{
    [AllureBddHierarchy("foo", "bar", "baz")]
    public class TestsClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
