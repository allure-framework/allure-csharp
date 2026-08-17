using System;
using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Internal;
using Allure.TestingPlatform.Internal.Correlation;
using Allure.TestingPlatform.Registration;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk.Registration;

/// <summary>
/// Base class for Allure Microsoft Testing Platform runtime registration sessions with custom
/// runtime, registration-context, and integration-context types.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
/// <typeparam name="TRegistrationContext">The registration context type.</typeparam>
/// <typeparam name="TIntegrationContext">The integration context type.</typeparam>
public abstract class AllureTestingPlatformRuntimeRegistrationSession<
    TConfiguration,
    TRuntime,
    TRegistrationContext,
    TIntegrationContext
> :
    AllureRuntimeRegistrationSession<
        TConfiguration,
        TRuntime,
        TRegistrationContext,
        TIntegrationContext
    >,
    IAllureTestingPlatformIntegrationContext<
        TConfiguration,
        TRuntime,
        TRegistrationContext
    >

    where TConfiguration : AllureTestingPlatformConfiguration, new()
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
    where TRegistrationContext : IAllureTestingPlatformRegistrationContext<TConfiguration>
    where TIntegrationContext : IAllureTestingPlatformIntegrationContext<
        TConfiguration,
        TRuntime,
        TRegistrationContext
    >
{
    Func<TConfiguration, ICorrelationStrategy> currentCorrelationStrategyFactory =
        (_) => new SessionUidCorrelationStrategy();

    Func<TConfiguration, ICorrelationContext> currentCorrelationContextFactory =
        (_) => NullCorrelationContext.Instance;

    Func<TConfiguration, ExecutionStateContext> currentExecutionStateContextFactory =
        (_) => NullExecutionStateContext.Instance;

    Func<TConfiguration, ILogger> currentLoggerFactory =
        (_) => NullLogger.Instance;

    /// <inheritdoc />
    public void Disable()
    {
        this.TransformConfiguration(
            static (config) => config.WithProperty(
                cfg => cfg.IsEnabled,
                false,
                (cfg, value) => cfg with { IsEnabled = value }
            )
        );
    }

    /// <inheritdoc />
    public void DisableHostProcessWatchdog()
    {
        this.TransformConfiguration(
            static (config) => config.WithProperty(
                cfg => cfg.IsProcessWatchdogEnabled,
                false,
                (cfg, value) => cfg with { IsProcessWatchdogEnabled = value }
            )
        );
    }

    /// <inheritdoc />
    public void UseCorrelationContext(Func<TConfiguration, ICorrelationContext> correlationContextFactory)
    {
        this.Modify(() => this.currentCorrelationContextFactory = correlationContextFactory);
    }

    /// <inheritdoc />
    public void UseCorrelationStrategy(Func<TConfiguration, ICorrelationStrategy> correlationStrategyFactory)
    {
        this.Modify(() => this.currentCorrelationStrategyFactory = correlationStrategyFactory);
    }

    /// <inheritdoc />
    public void UseExecutionStateContext(Func<TConfiguration, ExecutionStateContext> executionStateContextFactory)
    {
        this.Modify(() => this.currentExecutionStateContextFactory = executionStateContextFactory);
    }

    /// <inheritdoc />
    public void UseLogger(Func<TConfiguration, ILogger> loggerFactory)
    {
        this.Modify(() => this.currentLoggerFactory = loggerFactory);
    }

    /// <inheritdoc />
    protected override sealed TRuntime CreateRuntime(RuntimeCreationArguments<TConfiguration> args)
    {
        var configuration = args.Configuration;
        return this.CreateRuntime(
            args,
            new(
                Logger: this.currentLoggerFactory(configuration),
                CorrelationStrategy: this.currentCorrelationStrategyFactory(configuration),
                CorrelationContext: this.currentCorrelationContextFactory(configuration),
                ExecutionStateContext: this.currentExecutionStateContextFactory(configuration)
            )
        );
    }

    /// <summary>
    /// Creates the runtime from the common and Microsoft Testing Platform-specific arguments.
    /// </summary>
    /// <param name="commonArgs">The services common to all Allure runtimes.</param>
    /// <param name="testingPlatformArgs">The Microsoft Testing Platform-specific services.</param>
    /// <returns>The created runtime.</returns>
    protected abstract TRuntime CreateRuntime(
        RuntimeCreationArguments<TConfiguration> commonArgs,
        AllureTestingPlatformRuntimeArguments testingPlatformArgs
    );
}

