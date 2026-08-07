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
using Microsoft.Testing.Platform.Messages;

namespace Allure.TestingPlatform.Sdk.Registration;

public abstract class AllureTestingPlatformRuntimeRegistrationSession<
    TConfiguration,
    TRuntimeRegistrationContext,
    TRuntimeHook,
    TEndpointRegistrationContext,
    TEndpointHook,
    TRuntimeIntegrationContext,
    TIntegrationSnapshot,
    TRuntime
> :
    AllureRuntimeRegistrationSession<
        TConfiguration,
        TRuntimeRegistrationContext,
        TRuntimeHook,
        TEndpointRegistrationContext,
        TEndpointHook,
        TRuntimeIntegrationContext,
        TIntegrationSnapshot,
        TRuntime
    >,
    IAllureTestingPlatformRuntimeIntegrationContext<
        TConfiguration,
        TRuntimeRegistrationContext,
        TRuntimeHook,
        TEndpointRegistrationContext,
        TEndpointHook,
        TRuntime
    >

    where TConfiguration : AllureTestingPlatformConfiguration, new()
    where TRuntimeRegistrationContext : IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration>
    where TRuntimeHook : IAllureTestingPlatformRuntimeRegistrationHook<TConfiguration, TRuntimeRegistrationContext>
    where TEndpointRegistrationContext : IAllureTestingPlatformEndpointRegistrationContext<TConfiguration, TRuntime>
    where TEndpointHook : IAllureTestingPlatformEndpointRegistrationHook<TConfiguration, TEndpointRegistrationContext, TRuntime>
    where TRuntimeIntegrationContext : IAllureTestingPlatformRuntimeIntegrationContext<
        TConfiguration,
        TRuntimeRegistrationContext,
        TRuntimeHook,
        TEndpointRegistrationContext,
        TEndpointHook,
        TRuntime
    >
    where TIntegrationSnapshot : IAllureRuntimeIntegrationSnapshot<
        TConfiguration,
        TEndpointRegistrationContext,
        TEndpointHook,
        TRuntime
    >
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
{
    internal Func<TConfiguration, ICorrelationStrategy> currentCorrelationStrategyFactory =
        (_) => new TestingPlatformSessionUidCorrelationStrategy();

    internal Func<TConfiguration, ICorrelationContext> currentCorrelationContextFactory =
        (_) => NullCorrelationContext.Instance;

    internal Func<TConfiguration, ExecutionStateContext> currentExecutionStateContextFactory =
        (_) => NullExecutionStateContext.Instance;

    internal Func<TConfiguration, ILogger> currentLoggerFactory =
        (_) => NullLogger.Instance;

    internal Func<TConfiguration, IMessageBus> currentMessageBusFactory =
        (_) => NullMessageBus.Instance;

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

    public void UseMessageBus(Func<TConfiguration, IMessageBus> messageBusFactory)
    {
        this.Modify(() => this.currentMessageBusFactory = messageBusFactory);
    }
}

public abstract class AllureTestingPlatformRuntimeRegistrationSession<
    TConfiguration,
    TRuntimeRegistrationContext,
    TRuntimeHook,
    TEndpointRegistrationContext,
    TEndpointHook,
    TRuntimeIntegrationContext,
    TIntegrationSnapshot
> :
    AllureTestingPlatformRuntimeRegistrationSession<
        TConfiguration,
        TRuntimeRegistrationContext,
        TRuntimeHook,
        TEndpointRegistrationContext,
        TEndpointHook,
        TRuntimeIntegrationContext,
        TIntegrationSnapshot,
        IAllureTestingPlatformRuntime<TConfiguration>
    >,
    IAllureTestingPlatformRuntimeIntegrationContext<
        TConfiguration,
        TRuntimeRegistrationContext,
        TRuntimeHook,
        TEndpointRegistrationContext,
        TEndpointHook
    >

    where TConfiguration : AllureTestingPlatformConfiguration, new()
    where TRuntimeRegistrationContext : IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration>
    where TRuntimeHook : IAllureTestingPlatformRuntimeRegistrationHook<TConfiguration, TRuntimeRegistrationContext>
    where TEndpointRegistrationContext : IAllureTestingPlatformEndpointRegistrationContext<TConfiguration>
    where TEndpointHook : IAllureTestingPlatformEndpointRegistrationHook<TConfiguration, TEndpointRegistrationContext>
    where TRuntimeIntegrationContext : IAllureTestingPlatformRuntimeIntegrationContext<
        TConfiguration,
        TRuntimeRegistrationContext,
        TRuntimeHook,
        TEndpointRegistrationContext,
        TEndpointHook
    >
    where TIntegrationSnapshot : IAllureRuntimeIntegrationSnapshot<
        TConfiguration,
        TEndpointRegistrationContext,
        TEndpointHook,
        IAllureTestingPlatformRuntime<TConfiguration>
    >;

public class AllureTestingPlatformRuntimeRegistrationSession :
    AllureTestingPlatformRuntimeRegistrationSession<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformRuntimeRegistrationContext,
        IAllureTestingPlatformRuntimeRegistrationHook,
        IAllureTestingPlatformEndpointRegistrationContext,
        IAllureTestingPlatformEndpointRegistrationHook,
        IAllureTestingPlatformRuntimeIntegrationContext,
        IAllureRuntimeIntegrationSnapshot<
            AllureTestingPlatformConfiguration,
            IAllureTestingPlatformEndpointRegistrationContext,
            IAllureTestingPlatformEndpointRegistrationHook,
            IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
        >
    >,
    IAllureTestingPlatformRuntimeIntegrationContext
{
    protected override IAllureTestingPlatformRuntimeIntegrationContext IntegrationContext => this;

    protected override IAllureTestingPlatformRuntimeRegistrationContext RegistrationContext => this;

    protected override IAllureRuntimeIntegrationSnapshot<AllureTestingPlatformConfiguration, IAllureTestingPlatformEndpointRegistrationContext, IAllureTestingPlatformEndpointRegistrationHook, IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>> CaptureIntegrationSnapshot() =>
        new AllureTestingPlatformRuntimeIntegrationSnapshot(
            this.currentLoggerFactory,
            this.currentMessageBusFactory,
            this.currentCorrelationStrategyFactory,
            this.currentCorrelationContextFactory,
            this.currentExecutionStateContextFactory
        );
}
