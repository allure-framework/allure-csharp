using Xunit;
using Allure.Net.Commons.Attributes;

namespace Allure.Xunit.v3.Tests.FeatureLabels.Samples.FeatureAttributes
{
    public class OnTestMethod
    {
        [Fact]
        [AllureFeature("Foo")]
        public void TestMethod() { }
    }
}
