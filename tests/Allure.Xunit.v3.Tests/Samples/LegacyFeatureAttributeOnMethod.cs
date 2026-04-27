using Allure.Xunit.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.Samples.LegacyFeatureAttributeOnMethod
{
    public class TestsClass
    {
        [Fact]
        [AllureFeature("foo")]
        public void TestMethod() { }
    }
}



