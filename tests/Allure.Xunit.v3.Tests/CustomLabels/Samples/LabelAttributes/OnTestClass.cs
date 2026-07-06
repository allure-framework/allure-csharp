using Xunit;
using Allure.Net.Commons.Attributes;

namespace Allure.Xunit.v3.Tests.CustomLabels.Samples.LabelAttributes
{
    [AllureLabel("foo", "bar")]
    public class OnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
