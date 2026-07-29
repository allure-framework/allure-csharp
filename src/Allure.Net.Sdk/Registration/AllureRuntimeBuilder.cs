using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Runtime;
using Allure.Sdk.Configuration;
using Allure.Sdk.Internal.Registration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Provides the base implementation for builders that construct a custom Allure
/// runtime and its optional in-process endpoint.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntimeRegistrationContext">The runtime registration context type.</typeparam>
/// <typeparam name="TRuntimeHook">The runtime registration hook type.</typeparam>
/// <typeparam name="TEndpointRegistrationContext">The endpoint registration context type.</typeparam>
/// <typeparam name="TEndpointHook">The endpoint registration hook type.</typeparam>
/// <typeparam name="TRuntime">The type of runtime constructed by the builder.</typeparam>
public abstract class AllureRuntimeBuilder<
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
    >

    where TConfiguration : AllureConfiguration, new()
    where TRuntimeRegistrationContext : IAllureRuntimeRegistrationContext<TConfiguration>
    where TRuntimeHook : IAllureRuntimeRegistrationHook<TConfiguration, TRuntimeRegistrationContext>
    where TEndpointRegistrationContext : IAllureInProcessEndpointRegistrationContext<TConfiguration>
    where TEndpointHook : IAllureInProcessEndpointRegistrationHook<TConfiguration, TEndpointRegistrationContext>
    where TRuntime : IAllureRuntime<TConfiguration>
{
    readonly string runtimeName;

    Func<IEnumerable<IAllureConfigurationSource<TConfiguration>>> currentConfigurationSourcesFactory =
        AllureRegistrationDefaults.ConfigurationSources<TConfiguration>();

    Func<TConfiguration, IEnumerable<TRuntimeHook?>> currentHooksFactory =
        AllureRegistrationDefaults.RuntimeHookProviders<TConfiguration, TRuntimeRegistrationContext, TRuntimeHook>();

    Func<IAllureRegistrationDependencies<TConfiguration>, IAllureExecutionContext> currentContextFactory =
        AllureRegistrationDefaults.Context<TConfiguration>();

    Func<IAllureRegistrationDependencies<TConfiguration>, IAllureLifecycleApi> currentLifecycleApiFactory =
        AllureRegistrationDefaults.LifecycleApi<TConfiguration>();

    Func<IAllureRegistrationDependencies<TConfiguration>, IAllureModelApi> currentModelApiFactory =
        AllureRegistrationDefaults.ModelApi<TConfiguration>();

    bool useRuleBasedSerializer = true;

    Func<TConfiguration, IAllureParameterSerializer> currentSerializerFactory;

    Func<TConfiguration, IAllureResultsDestination> currentDestinationFactory =
        AllureRegistrationDefaults.Destination<TConfiguration>();

    readonly List<Action<TConfiguration, IParameterSerializationRulesContext>> currentRuleBasedSerializerRegistrations = [];

    (
        string id,
        Action<TRuntime, IAllureInProcessEndpointIntegrationContext<TConfiguration, TEndpointRegistrationContext, TEndpointHook>>
    )? currentEndpointRegistration = null;

    /// <summary>
    /// Initializes a runtime builder with the specified display name.
    /// </summary>
    /// <param name="runtimeName">The runtime display name.</param>
    public AllureRuntimeBuilder(string runtimeName)
    {
        this.currentSerializerFactory = AllureRegistrationDefaults.ParameterSerializer(
            this.currentRuleBasedSerializerRegistrations
        );
        this.runtimeName = runtimeName;
    }

    /// <inheritdoc/>
    public void UseConfigurationSources(Func<IEnumerable<IAllureConfigurationSource<TConfiguration>>> sourcesFactory)
    {
        this.currentConfigurationSourcesFactory = sourcesFactory;
    }

    /// <inheritdoc/>
    public void UseRegistrationHooks(Func<TConfiguration, IEnumerable<TRuntimeHook?>> hookFactory)
    {
        this.currentHooksFactory = hookFactory;
    }

    /// <inheritdoc/>
    public void UseContext(Func<IAllureRegistrationDependencies<TConfiguration>, IAllureExecutionContext> contextFactory)
    {
        this.currentContextFactory = contextFactory;
    }

    /// <inheritdoc/>
    public void UseDestination(Func<TConfiguration, IAllureResultsDestination> destinationFactory)
    {
        this.currentDestinationFactory = destinationFactory;
    }

    /// <inheritdoc/>
    public void UseLifecycleApi(Func<IAllureRegistrationDependencies<TConfiguration>, IAllureLifecycleApi> lifecycleApiFactory)
    {
        this.currentLifecycleApiFactory = lifecycleApiFactory;
    }

    /// <inheritdoc/>
    public void UseModelApi(Func<IAllureRegistrationDependencies<TConfiguration>, IAllureModelApi> modelApiFactory)
    {
        this.currentModelApiFactory = modelApiFactory;
    }

    /// <inheritdoc/>
    public void ConfigureSerialization(Action<TConfiguration, IParameterSerializationRulesContext> registration)
    {
        if (!this.useRuleBasedSerializer)
        {
            this.currentSerializerFactory = AllureRegistrationDefaults.ParameterSerializer(
                this.currentRuleBasedSerializerRegistrations
            );
            this.useRuleBasedSerializer = true;
        }

        this.currentRuleBasedSerializerRegistrations.Add(registration);
    }

    /// <inheritdoc/>
    public void ConfigureSerialization(Action<IParameterSerializationRulesContext> registration) =>
        this.ConfigureSerialization((_, context) => registration(context));

    /// <inheritdoc/>
    public void UseParameterSerializer(Func<TConfiguration, IAllureParameterSerializer> serializerFactory)
    {
        this.useRuleBasedSerializer = false;
        this.currentRuleBasedSerializerRegistrations.Clear();
        this.currentSerializerFactory = serializerFactory;
    }

    /// <inheritdoc/>
    public void UseParameterSerializer(Func<IAllureParameterSerializer> serializerFactory)
    {
        this.UseParameterSerializer((_) => serializerFactory());
    }

    /// <inheritdoc/>
    public void RegisterInProcessEndpoint(
        string endpointId,
        Action<TRuntime, IAllureInProcessEndpointIntegrationContext<TConfiguration, TEndpointRegistrationContext, TEndpointHook>> endpointRegistration
    )
    {
        this.currentEndpointRegistration = (endpointId, endpointRegistration);
    }

    /// <summary>
    /// Resolves configuration, runs registration hooks, constructs the runtime,
    /// and installs its configured endpoint.
    /// </summary>
    /// <returns>The constructed runtime.</returns>
    public TRuntime Build()
    {

        var configuration = this.RunHooks();

        var parameterSerializer = this.currentSerializerFactory(configuration);
        var destination = this.currentDestinationFactory(configuration);

        var dependencies = new AllureRegistrationDependencies<TConfiguration>(
            configuration,
            parameterSerializer,
            new LateBoundReference<IAllureRuntime<TConfiguration>>()
        );

        var context = this.currentContextFactory(dependencies);
        var lifecycleApi = this.currentLifecycleApiFactory(dependencies);
        var modelApi = this.currentModelApiFactory(dependencies);

        var runtime = this.CreateRuntimeInstance(
            configuration,
            parameterSerializer,
            destination,
            context,
            lifecycleApi,
            modelApi
        );

        dependencies.BindRuntime(runtime);

        if (this.currentEndpointRegistration is var (routeId, routeRegistration))
        {
            var endpointRouteBuilder = this.CreateRouteBuilder(new(
                this.runtimeName,
                routeId,
                runtime,
                this.useRuleBasedSerializer,
                this.currentRuleBasedSerializerRegistrations
            ));
            routeRegistration(runtime, endpointRouteBuilder);
            var route = endpointRouteBuilder.Build();
            AllureRuntimeRouter.Install(route);
        }

        return runtime;
    }

    /// <summary>
    /// Creates the runtime instance from the resolved components.
    /// </summary>
    /// <param name="configuration">The resolved runtime configuration.</param>
    /// <param name="parameterSerializer">The configured parameter serializer.</param>
    /// <param name="destination">The configured results destination.</param>
    /// <param name="context">The configured execution-context service.</param>
    /// <param name="lifecycleApi">The configured lifecycle API.</param>
    /// <param name="modelApi">The configured model API.</param>
    /// <returns>The constructed runtime instance.</returns>
    protected abstract TRuntime CreateRuntimeInstance(
        TConfiguration configuration,
        IAllureParameterSerializer parameterSerializer,
        IAllureResultsDestination destination,
        IAllureExecutionContext context,
        IAllureLifecycleApi lifecycleApi,
        IAllureModelApi modelApi
    );

    /// <summary>
    /// Gets the integration-specific context passed to runtime registration
    /// hooks.
    /// </summary>
    protected abstract TRuntimeRegistrationContext RegistrationContext { get; }

    /// <summary>
    /// Creates the integration-specific in-process route builder.
    /// </summary>
    /// <param name="args">The resolved route builder arguments.</param>
    /// <returns>The route builder used to configure and construct the endpoint route.</returns>
    protected abstract AllureInProcessRouteBuilder<TConfiguration, TEndpointRegistrationContext, TEndpointHook> CreateRouteBuilder(
        AllureRouteBuilderArgs<TConfiguration> args
    );

    TConfiguration ResolveConfiguration()
    {
        foreach (var source in this.currentConfigurationSourcesFactory())
        {
            if (source.CanLoad)
            {
                return source.LoadConfiguration();
            }
        }

        return new TConfiguration();
    }

    TConfiguration RunHooks()
    {
        var preHookConfiguration = this.ResolveConfiguration();
        var configurationSourcesFactoryBefore = this.currentConfigurationSourcesFactory;

        foreach (var provider in this.currentHooksFactory(preHookConfiguration))
        {
            provider?.SetUp(this.RegistrationContext);
        }

        return ReferenceEquals(configurationSourcesFactoryBefore, this.currentConfigurationSourcesFactory)
            ? preHookConfiguration
            : this.ResolveConfiguration();
    }
}

