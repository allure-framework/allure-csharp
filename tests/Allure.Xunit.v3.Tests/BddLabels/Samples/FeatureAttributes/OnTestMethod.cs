using Xunit;
using Allure.Net.Commons.Attributes;

namespace Allure.Xunit.Tests.Samples.FeatureAttributes
{
    public class OnTestMethod
    {
        [Fact]
        [AllureFeature("Foo")]
        public void TestMethod() { }
    }
}
