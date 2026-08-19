using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.FeatureLabels.FeatureAttributes
{
    public class OnTestMethod
    {
        [Fact]
        [AllureFeature("Foo")]
        public void TestMethod() { }
    }
}
