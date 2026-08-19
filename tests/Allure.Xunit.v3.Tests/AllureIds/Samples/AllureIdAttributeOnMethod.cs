using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.AllureIds.AllureIdAttributeOnMethod
{
    public class TestsClass
    {
        [Fact]
        [AllureId(1001)]
        public void TestMethod() { }
    }
}
