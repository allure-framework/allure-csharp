using System;
using System.Collections.Generic;
using System.Linq;
using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Internal.Runtime;
using Allure.Sdk.Registration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Registration;

internal class AllureInProcessRouteBuilder<TConfiguration, TRuntime> :
    IAllureInProcessEndpointIntegrationContext<TRuntime>

    where TConfiguration : AllureConfiguration
    where TRuntime : IAllureRuntime<TConfiguration>
{
    readonly string runtimeName;

    readonly string routeId;

    Func<TRuntime, IEnumerable<IAllureRegistrationHook<IAllureInProcessEndpointRegistrationContext<TRuntime>>?>> currentHooksFactory =
        (_) => [];

    Func<TRuntime, bool> availabilityPredicate = (_) => true;

    Func<TRuntime, bool> currentScopePredicate = (_) => true;

    Func<TRuntime, bool> globalScopePredicate = (_) => true;

    Func<TRuntime, IEnumerable<string>> currentSuppressedRouteIdsFactory = (_) => [];

    Func<TRuntime, AllureInProcessOperations> currentOperationsFactory = (runtime) =>
        new AllureInProcessOperations(
            new RuntimeSyncOperations<TConfiguration>(runtime),
            new RuntimeAsyncOperations<TConfiguration>(runtime)
        );

    bool useRuleBasedSerializer = false;

    Func<TRuntime, IAllureParameterSerializer> currentSerializerFactory;

    readonly List<Action<TRuntime, IParameterSerializationRulesContext>> currentRuleBasedSerializerRegistrations;

    readonly Action<TRuntime, IAllureInProcessEndpointIntegrationContext<TRuntime>> endpointRegistration;

    protected TRuntime Runtime { get; }

    public AllureInProcessRouteBuilder(
        string runtimeName,
        string routeId,
        TRuntime runtime,
        bool useRuleBasedSerializer,
        IEnumerable<Action<TConfiguration, IParameterSerializationRulesContext>> ruleBasedSerializerRegistrations,
        Action<TRuntime, IAllureInProcessEndpointIntegrationContext<TRuntime>> endpointRegistration
    )
    {
        this.useRuleBasedSerializer = useRuleBasedSerializer;
        this.runtimeName = runtimeName;
        this.routeId = routeId;
        this.Runtime = runtime;
        this.currentRuleBasedSerializerRegistrations = [
            .. ruleBasedSerializerRegistrations
                .Select((registration) =>
                    (Action<TRuntime, IParameterSerializationRulesContext>)(
                        (runtime, context) => registration(runtime.Configuration, context)
                    )
                )
        ];
        this.currentSerializerFactory =
            useRuleBasedSerializer
                ? (runtime) =>
                    AllureRegistrationDefaults.EndpointParameterSerializer(
                        this.currentRuleBasedSerializerRegistrations
                    )(runtime)
                : (_) => this.Runtime.ParameterSerializer;
        this.endpointRegistration = endpointRegistration;
    }

    public void UseRegistrationHooks(
        Func<TRuntime, IEnumerable<IAllureRegistrationHook<IAllureInProcessEndpointRegistrationContext<TRuntime>>?>> hooksFactory
    )
    {
        this.currentHooksFactory = hooksFactory;
    }

    public void SetAvailabilityPredicate(Func<TRuntime, bool> isAvailable)
    {
        this.availabilityPredicate = isAvailable;
    }

    public void SetAvailabilityPredicate(Func<bool> isAvailable)
    {
        this.availabilityPredicate = (_) => isAvailable();
    }

    public void SuppressRoutes(Func<TRuntime, IEnumerable<string>> suppressedRouteIdsFactory)
    {
        this.currentSuppressedRouteIdsFactory = suppressedRouteIdsFactory;
    }

    public void SuppressRoutes(Func<IEnumerable<string>> suppressedRouteIdsFactory)
    {
        this.currentSuppressedRouteIdsFactory = (_) => suppressedRouteIdsFactory();
    }

    public void UseCurrentScopePredicate(Func<TRuntime, bool> predicate)
    {
        this.currentScopePredicate = predicate;
    }

    public void UseGlobalScopePredicate(Func<TRuntime, bool> predicate)
    {
        this.globalScopePredicate = predicate;
    }

    public void UseOperations(Func<TRuntime, AllureInProcessOperations> operationsFactory)
    {
        this.currentOperationsFactory = operationsFactory;
    }

    public void UseParameterSerializer(Func<TRuntime, IAllureParameterSerializer> serializerFactory)
    {
        this.useRuleBasedSerializer = false;
        this.currentRuleBasedSerializerRegistrations.Clear();
        this.currentSerializerFactory = serializerFactory;
    }

    public void UseParameterSerializer(Func<IAllureParameterSerializer> serializerFactory) =>
        this.UseParameterSerializer((_) => serializerFactory());

    public void ConfigureSerialization(Action<TRuntime, IParameterSerializationRulesContext> registration)
    {
        if (!this.useRuleBasedSerializer)
        {
            this.currentSerializerFactory = (runtime) => AllureRegistrationDefaults.EndpointParameterSerializer(
                currentRuleBasedSerializerRegistrations
            )(runtime);
            this.useRuleBasedSerializer = true;
        }

        this.currentRuleBasedSerializerRegistrations.Add(registration);
    }

    public void ConfigureSerialization(Action<IParameterSerializationRulesContext> registration)
    {
        this.ConfigureSerialization((_, ctx) => registration(ctx));
    }

    public IAllureRuntimeRoute Build()
    {
        this.endpointRegistration(this.Runtime, this);
        this.RunHooks();
        return new AllureRuntimeRoute(
            routeId,
            () => this.currentScopePredicate(this.Runtime),
            () => this.globalScopePredicate(this.Runtime),
            [.. this.currentSuppressedRouteIdsFactory(this.Runtime)],
            new AllureInProcessRuntimeEndpoint(
                this.runtimeName,
                () => this.availabilityPredicate(this.Runtime),
                this.currentOperationsFactory(this.Runtime),
                this.currentSerializerFactory(this.Runtime)
            )
        );
    }

    void RunHooks()
    {
        foreach (var hook in this.currentHooksFactory(this.Runtime))
        {
            hook?.SetUp(this);
        }
    }
}
