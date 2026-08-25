using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.BddLabels.EpicApi
{
    [AllureEpic("Foo")]
    public class AttributeOnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
