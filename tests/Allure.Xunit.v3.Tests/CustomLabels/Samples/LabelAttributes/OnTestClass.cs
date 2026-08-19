using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.CustomLabels.LabelAttributes
{
    [AllureLabel("foo", "bar")]
    public class OnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
