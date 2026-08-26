using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.CustomLabels.LabelApi
{
    [AllureLabel("foo", "bar")]
    public class AttributeOnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
