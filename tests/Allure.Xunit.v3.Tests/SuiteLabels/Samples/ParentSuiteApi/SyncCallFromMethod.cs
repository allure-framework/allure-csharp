using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.SuiteLabels.ParentSuiteApi
{
    public class SyncCallFromMethod
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.AddParentSuite("Parent Suite");
        }
    }
}
