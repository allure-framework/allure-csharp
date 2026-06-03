using Allure.Net.Commons;
using Xunit;

namespace Allure.Xunit.Tests.BddLabels.Samples.AddFeatureFromTest
{
    public class TestsClass
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.AddFeature("foo");
        }
    }
}
