using System.Threading;
using System.Threading.Tasks;
using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Extensions.TestHost;

namespace Allure.TestingPlatform.Sdk.TestingPlatformExtensions;

/// <summary>
/// Starts the Allure.TestingPlatform runtime inside the test host process.
/// </summary>
public sealed class AllureTestingPlatformTestHostRuntimeController(
    IAllureRuntimeRegistrationPlan<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
    > runtimeRegistrationPlan
) :
    AllureTestingPlatformRuntimeControllerExtension<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
    >(
        "25aa78ea-da43-40ce-9fcf-356941b2f8cb",
        "Allure.TestingPlatform lifetime",
        "Ensures the Allure.TestingPlatform runtime is initialized correctly, early, "
            + "and exactly once per test host application.",
        runtimeRegistrationPlan
    ),
    ITestHostApplicationLifetime
{
    /// <inheritdoc />
    public Task BeforeRunAsync(CancellationToken cancellationToken)
    {
        this.EnsureRuntimeStarted();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task AfterRunAsync(int exitCode, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
