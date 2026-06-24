using System;
using System.Threading;
using System.Threading.Tasks;
using Allure.TestingPlatform.Sdk;
using Microsoft.Testing.Platform.Extensions.TestHost;

namespace Allure.TestingPlatform.Implementation;

public class AllureMtpRuntimeProvider(
    IServiceProvider serviceProvider,
    IAllureRuntimeBuilder allureRuntimeBuilder
) :
    AllureMtpToggleableExtension(
        "25aa78ea-da43-40ce-9fcf-356941b2f8cb",
        "Allure runtime provider",
        "Builds Allure runtime when the app run starts and allows consumers to access it.",
        serviceProvider
    ),
    ITestHostApplicationLifetime,
    IAllureRuntimeProvider
{
    IAllureRuntime? allureRuntime;

    public IAllureRuntime Runtime => this.allureRuntime ?? throw new InvalidOperationException(
        "Internal error: Allure.TestingPlatform is not initialized properly."
    );

    public Task BeforeRunAsync(CancellationToken cancellationToken)
    {
        this.allureRuntime = allureRuntimeBuilder.Build(this.ServiceProvider);
        return Task.CompletedTask;
    }

    public Task AfterRunAsync(int exitCode, CancellationToken cancellationToken)
    {
        this.allureRuntime = null;
        return Task.CompletedTask;
    }
}