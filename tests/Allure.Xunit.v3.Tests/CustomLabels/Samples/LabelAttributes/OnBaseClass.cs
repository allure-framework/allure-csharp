using Xunit;
using Allure.Net.Commons.Attributes;

namespace Allure.Xunit.v3.Tests.Samples.CustomLabels.LabelAttributes
{
    [AllureLabel("foo", "bar")]
    public class BaseClass { }

    public class OnBaseClass : BaseClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
