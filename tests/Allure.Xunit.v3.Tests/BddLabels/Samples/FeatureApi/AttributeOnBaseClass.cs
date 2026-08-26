using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.BddLabels.FeatureApi
{
    [AllureFeature("Foo")]
    public class BaseClass { }

    public class AttributeOnBaseClass : BaseClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
