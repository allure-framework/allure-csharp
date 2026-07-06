using Xunit;
using Allure.Net.Commons.Attributes;

namespace Allure.Xunit.v3.Tests.FeatureLabels.Samples.FeatureAttributes
{
    [AllureFeature("Foo")]
    public class OnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
