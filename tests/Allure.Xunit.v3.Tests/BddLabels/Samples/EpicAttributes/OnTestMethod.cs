using Xunit;
using Allure.Net.Commons.Attributes;

namespace Allure.Xunit.v3.Tests.EpicLabels.Samples.EpicAttributes
{
    public class OnTestMethod
    {
        [Fact]
        [AllureEpic("Foo")]
        public void TestMethod() { }
    }
}
