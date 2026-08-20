using Allure;
using Allure.Model;
using Xunit;
using System.Threading.Tasks;

namespace Allure.Xunit.v3.Tests.Samples.Severities.SeverityApi
{
    public class AsyncCallFromMethod
    {
        [Fact]
        public async Task TestMethod()
        {
            await AllureApi.SetSeverityAsync(Severity.Critical, TestContext.Current.CancellationToken);
        }
    }
}
