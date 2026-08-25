using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.BddLabels.FeatureApi
{
    public class AttributeOnTestMethod
    {
        [Fact]
        [AllureFeature("Foo")]
        public void TestMethod() { }
    }
}
