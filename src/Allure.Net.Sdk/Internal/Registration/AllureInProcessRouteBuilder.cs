using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Internal.Runtime;
using Allure.Sdk.Registration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Registration;

class AllureInProcessRouteBuilder<TConfiguration, THook> :
    IAllureInProcessEndpointIntegrationContext<TConfiguration, THook>

    where TConfiguration : AllureConfiguration
    where THook : IAllureInProcessEndpointRegistrationHook<TConfiguration>
{
    readonly string runtimeName;

    readonly string routeId;

    readonly IAllureRuntime<TConfiguration> runtime;

    Func<TConfiguration, IEnumerable<THook?>> currentHooksFactory =
        (_) => [];

    Func<IAllureRuntime<TConfiguration>, bool> availabilityPredicate = (_) => true;

    Func<IAllureRuntime<TConfiguration>, bool> currentScopePredicate = (_) => true;

    Func<IAllureRuntime<TConfiguration>, bool> globalScopePredicate = (_) => true;

    Func<IAllureRuntime<TConfiguration>, IEnumerable<string>> currentSuppressedRouteIdsFactory = (_) => [];

    Func<IAllureRuntime<TConfiguration>, AllureInProcessOperations> currentOperationsFactory = (runtime) =>
        new AllureInProcessOperations(
            new RuntimeSyncOperations<TConfiguration>(runtime),
            new RuntimeAsyncOperations<TConfiguration>(runtime)
        );

    bool useRuleBasedSerializer = false;

    Func<IAllureRuntime<TConfiguration>, IAllureParameterSerializer> currentSerializerFactory;

    readonly List<Action<TConfiguration, IParameterSerializationRulesContext>> currentRuleBasedSerializerRegistrations;

    public AllureInProcessRouteBuilder(
        string runtimeName,
        string routeId,
        IAllureRuntime<TConfiguration> runtime,
        bool useRuleBasedSerializer,
        IEnumerable<Action<TConfiguration, IParameterSerializationRulesContext>> ruleBasedSerializerRegistrations
    )
    {
        this.useRuleBasedSerializer = useRuleBasedSerializer;
        this.runtimeName = runtimeName;
        this.routeId = routeId;
        this.runtime = runtime;
        this.currentRuleBasedSerializerRegistrations = [.. ruleBasedSerializerRegistrations];
        this.currentSerializerFactory =
            useRuleBasedSerializer
                ? (runtime) =>
                    AllureRegistrationDefaults.ParameterSerializer<TConfiguration>(
                        currentRuleBasedSerializerRegistrations
                    )(runtime.Configuration)
                : (_) => runtime.ParameterSerializer;
    }

    public void UseRegistrationHooks(
        Func<TConfiguration, IEnumerable<THook?>> hooksFactory
    )
    {
        this.currentHooksFactory = hooksFactory;
    }

    public void SetAvailabilityPredicate(Func<IAllureRuntime<TConfiguration>, bool> isAvailable)
    {
        this.availabilityPredicate = isAvailable;
    }

    public void SetAvailabilityPredicate(Func<bool> isAvailable)
    {
        this.availabilityPredicate = (_) => isAvailable();
    }

    public void SuppressRoutes(Func<IAllureRuntime<TConfiguration>, IEnumerable<string>> suppressedRouteIdsFactory)
    {
        this.currentSuppressedRouteIdsFactory = suppressedRouteIdsFactory;
    }

    public void SuppressRoutes(Func<IEnumerable<string>> suppressedRouteIdsFactory)
    {
        this.currentSuppressedRouteIdsFactory = (_) => suppressedRouteIdsFactory();
    }

    public void UseCurrentScopePredicate(Func<IAllureRuntime<TConfiguration>, bool> predicate)
    {
        this.currentScopePredicate = predicate;
    }

    public void UseGlobalScopePredicate(Func<IAllureRuntime<TConfiguration>, bool> predicate)
    {
        this.globalScopePredicate = predicate;
    }

    public void UseOperations(Func<IAllureRuntime<TConfiguration>, AllureInProcessOperations> operationsFactory)
    {
        this.currentOperationsFactory = operationsFactory;
    }

    public void UseParameterSerializer(Func<IAllureRuntime<TConfiguration>, IAllureParameterSerializer> serializerFactory)
    {
        this.useRuleBasedSerializer = false;
        this.currentRuleBasedSerializerRegistrations.Clear();
        this.currentSerializerFactory = serializerFactory;
    }

    public void UseParameterSerializer(Func<IAllureParameterSerializer> serializerFactory) =>
        this.UseParameterSerializer((_) => serializerFactory());

    public void ConfigureSerialization(Action<TConfiguration, IParameterSerializationRulesContext> registration)
    {
        if (!this.useRuleBasedSerializer)
        {
            this.currentSerializerFactory = (runtime) => AllureRegistrationDefaults.ParameterSerializer(
                currentRuleBasedSerializerRegistrations
            )(runtime.Configuration);
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
        this.RunHooks();
        return new AllureRuntimeRoute(
            routeId,
            () => this.currentScopePredicate(this.runtime),
            () => this.globalScopePredicate(this.runtime),
            [.. this.currentSuppressedRouteIdsFactory(this.runtime)],
            new AllureInProcessRuntimeEndpoint(
                this.runtimeName,
                () => this.availabilityPredicate(this.runtime),
                this.currentOperationsFactory(this.runtime),
                this.currentSerializerFactory(this.runtime)
            )
        );
    }

    void RunHooks()
    {
        foreach (var hook in this.currentHooksFactory(this.runtime.Configuration))
        {
            hook?.SetUp(this);
        }
    }
}