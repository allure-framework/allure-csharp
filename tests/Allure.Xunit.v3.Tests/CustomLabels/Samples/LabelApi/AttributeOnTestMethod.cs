using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.CustomLabels.LabelApi
{
    public class AttributeOnTestMethod
    {
        [Fact]
        [AllureLabel("foo", "bar")]
        public void TestMethod() { }
    }
}
