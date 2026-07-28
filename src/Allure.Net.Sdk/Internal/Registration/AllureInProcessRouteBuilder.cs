using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Internal.Runtime;
using Allure.Sdk.Registration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Registration;

class AllureInProcessRouteBuilder<TConfiguration, THook>(
    string runtimeName,
    string routeId,
    IAllureRuntime<TConfiguration> runtime
) :
    IAllureInProcessEndpointIntegrationContext<TConfiguration, THook>

    where TConfiguration : AllureConfiguration
    where THook : IAllureInProcessEndpointRegistrationHook<TConfiguration>
{
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

    Func<IAllureRuntime<TConfiguration>, IAllureParameterSerializer> currentSerializerFactory =
        (_) => runtime.ParameterSerializer;

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
        this.currentSerializerFactory = serializerFactory;
    }

    public void UseParameterSerializer(Func<IAllureParameterSerializer> serializerFactory)
    {
        this.currentSerializerFactory = (_) => serializerFactory();
    }

    public IAllureRuntimeRoute Build()
    {
        this.RunHooks();
        return new AllureRuntimeRoute(
            routeId,
            () => this.currentScopePredicate(runtime),
            () => this.globalScopePredicate(runtime),
            [.. this.currentSuppressedRouteIdsFactory(runtime)],
            new AllureInProcessRuntimeEndpoint(
                runtimeName,
                () => this.availabilityPredicate(runtime),
                this.currentOperationsFactory(runtime),
                this.currentSerializerFactory(runtime)
            )
        );
    }

    void RunHooks()
    {
        foreach (var hook in this.currentHooksFactory(runtime.Configuration))
        {
            hook?.SetUp(this);
        }
    }
}