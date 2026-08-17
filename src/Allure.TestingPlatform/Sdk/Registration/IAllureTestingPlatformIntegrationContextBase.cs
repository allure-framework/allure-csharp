using System;
using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk.Registration;

/// <summary>
/// Provides Microsoft Testing Platform-specific runtime integration operations.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
public interface IAllureTestingPlatformIntegrationContextBase<TConfiguration, out TRuntime> :
    IAllureRuntimeIntegrationContextBase<
        TConfiguration,
        TRuntime
    >,
    IAllureTestingPlatformRegistrationContext<TConfiguration>

    where TConfiguration : AllureTestingPlatformConfiguration
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
{
    /// <summary>
    /// Configures the factory used to create the runtime logger.
    /// </summary>
    /// <param name="loggerFactory">A factory that receives the resolved configuration.</param>
    void UseLogger(Func<TConfiguration, ILogger> loggerFactory);

    /// <summary>
    /// Configures the strategy used to correlate incoming messages.
    /// </summary>
    /// <param name="correlationStrategyFactory">
    /// A factory that receives the resolved configuration.
    /// </param>
    void UseCorrelationStrategy(Func<TConfiguration, ICorrelationStrategy> correlationStrategyFactory);

    /// <summary>
    /// Configures the context that provides the current correlation identifier.
    /// </summary>
    /// <param name="correlationContextFactory">
    /// A factory that receives the resolved configuration.
    /// </param>
    void UseCorrelationContext(Func<TConfiguration, ICorrelationContext> correlationContextFactory);

    /// <summary>
    /// Configures the context that tracks the current execution state.
    /// </summary>
    /// <param name="executionStateContextFactory">
    /// A factory that receives the resolved configuration.
    /// </param>
    void UseExecutionStateContext(Func<TConfiguration, ExecutionStateContext> executionStateContextFactory);
}
