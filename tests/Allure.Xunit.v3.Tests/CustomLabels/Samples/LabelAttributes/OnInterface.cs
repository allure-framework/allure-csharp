using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.CustomLabels.LabelAttributes
{
    [AllureLabel("foo", "bar")]
    public class IInterface { }

    public class OnInterface : IInterface
    {
        [Fact]
        public void TestMethod() { }
    }
}
