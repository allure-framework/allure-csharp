using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.BddLabels.Samples.FeatureAttributeOnMethod
{
    public class TestsClass
    {
        [Fact]
        [AllureFeature("foo")]
        public void TestMethod() { }
    }
}
