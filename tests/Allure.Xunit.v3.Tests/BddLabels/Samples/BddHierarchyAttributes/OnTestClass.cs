using Xunit;
using Allure.Net.Commons.Attributes;

namespace Allure.Xunit.v3.Tests.BddLabels.Samples.BddHierarchyAttributes
{
    [AllureBddHierarchy("Foo", "Bar", "Baz")]
    public class OnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
