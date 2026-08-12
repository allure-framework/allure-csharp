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

    readonly Action<TRuntime, IAllureInProcessEndpointIntegrationContext<TRuntime>> registration;

    /// <summary>
    /// Gets the integration-specific runtime associated with this route.
    /// </summary>
    protected TRuntime Runtime { get; }

    /// <summary>
    /// Initializes an in-process route builder from the components resolved by
    /// its runtime builder.
    /// </summary>
    /// <param name="args">The resolved route builder arguments.</param>
    public AllureInProcessRouteBuilder(EndpointRouteCreationArguments<TConfiguration, TRuntime> args)
    {
        this.useRuleBasedSerializer = args.UseRuleBasedSerializer;
        this.runtimeName = args.RuntimeName;
        this.routeId = args.RouteId;
        this.Runtime = args.Runtime;
        this.currentRuleBasedSerializerRegistrations = [
            .. args.RuleBasedSerializerRegistrations
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
                        currentRuleBasedSerializerRegistrations
                    )(runtime)
                : (_) => this.Runtime.ParameterSerializer;
        this.registration = args.Registration;
    }

    /// <inheritdoc/>
    public void UseRegistrationHooks(
        Func<TRuntime, IEnumerable<IAllureRegistrationHook<IAllureInProcessEndpointRegistrationContext<TRuntime>>?>> hooksFactory
    )
    {
        this.currentHooksFactory = hooksFactory;
    }

    /// <inheritdoc/>
    public void SetAvailabilityPredicate(Func<TRuntime, bool> isAvailable)
    {
        this.availabilityPredicate = isAvailable;
    }

    /// <inheritdoc/>
    public void SetAvailabilityPredicate(Func<bool> isAvailable)
    {
        this.availabilityPredicate = (_) => isAvailable();
    }

    /// <inheritdoc/>
    public void SuppressRoutes(Func<TRuntime, IEnumerable<string>> suppressedRouteIdsFactory)
    {
        this.currentSuppressedRouteIdsFactory = suppressedRouteIdsFactory;
    }

    /// <inheritdoc/>
    public void SuppressRoutes(Func<IEnumerable<string>> suppressedRouteIdsFactory)
    {
        this.currentSuppressedRouteIdsFactory = (_) => suppressedRouteIdsFactory();
    }

    /// <inheritdoc/>
    public void UseCurrentScopePredicate(Func<TRuntime, bool> predicate)
    {
        this.currentScopePredicate = predicate;
    }

    /// <inheritdoc/>
    public void UseGlobalScopePredicate(Func<TRuntime, bool> predicate)
    {
        this.globalScopePredicate = predicate;
    }

    /// <inheritdoc/>
    public void UseOperations(Func<TRuntime, AllureInProcessOperations> operationsFactory)
    {
        this.currentOperationsFactory = operationsFactory;
    }

    /// <inheritdoc/>
    public void UseParameterSerializer(Func<TRuntime, IAllureParameterSerializer> serializerFactory)
    {
        this.useRuleBasedSerializer = false;
        this.currentRuleBasedSerializerRegistrations.Clear();
        this.currentSerializerFactory = serializerFactory;
    }

    /// <inheritdoc/>
    public void UseParameterSerializer(Func<IAllureParameterSerializer> serializerFactory) =>
        this.UseParameterSerializer((_) => serializerFactory());

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public void ConfigureSerialization(Action<IParameterSerializationRulesContext> registration)
    {
        this.ConfigureSerialization((_, ctx) => registration(ctx));
    }

    /// <summary>
    /// Runs the configured endpoint hooks and constructs the route.
    /// </summary>
    /// <returns>The constructed in-process runtime route.</returns>
    public IAllureRuntimeRoute Build()
    {
        this.registration(this.Runtime, this);
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
