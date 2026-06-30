using System;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Registration;

/// <summary>
/// Exposes Allure.TestingPlatform runtime lifecycle events for SDK integrations.
/// </summary>
public interface IAllureTestingPlatformSdkEvents
{
    /// <summary>
    /// Occurs after the runtime has been configured.
    /// </summary>
    event Action<ConfiguredAllureTestingPlatformRuntime> OnConfigured;

    /// <summary>
    /// Occurs after the runtime has started.
    /// </summary>
    event Action<LiveAllureTestingPlatformRuntime> OnLive;
}
