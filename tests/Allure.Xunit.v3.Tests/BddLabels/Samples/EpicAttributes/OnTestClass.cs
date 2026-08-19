using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.EpicLabels.EpicAttributes
{
    [AllureEpic("Foo")]
    public class OnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
