using System;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Internal.Runtime;

public interface IAllureTestingPlatformRuntimeControl<out TConfiguration, out TRuntime> :
    IAllureTestingPlatformRuntimeHandle<TConfiguration, TRuntime>,
    IAsyncDisposable

    where TConfiguration : AllureTestingPlatformConfiguration
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
{
    void EnsureRuntimeStarted();
}
