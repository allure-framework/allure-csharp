using Xunit;
using Allure.Net.Commons.Attributes;

namespace Allure.Xunit.Tests.Samples.FeatureAttributes
{
    [AllureFeature("Foo")]
    public class IInterface { }

    public class OnInterface : IInterface
    {
        [Fact]
        public void TestMethod() { }
    }
}
