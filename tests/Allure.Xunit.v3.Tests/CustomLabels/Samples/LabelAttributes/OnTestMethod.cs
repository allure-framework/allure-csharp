using Xunit;
using Allure.Net.Commons.Attributes;

namespace Allure.Xunit.v3.Tests.CustomLabels.Samples.LabelAttributes
{
    public class OnTestMethod
    {
        [Fact]
        [AllureLabel("foo", "bar")]
        public void TestMethod() { }
    }
}
