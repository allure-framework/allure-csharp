using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.Names.Samples.NameAttributeOnMethod
{
    public class TestsClass
    {
        [Fact]
        [AllureName("Lorem Ipsum")]
        public void TestMethod() { }
    }
}
