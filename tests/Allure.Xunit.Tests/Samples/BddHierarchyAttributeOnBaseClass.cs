using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.Samples.BddHierarchyAttributeOnBaseClass
{
    [AllureBddHierarchy("foo", "bar", "baz")]
    public class BaseClass {}

    public class TestsClass : BaseClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
