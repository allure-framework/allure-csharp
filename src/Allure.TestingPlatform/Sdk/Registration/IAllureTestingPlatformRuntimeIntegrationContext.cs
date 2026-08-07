using System;
using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Registration;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Logging;
using Microsoft.Testing.Platform.Messages;

namespace Allure.TestingPlatform.Sdk.Registration;

public interface IAllureTestingPlatformRuntimeIntegrationContext<
    TConfiguration,
    TRuntimeRegistrationContext,
    TRuntimeHook,
    TEndpointRegistrationContext,
    TEndpointHook,
    TRuntime
> :
    IAllureRuntimeIntegrationContext<
        TConfiguration,
        TRuntimeRegistrationContext,
        TRuntimeHook,
        TEndpointRegistrationContext,
        TEndpointHook,
        TRuntime
    >,
    IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration>

    where TConfiguration : AllureTestingPlatformConfiguration, new()
    where TRuntimeRegistrationContext : IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration>
    where TRuntimeHook : IAllureTestingPlatformRuntimeRegistrationHook<TConfiguration, TRuntimeRegistrationContext>
    where TEndpointRegistrationContext : IAllureTestingPlatformEndpointRegistrationContext<TConfiguration, TRuntime>
    where TEndpointHook : IAllureTestingPlatformEndpointRegistrationHook<TConfiguration, TEndpointRegistrationContext, TRuntime>
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
{
    void UseLogger(Func<TConfiguration, ILogger> loggerFactory);

    void UseMessageBus(Func<TConfiguration, IMessageBus> messageBusFactory);

    void UseCorrelationStrategy(Func<TConfiguration, ICorrelationStrategy> correlationStrategyFactory);

    void UseCorrelationContext(Func<TConfiguration, ICorrelationContext> correlationContextFactory);

    void UseExecutionStateContext(Func<TConfiguration, ExecutionStateContext> executionStateContextFactory);
}

public interface IAllureTestingPlatformRuntimeIntegrationContext<
    TConfiguration,
    TRuntimeRegistrationContext,
    TRuntimeHook,
    TEndpointRegistrationContext,
    TEndpointHook
> :
    IAllureTestingPlatformRuntimeIntegrationContext<
        TConfiguration,
        TRuntimeRegistrationContext,
        TRuntimeHook,
        TEndpointRegistrationContext,
        TEndpointHook,
        IAllureTestingPlatformRuntime<TConfiguration>
    >

    where TConfiguration : AllureTestingPlatformConfiguration, new()
    where TRuntimeRegistrationContext : IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration>
    where TRuntimeHook : IAllureTestingPlatformRuntimeRegistrationHook<TConfiguration, TRuntimeRegistrationContext>
    where TEndpointRegistrationContext : IAllureTestingPlatformEndpointRegistrationContext<TConfiguration>
    where TEndpointHook : IAllureTestingPlatformEndpointRegistrationHook<TConfiguration, TEndpointRegistrationContext>;

public interface IAllureTestingPlatformRuntimeIntegrationContext :
    IAllureTestingPlatformRuntimeIntegrationContext<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformRuntimeRegistrationContext,
        IAllureTestingPlatformRuntimeRegistrationHook,
        IAllureTestingPlatformEndpointRegistrationContext,
        IAllureTestingPlatformEndpointRegistrationHook
    >,
    IAllureTestingPlatformRuntimeRegistrationContext;
