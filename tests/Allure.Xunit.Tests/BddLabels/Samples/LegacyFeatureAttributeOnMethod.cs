using Allure.Xunit.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.BddLabels.Samples.LegacyFeatureAttributeOnMethod
{
    public class TestsClass
    {
        [Fact]
        [AllureFeature("foo")]
        public void TestMethod() { }
    }
}