/// <summary>
/// Configures and constructs a standard Allure runtime with a custom
/// configuration type, custom registration hook types, and optional
/// in-process endpoint.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntimeRegistrationContext">The runtime registration context type.</typeparam>
/// <typeparam name="TRuntimeHook">The runtime registration hook type.</typeparam>
/// <typeparam name="TEndpointRegistrationContext">The endpoint registration context type.</typeparam>
/// <typeparam name="TEndpointHook">The endpoint registration hook type.</typeparam>
/// <param name="runtimeName">The runtime display name.</param>
public abstract class AllureRuntimeBuilder<
    TConfiguration,
    TRuntimeRegistrationContext,
    TRuntimeHook,
    TEndpointRegistrationContext,
    TEndpointHook
>(string runtimeName) :
    AllureRuntimeBuilder<
        TConfiguration,
        TRuntimeRegistrationContext,
        TRuntimeHook,
        TEndpointRegistrationContext,
        TEndpointHook,
        IAllureRuntime<TConfiguration>
    >(runtimeName),
    IAllureRuntimeIntegrationContext<TConfiguration, TRuntimeRegistrationContext, TRuntimeHook, TEndpointRegistrationContext, TEndpointHook>

    where TConfiguration : AllureConfiguration, new()
    where TRuntimeRegistrationContext : IAllureRuntimeRegistrationContext<TConfiguration>
    where TRuntimeHook : IAllureRuntimeRegistrationHook<TConfiguration, TRuntimeRegistrationContext>
    where TEndpointRegistrationContext : IAllureInProcessEndpointRegistrationContext<TConfiguration>
    where TEndpointHook : IAllureInProcessEndpointRegistrationHook<TConfiguration, TEndpointRegistrationContext>
{
    /// <inheritdoc/>
    protected override IAllureRuntime<TConfiguration> CreateRuntimeInstance(
        TConfiguration configuration,
        IAllureParameterSerializer parameterSerializer,
        IAllureResultsDestination destination,
        IAllureExecutionContext context,
        IAllureLifecycleApi lifecycleApi,
        IAllureModelApi modelApi
    ) => new AllureRuntime<TConfiguration>(
        configuration,
        parameterSerializer,
        destination,
        context,
        lifecycleApi,
        modelApi
    );
}

