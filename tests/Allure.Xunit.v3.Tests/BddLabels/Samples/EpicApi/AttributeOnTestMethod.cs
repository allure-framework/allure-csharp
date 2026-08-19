using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.BddLabels.EpicApi
{
    public class AttributeOnTestMethod
    {
        [Fact]
        [AllureEpic("Foo")]
        public void TestMethod() { }
    }
}
