using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Documentation.DocumentationApi
{
    public class SyncCallFromMethod
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.SetDescription("Test description");
            AllureApi.SetDescriptionHtml("<p>Test HTML</p>");
            AllureApi.SetPreconditions("Test preconditions");
            AllureApi.SetPreconditionsHtml("<p>Test preconditions HTML</p>");
            AllureApi.SetExpectedResults("Test expected results");
            AllureApi.SetExpectedResultsHtml("<p>Test expected results HTML</p>");
        }
    }
}
