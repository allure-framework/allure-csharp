using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.FeatureLabels.FeatureAttributes
{
    [AllureFeature("Foo")]
    public class OnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
