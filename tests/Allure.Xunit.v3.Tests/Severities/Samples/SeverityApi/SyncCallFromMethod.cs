using Allure;
using Allure.Model;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Severities.SeverityApi
{
    public class SyncCallFromMethod
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.SetSeverity(Severity.Critical);
        }
    }
}
