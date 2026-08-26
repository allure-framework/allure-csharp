using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Owners.OwnerApi
{
    public class SyncCallFromMethod
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.SetOwner("John Doe");
        }
    }
}
