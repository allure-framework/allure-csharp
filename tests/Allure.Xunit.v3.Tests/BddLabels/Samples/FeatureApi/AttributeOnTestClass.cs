using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.BddLabels.FeatureApi
{
    [AllureFeature("Foo")]
    public class AttributeOnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
