using System;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Internal.Runtime;

/// <summary>
/// Controls the lifetime of an Allure.TestingPlatform runtime.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
public interface IAllureTestingPlatformRuntimeControl<out TConfiguration, out TRuntime> :
    IAllureTestingPlatformRuntimeHandle<TConfiguration, TRuntime>,
    IAsyncDisposable

    where TConfiguration : AllureTestingPlatformConfiguration
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
{
    /// <summary>
    /// Ensures that the runtime has been created and started.
    /// </summary>
    void EnsureRuntimeStarted();
}
