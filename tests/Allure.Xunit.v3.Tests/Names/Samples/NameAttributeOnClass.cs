using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Names.NameAttributeOnClass
{
    [AllureName("Lorem Ipsum")]
    public class TestClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
