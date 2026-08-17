using System;
using System.Threading;
using System.Threading.Tasks;
using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Internal.Runtime;
using Allure.TestingPlatform.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.TestingPlatformExtensions;
using Microsoft.Testing.Platform.Extensions.TestHostControllers;

namespace Allure.TestingPlatform.Internal.TestingPlatformExtensions;

using IAllureTestingPlatformRuntimeControl =
    IAllureTestingPlatformRuntimeControl<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
    >;

sealed class AllureTestingPlatformHostProcessWatchdog(
    IAllureTestingPlatformRuntimeControl runtimeControl
) :
    AllureTestingPlatformExtension<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
    >(
        "939b0d5f-517d-4abb-968e-593bc4f67f0c",
        "Allure.TestingPlatform crash watcher",
        "Emits an Allure global error if the test host crashes.",
        runtimeControl
    ),
    ITestHostProcessLifetimeHandler,
    IAsyncDisposable
{
    public override Task<bool> IsEnabledAsync()
    {
        if (!this.Configuration.IsProcessWatchdogEnabled)
        {
            return Task.FromResult(false);
        }

        return base.IsEnabledAsync();
    }

    public Task BeforeTestHostProcessStartAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;

    public async Task OnTestHostProcessExitedAsync(
        ITestHostProcessInformation testHostProcessInformation,
        CancellationToken cancellationToken
    )
    {
        if (testHostProcessInformation is { HasExitedGracefully: false, ExitCode: var exitCode, PID: var pid })
        {
            var message = $"Test host application process (PID={pid}) has crashed. Exit code: {exitCode}";
            runtimeControl.EnsureRuntimeStarted();
            await this.Runtime.ResultsDestination.WriteGlobalsAsync(
                new Globals { Errors = [ new(){ Message = message } ] },
                cancellationToken
            );
        }
    }

    public Task OnTestHostProcessStartedAsync(
        ITestHostProcessInformation testHostProcessInformation,
        CancellationToken cancellationToken
    ) =>
        Task.CompletedTask;

    public ValueTask DisposeAsync()
    {
        return runtimeControl.DisposeAsync();
    }
}
