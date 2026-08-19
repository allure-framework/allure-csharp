using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.BddLabels.FeatureApi
{
    public class SyncCallFromMethod
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.AddFeature("Foo");
        }
    }
}
