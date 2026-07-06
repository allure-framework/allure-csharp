using Xunit;
using Allure.Net.Commons.Attributes;

namespace Allure.Xunit.v3.Tests.Samples.EpicLabels.EpicAttributes
{
    public class OnTestMethod
    {
        [Fact]
        [AllureEpic("Foo")]
        public void TestMethod() { }
    }
}
