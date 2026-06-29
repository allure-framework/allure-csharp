using System;
using System.Threading;
using System.Threading.Tasks;
using Allure.TestingPlatform.Internal.Registration;
using Allure.TestingPlatform.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.Runtime.AdapterState;
using Microsoft.Testing.Platform.Extensions.TestHost;

namespace Allure.TestingPlatform.Internal.TestingPlatformExtensions;

public class AllureTestHostApplicationLifetime(
    IAllureTestingPlatformBuilder allureTestingPlatformBuilder
) :
    AllureTestingPlatformExtension(
        "25aa78ea-da43-40ce-9fcf-356941b2f8cb",
        "Allure.TestingPlatform lifetime",
        "Ensures Allure.TestingPlatform is initialized correctly, early, and exactly once per test host application.",
        allureTestingPlatformBuilder.StateProvider
    ),
    ITestHostApplicationLifetime
{
    public override Task<bool> IsEnabledAsync()
    {
        if (allureTestingPlatformBuilder is { StateProvider.Value.State: AllureTestingPlatformState.NotInitialized })
        {
            allureTestingPlatformBuilder.Configure();
            Console.WriteLine(allureTestingPlatformBuilder.StateProvider.Value.IsEnabled);
        }
        return base.IsEnabledAsync();
    }

    public Task BeforeRunAsync(CancellationToken cancellationToken)
    {
        allureTestingPlatformBuilder.Build();
        return Task.CompletedTask;
    }

    public Task AfterRunAsync(int exitCode, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
