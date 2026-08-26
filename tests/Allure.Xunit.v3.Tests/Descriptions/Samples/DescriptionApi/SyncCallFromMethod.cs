using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Descriptions.DescriptionApi
{
    public class SyncCallFromMethod
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.SetDescription("Test description");
            AllureApi.SetDescriptionHtml("<p>Test HTML</p>");
        }
    }
}
