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
public abstract class AllureInProcessRouteBuilder<TConfiguration, TContext, THook> :
    IAllureInProcessEndpointIntegrationContext<TConfiguration, TContext, THook>

    where TConfiguration : AllureConfiguration
    where TContext : IAllureInProcessEndpointRegistrationContext<TConfiguration>
    where THook : IAllureInProcessEndpointRegistrationHook<TConfiguration, TContext>
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

    /// <summary>
    /// Initializes an in-process route builder from the components resolved by
    /// its runtime builder.
    /// </summary>
    /// <param name="args">The resolved route builder arguments.</param>
    public AllureInProcessRouteBuilder(AllureRouteBuilderArgs<TConfiguration> args)
    {
        this.useRuleBasedSerializer = args.UseRuleBasedSerializer;
        this.runtimeName = args.RuntimeName;
        this.routeId = args.RouteId;
        this.runtime = args.Runtime;
        this.currentRuleBasedSerializerRegistrations = [.. args.RuleBasedSerializerRegistrations];
        this.currentSerializerFactory =
            useRuleBasedSerializer
                ? (runtime) =>
                    AllureRegistrationDefaults.ParameterSerializer<TConfiguration>(
                        currentRuleBasedSerializerRegistrations
                    )(runtime.Configuration)
                : (_) => runtime.ParameterSerializer;
    }

    /// <inheritdoc/>
    public void UseRegistrationHooks(
        Func<TConfiguration, IEnumerable<THook?>> hooksFactory
    )
    {
        this.currentHooksFactory = hooksFactory;
    }

    /// <inheritdoc/>
    public void SetAvailabilityPredicate(Func<IAllureRuntime<TConfiguration>, bool> isAvailable)
    {
        this.availabilityPredicate = isAvailable;
    }

    /// <inheritdoc/>
    public void SetAvailabilityPredicate(Func<bool> isAvailable)
    {
        this.availabilityPredicate = (_) => isAvailable();
    }

    /// <inheritdoc/>
    public void SuppressRoutes(Func<IAllureRuntime<TConfiguration>, IEnumerable<string>> suppressedRouteIdsFactory)
    {
        this.currentSuppressedRouteIdsFactory = suppressedRouteIdsFactory;
    }

    /// <inheritdoc/>
    public void SuppressRoutes(Func<IEnumerable<string>> suppressedRouteIdsFactory)
    {
        this.currentSuppressedRouteIdsFactory = (_) => suppressedRouteIdsFactory();
    }

    /// <inheritdoc/>
    public void UseCurrentScopePredicate(Func<IAllureRuntime<TConfiguration>, bool> predicate)
    {
        this.currentScopePredicate = predicate;
    }

    /// <inheritdoc/>
    public void UseGlobalScopePredicate(Func<IAllureRuntime<TConfiguration>, bool> predicate)
    {
        this.globalScopePredicate = predicate;
    }

    /// <inheritdoc/>
    public void UseOperations(Func<IAllureRuntime<TConfiguration>, AllureInProcessOperations> operationsFactory)
    {
        this.currentOperationsFactory = operationsFactory;
    }

    /// <inheritdoc/>
    public void UseParameterSerializer(Func<IAllureRuntime<TConfiguration>, IAllureParameterSerializer> serializerFactory)
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

    /// <summary>
    /// Gets the integration-specific context passed to endpoint registration
    /// hooks.
    /// </summary>
    protected abstract TContext RegistrationContext { get; }

    void RunHooks()
    {
        foreach (var hook in this.currentHooksFactory(this.runtime.Configuration))
        {
            hook?.SetUp(this.RegistrationContext);
        }
    }
}

class AllureInProcessRouteBuilder(
    AllureRouteBuilderArgs<AllureConfiguration> args
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
