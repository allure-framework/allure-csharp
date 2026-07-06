using Xunit;
using Allure.Net.Commons.Attributes;

namespace Allure.Xunit.v3.Tests.Samples.EpicLabels.EpicAttributes
{
    [AllureEpic("Foo")]
    public class OnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