/// <summary>
/// Configures and constructs a standard Allure runtime and its optional
/// in-process endpoint.
/// </summary>
/// <param name="runtimeName">The runtime display name.</param>
public sealed class AllureRuntimeBuilder(string runtimeName) :
    AllureRuntimeBuilder<
        AllureConfiguration,
        IAllureRuntimeRegistrationContext,
        IAllureRuntimeRegistrationHook,
        IAllureInProcessEndpointRegistrationContext,
        IAllureInProcessEndpointRegistrationHook,
        IAllureRuntime<AllureConfiguration>
    >(runtimeName),
    IAllureRuntimeIntegrationContext
{
    /// <inheritdoc/>
    protected override IAllureRuntime<AllureConfiguration> CreateRuntimeInstance(
        AllureConfiguration configuration,
        IAllureParameterSerializer parameterSerializer,
        IAllureResultsDestination destination,
        IAllureExecutionContext context,
        IAllureLifecycleApi lifecycleApi,
        IAllureModelApi modelApi
    ) => new AllureRuntime<AllureConfiguration>(
        configuration,
        parameterSerializer,
        destination,
        context,
        lifecycleApi,
        modelApi
    );

    /// <inheritdoc/>
    protected override IAllureRuntimeRegistrationContext RegistrationContext => this;

    /// <inheritdoc/>
    protected override AllureInProcessRouteBuilder<
        AllureConfiguration,
        IAllureInProcessEndpointRegistrationContext,
        IAllureInProcessEndpointRegistrationHook
    > CreateRouteBuilder(AllureRouteBuilderArgs<AllureConfiguration> args) =>
        new AllureInProcessRouteBuilder(args);
}
