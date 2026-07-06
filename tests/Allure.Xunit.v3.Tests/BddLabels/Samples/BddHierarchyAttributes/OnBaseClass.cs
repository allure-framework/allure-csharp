using Xunit;
using Allure.Net.Commons.Attributes;

namespace Allure.Xunit.v3.Tests.Samples.BddLabels.BddHierarchyAttributes
{
    [AllureBddHierarchy("Foo", "Bar", "Baz")]
    public class BaseClass { }

    public class OnBaseClass : BaseClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
