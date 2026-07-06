using Xunit;
using Allure.Net.Commons.Attributes;

namespace Allure.Xunit.Tests.Samples.BddHierarchyAttributes
{
    public class OnTestMethod
    {
        [Fact]
        [AllureBddHierarchy("Foo", "Bar", "Baz")]
        public void TestMethod() { }
    }
}
