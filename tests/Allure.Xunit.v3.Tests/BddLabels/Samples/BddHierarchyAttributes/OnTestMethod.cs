using Xunit;
using Allure.Net.Commons.Attributes;

namespace Allure.Xunit.v3.Tests.Samples.BddLabels.BddHierarchyAttributes
{
    public class OnTestMethod
    {
        [Fact]
        [AllureBddHierarchy("Foo", "Bar", "Baz")]
        public void TestMethod() { }
    }
}
