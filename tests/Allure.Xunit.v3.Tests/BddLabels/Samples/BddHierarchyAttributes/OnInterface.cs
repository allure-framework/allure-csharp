using Xunit;
using Allure.Net.Commons.Attributes;

namespace Allure.Xunit.v3.Tests.Samples.BddLabels.BddHierarchyAttributes
{
    [AllureBddHierarchy("Foo", "Bar", "Baz")]
    public class IInterface { }

    public class OnInterface : IInterface
    {
        [Fact]
        public void TestMethod() { }
    }
}
