using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Names.NameAttributeOnMethod
{
    public class TestClass
    {
        [Fact]
        [AllureName("Lorem Ipsum")]
        public void TestMethod() { }
    }
}
