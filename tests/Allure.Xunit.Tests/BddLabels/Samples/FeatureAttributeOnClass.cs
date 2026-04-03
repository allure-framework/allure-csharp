using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.BddLabels.Samples.FeatureAttributeOnClass
{
    [AllureFeature("foo")]
    public class TestsClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
