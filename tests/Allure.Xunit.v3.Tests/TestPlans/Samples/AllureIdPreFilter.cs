using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.TestPlans.AllureIdPreFilter
{
    public class SelectedMarkerClass
    {
        public SelectedMarkerClass()
        {
            AllureApi.AddGlobalError("selected Allure ID test was constructed");
        }

        [Fact]
        [AllureId(3002)]
        public void TestMethod() { }
    }

    public class UnselectedMarkerClass
    {
        public UnselectedMarkerClass()
        {
            AllureApi.AddGlobalError("unselected Allure ID test was constructed");
        }

        [Fact]
        [AllureId(3003)]
        public void TestMethod() { }
    }
}
