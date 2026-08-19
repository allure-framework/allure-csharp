using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.EpicLabels.EpicAttributes
{
    public class OnTestMethod
    {
        [Fact]
        [AllureEpic("Foo")]
        public void TestMethod() { }
    }
}
