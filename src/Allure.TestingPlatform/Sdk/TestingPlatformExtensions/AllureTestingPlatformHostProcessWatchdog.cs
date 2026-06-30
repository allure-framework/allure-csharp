using System.Threading;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Extensions.TestHostControllers;

namespace Allure.TestingPlatform.Sdk.TestingPlatformExtensions;

public class AllureTestingPlatformHostProcessWatchdog(
    bool isEnabled,
    IAllureTestingPlatformRuntimeController runtimeController
) :
    AllureTestingPlatformRuntimeControllerExtension(
        "939b0d5f-517d-4abb-968e-593bc4f67f0c",
        "Allure.TestingPlatform crash watcher",
        "Emits an Allure global error if the test host crashes.",
        runtimeController
    ),
    ITestHostProcessLifetimeHandler
{
    public override Task<bool> IsEnabledAsync()
    {
        if (!isEnabled)
        {
            return Task.FromResult(false);
        }

        return base.IsEnabledAsync();
    }
    public Task BeforeTestHostProcessStartAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;

    public Task OnTestHostProcessExitedAsync(
        ITestHostProcessInformation testHostProcessInformation,
        CancellationToken cancellationToken
    )
    {
        if (testHostProcessInformation is { HasExitedGracefully: false, ExitCode: var exitCode, PID: var pid }
            && this.EnsureRuntimeStarted() is LiveAllureTestingPlatformRuntime { Writer: var writer })
        {
            var message = $"Test host application process (PID={pid}) has crashed. Exit code: {exitCode}";
            writer.Write(new Globals { errors = [ new(){ message = message } ] });
        }
        return Task.CompletedTask;
    }

    public Task OnTestHostProcessStartedAsync(
        ITestHostProcessInformation testHostProcessInformation,
        CancellationToken cancellationToken
    ) =>
        Task.CompletedTask;
}
