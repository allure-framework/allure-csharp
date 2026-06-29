using System.Threading;
using System.Threading.Tasks;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Extensions.TestHost;

namespace Allure.TestingPlatform.Sdk.TestingPlatformExtensions;

public class AllureTestingPlatformInProcessRuntimeController(
    IAllureTestingPlatformRuntimeController runtimeController
) :
    AllureTestingPlatformRuntimeResponsibleExtension(
        "25aa78ea-da43-40ce-9fcf-356941b2f8cb",
        "Allure.TestingPlatform lifetime",
        "Ensures Allure.TestingPlatform runtime is initialized correctly, early, "
            + "and exactly once per test host application.",
        runtimeController
    ),
    ITestHostApplicationLifetime
{
    public Task BeforeRunAsync(CancellationToken cancellationToken)
    {
        this.Controller.Start();
        return Task.CompletedTask;
    }

    public Task AfterRunAsync(int exitCode, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
