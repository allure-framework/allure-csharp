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
    IAllureTestingPlatformRuntimeIntegrationContext<
        TConfiguration,
        TRuntime,
        TRegistrationContext
    >

    where TConfiguration : AllureTestingPlatformConfiguration, new()
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
    where TRegistrationContext : IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration>
    where TIntegrationContext : IAllureTestingPlatformRuntimeIntegrationContext<
        TConfiguration,
        TRuntime,
        TRegistrationContext
    >
{
    Func<TConfiguration, ICorrelationStrategy> currentCorrelationStrategyFactory =
        (_) => new TestingPlatformSessionUidCorrelationStrategy();

    Func<TConfiguration, ICorrelationContext> currentCorrelationContextFactory =
        (_) => NullCorrelationContext.Instance;

    Func<TConfiguration, ExecutionStateContext> currentExecutionStateContextFactory =
        (_) => NullExecutionStateContext.Instance;

    Func<TConfiguration, ILogger> currentLoggerFactory =
        (_) => NullLogger.Instance;

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

    public void UseCorrelationContext(Func<TConfiguration, ICorrelationContext> correlationContextFactory)
    {
        this.Modify(() => this.currentCorrelationContextFactory = correlationContextFactory);
    }

    public void UseCorrelationStrategy(Func<TConfiguration, ICorrelationStrategy> correlationStrategyFactory)
    {
        this.Modify(() => this.currentCorrelationStrategyFactory = correlationStrategyFactory);
    }

    public void UseExecutionStateContext(Func<TConfiguration, ExecutionStateContext> executionStateContextFactory)
    {
        this.Modify(() => this.currentExecutionStateContextFactory = executionStateContextFactory);
    }

    public void UseLogger(Func<TConfiguration, ILogger> loggerFactory)
    {
        this.Modify(() => this.currentLoggerFactory = loggerFactory);
    }

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

    protected abstract TRuntime CreateRuntime(
        RuntimeCreationArguments<TConfiguration> commonArgs,
        AllureTestingPlatformRuntimeCreationArguments testingPlatformArgs
    );
}

public abstract class AllureTestingPlatformRuntimeRegistrationSession<
    TConfiguration,
    TRuntime,
    TRegistrationContext
> :
    AllureTestingPlatformRuntimeRegistrationSession<
        TConfiguration,
        TRuntime,
        TRegistrationContext,
        IAllureTestingPlatformRuntimeIntegrationContext<TConfiguration, TRuntime, TRegistrationContext>
    >

    where TConfiguration : AllureTestingPlatformConfiguration, new()
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
    where TRegistrationContext : IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration>
{
    protected override IAllureTestingPlatformRuntimeIntegrationContext<TConfiguration, TRuntime, TRegistrationContext> IntegrationContext => this;
}

public abstract class AllureTestingPlatformRuntimeRegistrationSession<
    TConfiguration,
    TRuntime
> :
    AllureTestingPlatformRuntimeRegistrationSession<
        TConfiguration,
        TRuntime,
        IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration>,
        IAllureTestingPlatformRuntimeIntegrationContext<TConfiguration, TRuntime>
    >,
    IAllureTestingPlatformRuntimeIntegrationContext<TConfiguration, TRuntime>

    where TConfiguration : AllureTestingPlatformConfiguration, new()
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
{
    protected override IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration> RegistrationContext => this;
}

public class AllureTestingPlatformRuntimeRegistrationSession<TConfiguration> :
    AllureTestingPlatformRuntimeRegistrationSession<
        TConfiguration,
        IAllureTestingPlatformRuntime<TConfiguration>,
        IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration>,
        IAllureTestingPlatformRuntimeIntegrationContext<TConfiguration>
    >,
    IAllureTestingPlatformRuntimeIntegrationContext<TConfiguration>

    where TConfiguration : AllureTestingPlatformConfiguration, new()
{
    protected override IAllureTestingPlatformRuntimeIntegrationContext<TConfiguration> IntegrationContext => this;

    protected override IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration> RegistrationContext => this;

    protected override IAllureTestingPlatformRuntime<TConfiguration> CreateRuntime(
        RuntimeCreationArguments<TConfiguration> commonArgs,
        AllureTestingPlatformRuntimeCreationArguments testingPlatformArgs
    ) =>
        new AllureTestingPlatformRuntime<TConfiguration>(
            commonArgs.Configuration,
            commonArgs.ParameterSerializer,
            commonArgs.Destination,
            commonArgs.Context,
            commonArgs.LifecycleApi,
            commonArgs.ModelApi,
            testingPlatformArgs.Logger,
            testingPlatformArgs.CorrelationStrategy,
            testingPlatformArgs.CorrelationContext,
            testingPlatformArgs.ExecutionStateContext
        );
}

public class AllureTestingPlatformRuntimeRegistrationSession :
    AllureTestingPlatformRuntimeRegistrationSession<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformRuntime,
        IAllureTestingPlatformRuntimeRegistrationContext,
        IAllureTestingPlatformRuntimeIntegrationContext
    >,
    IAllureTestingPlatformRuntimeIntegrationContext
{
    protected override IAllureTestingPlatformRuntimeIntegrationContext IntegrationContext => this;

    protected override IAllureTestingPlatformRuntimeRegistrationContext RegistrationContext => this;

    protected override IAllureTestingPlatformRuntime CreateRuntime(
        RuntimeCreationArguments<AllureTestingPlatformConfiguration> commonArgs,
        AllureTestingPlatformRuntimeCreationArguments testingPlatformArgs
    ) =>
        new AllureTestingPlatformRuntime(
            commonArgs.Configuration,
            commonArgs.ParameterSerializer,
            commonArgs.Destination,
            commonArgs.Context,
            commonArgs.LifecycleApi,
            commonArgs.ModelApi,
            testingPlatformArgs.Logger,
            testingPlatformArgs.CorrelationStrategy,
            testingPlatformArgs.CorrelationContext,
            testingPlatformArgs.ExecutionStateContext
        );
}
