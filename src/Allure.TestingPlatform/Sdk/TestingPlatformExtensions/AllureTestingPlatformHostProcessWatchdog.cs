using System.Threading;
using System.Threading.Tasks;
using Allure.Model;
using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Extensions.TestHostControllers;

namespace Allure.TestingPlatform.Sdk.TestingPlatformExtensions;

public class AllureTestingPlatformHostProcessWatchdog<TConfiguration, TRuntime>(
    IAllureRuntimeRegistrationPlan<TConfiguration, TRuntime> runtimeRegistrationPlan
) :
    AllureTestingPlatformRuntimeControllerExtension<TConfiguration, TRuntime>(
        "939b0d5f-517d-4abb-968e-593bc4f67f0c",
        "Allure.TestingPlatform crash watcher",
        "Emits an Allure global error if the test host crashes.",
        runtimeRegistrationPlan
    ),
    ITestHostProcessLifetimeHandler

    where TConfiguration : AllureTestingPlatformConfiguration
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
{
    /// <inheritdoc />
    public override Task<bool> IsEnabledAsync()
    {
        if (!this.Configuration.IsProcessWatchdogEnabled)
        {
            return Task.FromResult(false);
        }

        return base.IsEnabledAsync();
    }

    /// <inheritdoc />
    public Task BeforeTestHostProcessStartAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;

    /// <inheritdoc />
    public async Task OnTestHostProcessExitedAsync(
        ITestHostProcessInformation testHostProcessInformation,
        CancellationToken cancellationToken
    )
    {
        if (testHostProcessInformation is { HasExitedGracefully: false, ExitCode: var exitCode, PID: var pid })
        {
            var message = $"Test host application process (PID={pid}) has crashed. Exit code: {exitCode}";
            this.EnsureRuntimeStarted();
            await this.Runtime.ResultsDestination.WriteGlobalsAsync(
                new Globals { Errors = [ new(){ Message = message } ] },
                cancellationToken
            );
        }
    }

    /// <inheritdoc />
    public Task OnTestHostProcessStartedAsync(
        ITestHostProcessInformation testHostProcessInformation,
        CancellationToken cancellationToken
    ) =>
        Task.CompletedTask;
}
