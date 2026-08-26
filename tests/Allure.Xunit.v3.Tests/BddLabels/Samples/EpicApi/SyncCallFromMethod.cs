using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.BddLabels.EpicApi
{
    public class SyncCallFromMethod
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.AddEpic("Foo");
        }
    }
}
