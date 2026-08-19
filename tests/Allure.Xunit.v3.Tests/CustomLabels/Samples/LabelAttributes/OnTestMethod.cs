using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.CustomLabels.LabelAttributes
{
    public class OnTestMethod
    {
        [Fact]
        [AllureLabel("foo", "bar")]
        public void TestMethod() { }
    }
}
