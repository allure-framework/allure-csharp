using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Internal.Runtime;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Provides the base implementation for builders that construct an in-process
/// endpoint route with an integration-specific registration context and hook.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TContext">The endpoint registration context type.</typeparam>
/// <typeparam name="THook">The endpoint registration hook type.</typeparam>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
public abstract class AllureInProcessRouteBuilder<TConfiguration, TContext, THook, TRuntime> :
    IAllureInProcessEndpointIntegrationContext<TConfiguration, TContext, THook, TRuntime>

    where TConfiguration : AllureConfiguration
    where TContext : IAllureInProcessEndpointRegistrationContext<TConfiguration, TRuntime>
    where THook : IAllureInProcessEndpointRegistrationHook<TConfiguration, TContext, TRuntime>
    where TRuntime : IAllureRuntime<TConfiguration>
{
    readonly string runtimeName;

    readonly string routeId;

    Func<TConfiguration, IEnumerable<THook?>> currentHooksFactory =
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

    readonly List<Action<TConfiguration, IParameterSerializationRulesContext>> currentRuleBasedSerializerRegistrations;

    /// <summary>
    /// Gets the integration-specific runtime associated with this route.
    /// </summary>
    protected TRuntime Runtime { get; }

    /// <summary>
    /// Initializes an in-process route builder from the components resolved by
    /// its runtime builder.
    /// </summary>
    /// <param name="args">The resolved route builder arguments.</param>
    public AllureInProcessRouteBuilder(AllureRouteBuilderArgs<TConfiguration, TRuntime> args)
    {
        this.useRuleBasedSerializer = args.UseRuleBasedSerializer;
        this.runtimeName = args.RuntimeName;
        this.routeId = args.RouteId;
        this.Runtime = args.Runtime;
        this.currentRuleBasedSerializerRegistrations = [.. args.RuleBasedSerializerRegistrations];
        this.currentSerializerFactory =
            useRuleBasedSerializer
                ? (runtime) =>
                    AllureRegistrationDefaults.ParameterSerializer<TConfiguration>(
                        currentRuleBasedSerializerRegistrations
                    )(runtime.Configuration)
                : (_) => this.Runtime.ParameterSerializer;
    }

    /// <inheritdoc/>
    public void UseRegistrationHooks(
        Func<TConfiguration, IEnumerable<THook?>> hooksFactory
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

    /// <summary>
    /// Gets the integration-specific context passed to endpoint registration
    /// hooks.
    /// </summary>
    protected abstract TContext RegistrationContext { get; }

    void RunHooks()
    {
        foreach (var hook in this.currentHooksFactory(this.Runtime.Configuration))
        {
            hook?.SetUp(this.RegistrationContext);
        }
    }
}

/// <summary>
/// Provides the base implementation for builders that construct an in-process
/// endpoint route for a standard Allure runtime with an integration-specific
/// registration context and hook.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TContext">The endpoint registration context type.</typeparam>
/// <typeparam name="THook">The endpoint registration hook type.</typeparam>
/// <param name="args">The resolved route builder arguments.</param>
public abstract class AllureInProcessRouteBuilder<TConfiguration, TContext, THook>(
    AllureRouteBuilderArgs<TConfiguration, IAllureRuntime<TConfiguration>> args
) :
    AllureInProcessRouteBuilder<
        TConfiguration,
        TContext,
        THook,
        IAllureRuntime<TConfiguration>
    >(args),
    IAllureInProcessEndpointIntegrationContext<TConfiguration, TContext, THook>

    where TConfiguration : AllureConfiguration
    where TContext : IAllureInProcessEndpointRegistrationContext<TConfiguration>
    where THook : IAllureInProcessEndpointRegistrationHook<TConfiguration, TContext>;

class AllureInProcessRouteBuilder(
    AllureRouteBuilderArgs<AllureConfiguration, IAllureRuntime<AllureConfiguration>> args
) :
    AllureInProcessRouteBuilder<
        AllureConfiguration,
        IAllureInProcessEndpointRegistrationContext,
        IAllureInProcessEndpointRegistrationHook
    >(args),
    IAllureInProcessEndpointIntegrationContext
{
    /// <inheritdoc/>
    protected override IAllureInProcessEndpointRegistrationContext RegistrationContext => this;
}
