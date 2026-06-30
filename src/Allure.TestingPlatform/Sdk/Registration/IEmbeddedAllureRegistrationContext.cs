using System;
using Allure.Net.Commons.Configuration;
using Allure.TestingPlatform.Registration;
using Allure.TestingPlatform.Sdk.Correlation;

namespace Allure.TestingPlatform.Sdk.Registration;

/// <summary>
/// Configures an embedded Allure.TestingPlatform registration to be used as a foundation
/// for a Microsoft.Testing.Platform-compatible test framework adapter.
/// </summary>
public interface IEmbeddedAllureRegistrationContext : IStandaloneAllureRegistrationContext
{
    /// <summary>
    /// Sets the strategy used to correlate SDK messages with
    /// Microsoft.Testing.Platform test sessions.
    /// </summary>
    IEmbeddedAllureRegistrationContext UseCorrelation(
        Func<IServiceProvider, AllureConfiguration, ICorrelationStrategy> correlationStrategyFactory
    );

    /// <summary>
    /// Registers handlers for SDK lifecycle events.
    /// </summary>
    IEmbeddedAllureRegistrationContext SetSdkEventHandlers(
        Action<IAllureTestingPlatformSdkEvents> sdkEventHandlersRegistration
    );
}
