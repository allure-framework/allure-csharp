using System;
using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Logging;
using Microsoft.Testing.Platform.Messages;

namespace Allure.TestingPlatform.Sdk.Registration;

public abstract class AllureTestingPlatformRuntimeIntegrationSnapshot<
    TConfiguration,
    TContext,
    THook,
    TRuntime
>(
    Func<TConfiguration, ILogger> loggerFactory,
    Func<TConfiguration, IMessageBus> messageBusFactory,
    Func<TConfiguration, ICorrelationStrategy> correlationStrategyFactory,
    Func<TConfiguration, ICorrelationContext> correlationContextFactory,
    Func<TConfiguration, ExecutionStateContext> executionStateContextFactory
) :
    IAllureRuntimeIntegrationSnapshot<TConfiguration, TContext, THook, TRuntime>

    where TConfiguration : AllureTestingPlatformConfiguration
    where TContext : IAllureTestingPlatformEndpointRegistrationContext<
        TConfiguration,
        TRuntime
    >
    where THook : IAllureTestingPlatformEndpointRegistrationHook<
        TConfiguration,
        TContext,
        TRuntime
    >
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
{
    public abstract AllureInProcessRouteBuilder<TConfiguration, TContext, THook, TRuntime> CreateRouteBuilder(AllureRouteBuilderArgs<TConfiguration, TRuntime> args);

    public TRuntime CreateRuntime(RuntimeCreationArguments<TConfiguration> args) =>
        this.CreateRuntime(
            args,
            new AllureTestingPlatformRuntimeCreationArguments(
                loggerFactory(args.Configuration),
                messageBusFactory(args.Configuration),
                correlationStrategyFactory(args.Configuration),
                correlationContextFactory(args.Configuration),
                executionStateContextFactory(args.Configuration)
            )
        );

    public abstract TRuntime CreateRuntime(
        RuntimeCreationArguments<TConfiguration> commonArgs,
        AllureTestingPlatformRuntimeCreationArguments testingPlatformArgs
    );
}

public abstract class AllureTestingPlatformRuntimeIntegrationSnapshot<
    TConfiguration,
    TContext,
    THook
>(
    Func<TConfiguration, ILogger> loggerFactory,
    Func<TConfiguration, IMessageBus> messageBusFactory,
    Func<TConfiguration, ICorrelationStrategy> correlationStrategyFactory,
    Func<TConfiguration, ICorrelationContext> correlationContextFactory,
    Func<TConfiguration, ExecutionStateContext> executionStateContextFactory
) :
    AllureTestingPlatformRuntimeIntegrationSnapshot<
        TConfiguration,
        TContext,
        THook,
        IAllureTestingPlatformRuntime<TConfiguration>
    >(
        loggerFactory,
        messageBusFactory,
        correlationStrategyFactory,
        correlationContextFactory,
        executionStateContextFactory
    )

    where TConfiguration : AllureTestingPlatformConfiguration
    where TContext : IAllureTestingPlatformEndpointRegistrationContext<
        TConfiguration
    >
    where THook : IAllureTestingPlatformEndpointRegistrationHook<
        TConfiguration,
        TContext
    >
{
    public override IAllureTestingPlatformRuntime<TConfiguration> CreateRuntime(
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
            testingPlatformArgs.MessageBus,
            testingPlatformArgs.CorrelationStrategy,
            testingPlatformArgs.CorrelationContext,
            testingPlatformArgs.ExecutionStateContext
        );
}

public class AllureTestingPlatformRuntimeIntegrationSnapshot(
    Func<AllureTestingPlatformConfiguration, ILogger> loggerFactory,
    Func<AllureTestingPlatformConfiguration, IMessageBus> messageBusFactory,
    Func<AllureTestingPlatformConfiguration, ICorrelationStrategy> correlationStrategyFactory,
    Func<AllureTestingPlatformConfiguration, ICorrelationContext> correlationContextFactory,
    Func<AllureTestingPlatformConfiguration, ExecutionStateContext> executionStateContextFactory
) :
    AllureTestingPlatformRuntimeIntegrationSnapshot<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformEndpointRegistrationContext,
        IAllureTestingPlatformEndpointRegistrationHook
    >(
        loggerFactory,
        messageBusFactory,
        correlationStrategyFactory,
        correlationContextFactory,
        executionStateContextFactory
    )
{
    public override AllureInProcessRouteBuilder<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformEndpointRegistrationContext,
        IAllureTestingPlatformEndpointRegistrationHook,
        IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
    > CreateRouteBuilder(
        AllureRouteBuilderArgs<
            AllureTestingPlatformConfiguration,
            IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
        > args
    ) => new AllureTestingPlatformRuntimeRouteBuilder(args);
}
