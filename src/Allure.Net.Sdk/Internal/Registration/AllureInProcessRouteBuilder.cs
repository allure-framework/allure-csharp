using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Internal.Runtime;
using Allure.Sdk.Registration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Registration;

class AllureInProcessRouteBuilder<TConfiguration>(
    string runtimeName,
    string routeId,
    IAllureRuntime<TConfiguration> runtime
) :
    IAllureInProcessEndpointRegistrationContext<TConfiguration>

    where TConfiguration : AllureConfiguration
{
    Func<IAllureRuntime<TConfiguration>, bool> availabilityPredicate = (_) => true;

    Func<IAllureRuntime<TConfiguration>, bool> curentScopePredicate = (_) => true;

    Func<IAllureRuntime<TConfiguration>, bool> globalScopePredicate = (_) => true;

    Func<IAllureRuntime<TConfiguration>, IEnumerable<string>> currentSuppressedRouteIdsFactory = (_) => [];

    Func<IAllureRuntime<TConfiguration>, AllureInProcessOperations> currentOperationsFactory = (runtime) =>
        new AllureInProcessOperations(
            new RuntimeSyncOperations<TConfiguration>(runtime),
            new RuntimeAsyncOperations<TConfiguration>(runtime)
        );

    Func<IAllureRuntime<TConfiguration>, IAllureParameterSerializer> currentSerializerFactory =
        (_) => runtime.ParameterSerializer;

    public void SetAvailabilityPredicate(Func<IAllureRuntime<TConfiguration>, bool> isAvailable)
    {
        this.availabilityPredicate = isAvailable;
    }

    public void SuppressRoutes(Func<IAllureRuntime<TConfiguration>, IEnumerable<string>> suppressedRouteIdsFactory)
    {
        this.currentSuppressedRouteIdsFactory = suppressedRouteIdsFactory;
    }

    public void UseCurrentScopePredicate(Func<IAllureRuntime<TConfiguration>, bool> predicate)
    {
        this.curentScopePredicate = predicate;
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

    public IAllureRuntimeRoute Build() => new AllureRuntimeRoute(
        routeId,
        () => this.curentScopePredicate(runtime),
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