using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.EpicLabels.EpicAttributes
{
    [AllureEpic("Foo")]
    public class IInterface { }

    public class OnInterface : IInterface
    {
        [Fact]
        public void TestMethod() { }
    }
}