/// <summary>
/// Base class for Allure Microsoft Testing Platform runtime registration sessions with custom
/// runtime and registration-context types.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
/// <typeparam name="TRegistrationContext">The registration context type.</typeparam>
public abstract class AllureTestingPlatformRuntimeRegistrationSession<
    TConfiguration,
    TRuntime,
    TRegistrationContext
> :
    AllureTestingPlatformRuntimeRegistrationSession<
        TConfiguration,
        TRuntime,
        TRegistrationContext,
        IAllureTestingPlatformIntegrationContext<TConfiguration, TRuntime, TRegistrationContext>
    >

    where TConfiguration : AllureTestingPlatformConfiguration, new()
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
    where TRegistrationContext : IAllureTestingPlatformRegistrationContext<TConfiguration>
{
    /// <inheritdoc />
    protected override IAllureTestingPlatformIntegrationContext<TConfiguration, TRuntime, TRegistrationContext> IntegrationContext => this;
}

/// <summary>
/// Base class for Allure Microsoft Testing Platform runtime registration sessions with a custom
/// runtime type.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
public abstract class AllureTestingPlatformRuntimeRegistrationSession<
    TConfiguration,
    TRuntime
> :
    AllureTestingPlatformRuntimeRegistrationSession<
        TConfiguration,
        TRuntime,
        IAllureTestingPlatformRegistrationContext<TConfiguration>,
        IAllureTestingPlatformIntegrationContext<TConfiguration, TRuntime>
    >,
    IAllureTestingPlatformIntegrationContext<TConfiguration, TRuntime>

    where TConfiguration : AllureTestingPlatformConfiguration, new()
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
{
    /// <inheritdoc />
    protected override IAllureTestingPlatformRegistrationContext<TConfiguration> RegistrationContext => this;
}

/// <summary>
/// Provides the default runtime registration session for a specific configuration type.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
public class AllureTestingPlatformRegistrationSession<TConfiguration> :
    AllureTestingPlatformRuntimeRegistrationSession<
        TConfiguration,
        IAllureTestingPlatformRuntime<TConfiguration>,
        IAllureTestingPlatformRegistrationContext<TConfiguration>,
        IAllureTestingPlatformIntegrationContext<TConfiguration>
    >,
    IAllureTestingPlatformIntegrationContext<TConfiguration>

    where TConfiguration : AllureTestingPlatformConfiguration, new()
{
    /// <inheritdoc />
    protected override IAllureTestingPlatformIntegrationContext<TConfiguration> IntegrationContext => this;

    /// <inheritdoc />
    protected override IAllureTestingPlatformRegistrationContext<TConfiguration> RegistrationContext => this;

    /// <inheritdoc />
    protected override IAllureTestingPlatformRuntime<TConfiguration> CreateRuntime(
        RuntimeCreationArguments<TConfiguration> commonArgs,
        AllureTestingPlatformRuntimeArguments testingPlatformArgs
    ) =>
        new AllureTestingPlatformRuntime<TConfiguration>(
            commonArgs,
            testingPlatformArgs
        );
}

/// <summary>
/// Provides the registration session for the default Allure Microsoft Testing Platform runtime.
/// </summary>
public class AllureTestingPlatformRegistrationSession :
    AllureTestingPlatformRuntimeRegistrationSession<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformRuntime,
        IAllureTestingPlatformRegistrationContext,
        IAllureTestingPlatformIntegrationContext
    >,
    IAllureTestingPlatformIntegrationContext
{
    /// <inheritdoc />
    protected override IAllureTestingPlatformIntegrationContext IntegrationContext => this;

    /// <inheritdoc />
    protected override IAllureTestingPlatformRegistrationContext RegistrationContext => this;

    /// <inheritdoc />
    protected override IAllureTestingPlatformRuntime CreateRuntime(
        RuntimeCreationArguments<AllureTestingPlatformConfiguration> commonArgs,
        AllureTestingPlatformRuntimeArguments testingPlatformArgs
    ) =>
        new AllureTestingPlatformRuntime(commonArgs, testingPlatformArgs);
}
