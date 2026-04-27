using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.Samples.NameAttributeOnClass
{
    [AllureName("Lorem Ipsum")]
    public class TestsClass
    {
        [Fact]
        public void TestMethod() { }
    }
}



