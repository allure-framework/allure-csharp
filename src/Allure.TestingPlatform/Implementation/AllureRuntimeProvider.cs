using System;
using System.Threading;
using System.Threading.Tasks;
using Allure.TestingPlatform.Functions;
using Allure.TestingPlatform.Sdk;
using Microsoft.Testing.Platform.Extensions.TestHost;

namespace Allure.TestingPlatform.Implementation;

public class AllureRuntimeProvider(IAllureRuntimeBuildResult allureRuntimeBuildResult) :
    ITestHostApplicationLifetime,
    IAllureRuntimeProvider
{
    IAllureRuntime? allureRuntime;

    public string Uid => "25aa78ea-da43-40ce-9fcf-356941b2f8cb";

    public string DisplayName => "Allure runtime provider";

    public string Description =>
        "Builds Allure runtime when the app run starts and allows consumers to access it.";

    public string Version { get; } = TestingPlatformFunctions.CurrentPackageVersion;

    public Task<bool> IsEnabledAsync() => Task.FromResult(
        allureRuntimeBuildResult.ExtensionSettings.IsEnabled
    );

    public bool IsAllureEnabled => allureRuntimeBuildResult.ExtensionSettings.IsEnabled;

    public bool IsAllureAlive => this.allureRuntime is not null;

    public IAllureRuntime Runtime =>
        this.IsAllureEnabled
            ? this.allureRuntime ?? throw new InvalidOperationException(
                "Internal error: Allure.TestingPlatform is not fully initialized."
            )
            : throw new InvalidOperationException("Internal error: Allure is disabled.");

    public Task BeforeRunAsync(CancellationToken cancellationToken)
    {
        this.allureRuntime = allureRuntimeBuildResult.CreateRuntime();
        return Task.CompletedTask;
    }

    public Task AfterRunAsync(int exitCode, CancellationToken cancellationToken)
    {
        this.allureRuntime = null;
        return Task.CompletedTask;
    }
}