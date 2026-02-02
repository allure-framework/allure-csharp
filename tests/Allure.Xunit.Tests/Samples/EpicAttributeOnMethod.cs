using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.Samples.EpicAttributeOnMethod
{
    public class TestsClass
    {
        [Fact]
        [AllureEpic("foo")]
        public void TestMethod() { }
    }
}
