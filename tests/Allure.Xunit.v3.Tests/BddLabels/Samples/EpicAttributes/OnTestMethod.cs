using Xunit;
using Allure.Net.Commons.Attributes;

namespace Allure.Xunit.Tests.Samples.EpicAttributes
{
    public class OnTestMethod
    {
        [Fact]
        [AllureEpic("Foo")]
        public void TestMethod() { }
    }
}
