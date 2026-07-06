using Xunit;
using Allure.Net.Commons.Attributes;

namespace Allure.Xunit.Tests.Samples.FeatureAttributes
{
    [AllureFeature("Foo")]
    public class OnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
