using System;
using System.Collections.Immutable;
using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Registration;

internal sealed record AllureRuntimeRegistrationSnapshot<
    TConfiguration,
    TEndpointRegistrationContext,
    TEndpointHook,
    TRuntime
>(
    Func<IAllureRegistrationDependencies<TConfiguration>, IAllureExecutionContext> ContextFactory,
    Func<IAllureRegistrationDependencies<TConfiguration>, IAllureLifecycleApi> LifecycleApiFactory,
    Func<IAllureRegistrationDependencies<TConfiguration>, IAllureModelApi> ModelApiFactory,
    bool UseRuleBasedSerializer,
    Func<TConfiguration, IAllureParameterSerializer> SerializerFactory,
    Func<TConfiguration, IAllureResultsDestination> DestinationFactory,
    ImmutableArray<Action<TConfiguration, IParameterSerializationRulesContext>> RuleBasedSerializerRegistrations,
    AllureInProcessEndpointRegistration<TConfiguration, TEndpointRegistrationContext, TEndpointHook, TRuntime>? EndpointRegistration
)
    where TConfiguration : AllureConfiguration
    where TEndpointRegistrationContext : IAllureInProcessEndpointRegistrationContext<TConfiguration, TRuntime>
    where TEndpointHook : IAllureInProcessEndpointRegistrationHook<TConfiguration, TEndpointRegistrationContext, TRuntime>
    where TRuntime : IAllureRuntime<TConfiguration>;
