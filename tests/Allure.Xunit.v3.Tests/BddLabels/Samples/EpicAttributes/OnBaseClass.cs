using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.EpicLabels.EpicAttributes
{
    [AllureEpic("Foo")]
    public class BaseClass { }

    public class OnBaseClass : BaseClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
