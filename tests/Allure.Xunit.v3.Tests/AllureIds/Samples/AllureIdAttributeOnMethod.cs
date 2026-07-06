using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.AllureIds.Samples.AllureIdAttributeOnMethod
{
    public class TestsClass
    {
        [Fact]
        [AllureId(1001)]
        public void TestMethod() { }
    }
}
