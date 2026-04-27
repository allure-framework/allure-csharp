using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.Samples.BddHierarchyAttributeOnMethod
{
    public class TestsClass
    {
        [Fact]
        [AllureBddHierarchy("foo", "bar", "baz")]
        public void TestMethod() { }
    }
}



