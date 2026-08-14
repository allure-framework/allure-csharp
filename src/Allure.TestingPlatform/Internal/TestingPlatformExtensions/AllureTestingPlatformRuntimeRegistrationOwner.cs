using System;
using System.Threading;
using System.Threading.Tasks;
using Allure.TestingPlatform.Functions;
using Allure.TestingPlatform.Internal.Runtime;
using Microsoft.Testing.Platform.Extensions.TestHost;

namespace Allure.TestingPlatform.Internal.TestingPlatformExtensions;

class AllureTestingPlatformRuntimeRegistrationOwner(
    IAllureTestingPlatformRuntimeHandle runtimeHandle
) :
    ITestHostApplicationLifetime,
    IAsyncDisposable
{
    public string Uid => "d52cad76-ec4d-4868-9935-53c1711cea06";

    public string DisplayName => "Allure.TestingPlatform runtime lifetime";

    public string Description => "Owns the test-host runtime registration.";

    public string Version { get; } =
        TestingPlatformFunctions.GetPackageVersion(
            typeof(AllureTestingPlatformRuntimeRegistrationOwner)
        );

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);

    public Task BeforeRunAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;

    public Task AfterRunAsync(int exitCode, CancellationToken cancellationToken) =>
        Task.CompletedTask;

    public ValueTask DisposeAsync() => runtimeHandle.DisposeAsync();
}
