using Allure.Xunit.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.BddLabels.Samples.LegacyFeatureAttributeOnClass
{
    [AllureFeature("foo")]
    public class TestsClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
