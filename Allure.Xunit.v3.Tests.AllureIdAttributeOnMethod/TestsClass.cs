using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.Samples.AllureIdAttributeOnMethod
{
    public class TestsClass
    {
        [Fact]
        [AllureId(1001)]
        public void TestMethod() { }
    }
}