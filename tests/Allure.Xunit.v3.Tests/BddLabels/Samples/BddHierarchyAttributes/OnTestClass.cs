using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.BddLabels.BddHierarchyAttributes
{
    [AllureBddHierarchy("Foo", "Bar", "Baz")]
    public class OnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
