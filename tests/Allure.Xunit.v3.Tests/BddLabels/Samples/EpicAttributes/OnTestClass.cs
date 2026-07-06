using Xunit;
using Allure.Net.Commons.Attributes;

namespace Allure.Xunit.Tests.Samples.EpicAttributes
{
    [AllureEpic("Foo")]
    public class OnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
