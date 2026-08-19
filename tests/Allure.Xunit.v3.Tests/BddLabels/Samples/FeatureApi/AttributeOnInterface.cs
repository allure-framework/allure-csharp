using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.BddLabels.FeatureApi
{
    [AllureFeature("Foo")]
    public class IInterface { }

    public class AttributeOnInterface : IInterface
    {
        [Fact]
        public void TestMethod() { }
    }
}
