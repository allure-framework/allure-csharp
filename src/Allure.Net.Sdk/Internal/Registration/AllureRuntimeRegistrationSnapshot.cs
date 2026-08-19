using System;
using System.Collections.Immutable;
using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;
using Allure.Sdk.TestPlan;

namespace Allure.Sdk.Internal.Registration;

internal sealed record AllureRuntimeRegistrationSnapshot<TConfiguration, TRuntime>(
    Func<RuntimeServiceCreationContext<TConfiguration>, IAllureExecutionContext> ContextFactory,
    Func<RuntimeServiceCreationContext<TConfiguration>, IAllureLifecycleApi> LifecycleApiFactory,
    Func<RuntimeServiceCreationContext<TConfiguration>, IAllureModelApi> ModelApiFactory,
    Func<TConfiguration, AllureTestPlan> TestPlanFactory,
    bool UseRuleBasedSerializer,
    Func<TConfiguration, IAllureParameterSerializer> SerializerFactory,
    Func<TConfiguration, IAllureResultsDestination> DestinationFactory,
    ImmutableArray<Action<TConfiguration, IParameterSerializationRulesContext>> RuleBasedSerializerRegistrations,
    AllureInProcessEndpointRegistration<TConfiguration, TRuntime>? EndpointRegistration
)
    where TConfiguration : AllureConfiguration
    where TRuntime : IAllureRuntime<TConfiguration>;
