using Allure.Sdk.Results;
using Allure.Sdk.Runtime;

namespace Allure.Net.Sdk.Tests.Infrastructure;

public class RuntimeTestEnvironmentTests
{
    [Test]
    public async Task ShouldBuildIsolatedRuntime()
    {
        var environment = RuntimeTestEnvironment.Create();

        await Assert.That(environment.Runtime).IsAssignableTo<IAllureRuntime>();
        await Assert.That(environment.Runtime.ResultsDestination)
            .IsSameReferenceAs(environment.Destination);
        await Assert.That(environment.Destination)
            .IsAssignableTo<InMemoryResultsDestination>();
    }
}
