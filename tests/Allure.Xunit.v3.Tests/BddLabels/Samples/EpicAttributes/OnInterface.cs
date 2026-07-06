using Xunit;
using Allure.Net.Commons.Attributes;

namespace Allure.Xunit.Tests.Samples.EpicAttributes
{
    [AllureEpic("Foo")]
    public class IInterface { }

    public class OnInterface : IInterface
    {
        [Fact]
        public void TestMethod() { }
    }
}
