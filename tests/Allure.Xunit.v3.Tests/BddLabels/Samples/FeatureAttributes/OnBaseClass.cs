using Xunit;
using Allure.Net.Commons.Attributes;

namespace Allure.Xunit.Tests.Samples.FeatureAttributes
{
    [AllureFeature("Foo")]
    public class BaseClass { }

    public class OnBaseClass : BaseClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
