using System;
using System.Threading;
using System.Threading.Tasks;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Functions;
using Allure.TestingPlatform.Internal.Runtime;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Extensions.TestHost;

namespace Allure.TestingPlatform.Internal.TestingPlatformExtensions;

using IAllureTestingPlatformRuntimeControl =
    IAllureTestingPlatformRuntimeControl<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
    >;

sealed class AllureTestingPlatformRuntimeRegistrationOwner(
    IAllureTestingPlatformRuntimeControl runtimeControl
) :
    ITestHostApplicationLifetime,
    IAsyncDisposable
{
    public string Uid => "d52cad76-ec4d-4868-9935-53c1711cea06";

    public string DisplayName => "Allure.TestingPlatform runtime lifetime";

    public string Description => "Owns the test-host runtime registration.";

    public string Version { get; } =
        PackageVersions.For(
            typeof(AllureTestingPlatformRuntimeRegistrationOwner)
        );

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);

    public Task BeforeRunAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;

    public Task AfterRunAsync(int exitCode, CancellationToken cancellationToken) =>
        Task.CompletedTask;

    public ValueTask DisposeAsync() => runtimeControl.DisposeAsync();
}
