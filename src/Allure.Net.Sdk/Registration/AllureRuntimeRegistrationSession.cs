using System;
using System.Collections.Generic;
using System.Linq;
using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Internal.Registration;
using Allure.Sdk.Internal.Runtime;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Defines the base contract for a single-use runtime registration session.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntimeIntegrationContext">The integration context type.</typeparam>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
public abstract class AllureRuntimeRegistrationSession<
    TConfiguration,
    TRuntimeIntegrationContext,
    TRuntime
>
    where TConfiguration : AllureConfiguration
    where TRuntime : IAllureRuntime<TConfiguration>
{
    internal abstract IPreparedRuntimeRegistration<TConfiguration, TRuntime> Prepare(
        string runtimeName,
        Action<TRuntimeIntegrationContext> registration
    );
}

/// <summary>
/// Provides a single-use registration session for a custom Allure runtime and its
/// optional in-process endpoint.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntimeRegistrationContext">The runtime registration context type.</typeparam>
/// <typeparam name="TRuntimeHook">The runtime registration hook type.</typeparam>
/// <typeparam name="TEndpointRegistrationContext">The endpoint registration context type.</typeparam>
/// <typeparam name="TEndpointHook">The endpoint registration hook type.</typeparam>
/// <typeparam name="TRuntimeIntegrationContext">The integration context type.</typeparam>
/// <typeparam name="TIntegrationSnapshot">The integration snapshot type.</typeparam>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
public abstract class AllureRuntimeRegistrationSession<
    TConfiguration,
    TRuntimeRegistrationContext,
    TRuntimeHook,
    TEndpointRegistrationContext,
    TEndpointHook,
    TRuntimeIntegrationContext,
    TIntegrationSnapshot,
    TRuntime
> :
    AllureRuntimeRegistrationSession<TConfiguration, TRuntimeIntegrationContext, TRuntime>,
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
    where TRuntimeHook : IAllureRuntimeRegistrationHook<
        TConfiguration,
        TRuntimeRegistrationContext
    >
    where TEndpointRegistrationContext : IAllureInProcessEndpointRegistrationContext<
        TConfiguration,
        TRuntime
    >
    where TEndpointHook : IAllureInProcessEndpointRegistrationHook<
        TConfiguration,
        TEndpointRegistrationContext,
        TRuntime
    >
    where TRuntimeIntegrationContext : IAllureRuntimeIntegrationContext<
        TConfiguration,
        TRuntimeRegistrationContext,
        TRuntimeHook,
        TEndpointRegistrationContext,
        TEndpointHook,
        TRuntime
    >
    where TIntegrationSnapshot : IAllureRuntimeIntegrationSnapshot<
        TConfiguration,
        TEndpointRegistrationContext,
        TEndpointHook,
        TRuntime
    >
    where TRuntime : IAllureRuntime<TConfiguration>
{
    readonly object gate = new();

    RegistrationState state = RegistrationState.Created;

    Func<IEnumerable<IAllureConfigurationSource<TConfiguration>>> currentConfigurationSourcesFactory =
        AllureRegistrationDefaults.ConfigurationSources<TConfiguration>();

    readonly List<
        Func<
            TrackedConfiguration<TConfiguration>,
            TrackedConfiguration<TConfiguration>
        >
    > configurationTransformations = [];

    Func<TConfiguration, IEnumerable<TRuntimeHook?>> currentHooksFactory =
        AllureRegistrationDefaults.RuntimeHookProviders<
            TConfiguration,
            TRuntimeRegistrationContext,
            TRuntimeHook
        >();

    Func<
        RuntimeServiceCreationContext<TConfiguration>,
        IAllureExecutionContext
    > currentContextFactory =
        (ctx) => new AsyncLocalExecutionContext(ctx.RuntimeReference);

    Func<
        RuntimeServiceCreationContext<TConfiguration>,
        IAllureLifecycleApi
    > currentLifecycleApiFactory =
        (ctx) => new RuntimeLifecycleApi(ctx.RuntimeReference);

    Func<
        RuntimeServiceCreationContext<TConfiguration>,
        IAllureModelApi
    > currentModelApiFactory =
        (ctx) => new RuntimeModelApi(ctx.RuntimeReference);

    bool useRuleBasedSerializer = true;

    Func<TConfiguration, IAllureParameterSerializer> currentSerializerFactory;

    Func<TConfiguration, IAllureResultsDestination> currentDestinationFactory =
        AllureRegistrationDefaults.Destination<TConfiguration>();

    readonly List<
        Action<
            TConfiguration,
            IParameterSerializationRulesContext
        >
    > currentRuleBasedSerializerRegistrations = [];

    AllureInProcessEndpointRegistration<
        TConfiguration,
        TEndpointRegistrationContext,
        TEndpointHook,
        TRuntime
    >? currentEndpointRegistration = null;

    /// <summary>
    /// Initializes a runtime registration session with the default component factories.
    /// </summary>
    public AllureRuntimeRegistrationSession() : base()
    {
        this.currentSerializerFactory = AllureRegistrationDefaults.ParameterSerializer(
            this.currentRuleBasedSerializerRegistrations
        );
    }

    /// <inheritdoc/>
    public void UseRegistrationHooks(Func<TConfiguration, IEnumerable<TRuntimeHook?>> hooksFactory) =>
        this.Modify(() => this.currentHooksFactory = hooksFactory);

    /// <inheritdoc/>
    public void UseContext(Func<TConfiguration, IAllureExecutionContext> contextFactory) =>
        this.Modify(() => this.currentContextFactory = (ctx) => contextFactory(ctx.Configuration));

    /// <inheritdoc/>
    public void UseLifecycleApi(Func<TConfiguration, IAllureLifecycleApi> lifecycleApiFactory) =>
        this.Modify(() => this.currentLifecycleApiFactory = (ctx) => lifecycleApiFactory(ctx.Configuration));

    /// <inheritdoc/>
    public void UseModelApi(Func<TConfiguration, IAllureModelApi> modelApiFactory) =>
        this.Modify(() => this.currentModelApiFactory = (ctx) => modelApiFactory(ctx.Configuration));

    /// <inheritdoc/>
    public void RegisterInProcessEndpoint(
        string endpointId,
        Action<
            TRuntime,
            IAllureInProcessEndpointIntegrationContext<
                TConfiguration,
                TEndpointRegistrationContext,
                TEndpointHook,
                TRuntime
            >
        > endpointRegistration
    ) =>
        this.Modify(() => this.currentEndpointRegistration = new(endpointId, endpointRegistration));

    /// <inheritdoc/>
    public void UseConfigurationSources(Func<IEnumerable<IAllureConfigurationSource<TConfiguration>>> sourcesFactory) =>
        this.Modify(() => this.currentConfigurationSourcesFactory = sourcesFactory);

    /// <inheritdoc/>
    public void TransformConfiguration(
        Func<TrackedConfiguration<TConfiguration>, TrackedConfiguration<TConfiguration>> transformation
    ) =>
        this.Modify(() => this.configurationTransformations.Add(transformation));

    /// <inheritdoc/>
    public void ConfigureSerialization(Action<TConfiguration, IParameterSerializationRulesContext> registration) =>
        this.Modify(() =>
        {
            if (!this.useRuleBasedSerializer)
            {
                this.currentSerializerFactory = AllureRegistrationDefaults.ParameterSerializer(
                    this.currentRuleBasedSerializerRegistrations
                );
                this.useRuleBasedSerializer = true;
            }

            this.currentRuleBasedSerializerRegistrations.Add(registration);
        });

    /// <inheritdoc/>
    public void UseParameterSerializer(Func<TConfiguration, IAllureParameterSerializer> serializerFactory) =>
        this.Modify(() =>
        {
            this.useRuleBasedSerializer = false;
            this.currentRuleBasedSerializerRegistrations.Clear();
            this.currentSerializerFactory = serializerFactory;
        });

    /// <inheritdoc/>
    public void UseDestination(Func<TConfiguration, IAllureResultsDestination> destinationFactory) =>
        this.Modify(() => this.currentDestinationFactory = destinationFactory);

    /// <inheritdoc/>
    public void UseParameterSerializer(Func<IAllureParameterSerializer> serializerFactory) =>
        this.UseParameterSerializer((_) => serializerFactory());

    /// <inheritdoc/>
    public void ConfigureSerialization(Action<IParameterSerializationRulesContext> registration) =>
        this.ConfigureSerialization((_, context) => registration(context));

    /// <summary>
    /// Applies a modification while the registration context is active.
    /// </summary>
    /// <param name="action">The modification to apply.</param>
    /// <exception cref="InvalidOperationException">
    /// The registration context is not active.
    /// </exception>
    protected void Modify(Action action)
    {
        lock (this.gate)
        {
            this.EnsureCanModify();

            try
            {
                action();
            }
            catch
            {
                this.state = RegistrationState.Failed;
                throw;
            }
        }
    }

    /// <summary>
    /// Gets the integration context passed to the registration action.
    /// </summary>
    protected abstract TRuntimeIntegrationContext IntegrationContext { get; }

    /// <summary>
    /// Gets the integration-specific context passed to runtime registration
    /// hooks.
    /// </summary>
    protected abstract TRuntimeRegistrationContext RegistrationContext { get; }

    /// <summary>
    /// Captures the integration-specific factories required after the session closes.
    /// </summary>
    /// <returns>The immutable integration snapshot.</returns>
    protected abstract TIntegrationSnapshot CaptureIntegrationSnapshot();

    internal override IPreparedRuntimeRegistration<TConfiguration, TRuntime> Prepare(
        string runtimeName,
        Action<TRuntimeIntegrationContext> registration
    )
    {
        lock (this.gate)
        {
            this.OpenRegistration();

            try
            {
                registration(this.IntegrationContext);

                var initialConfiguration = this.ResolveConfiguration();

                var finalConfiguration = this.RunHooks(initialConfiguration);

                this.CloseRegistration();

                var commonSnapshot = this.CaptureCommonSnapshot();
                var integrationSnapshot = this.CaptureIntegrationSnapshot();


                return new PreparedRuntimeRegistration<
                    TConfiguration,
                    TEndpointRegistrationContext,
                    TEndpointHook,
                    TRuntime,
                    TIntegrationSnapshot
                >(
                    runtimeName,
                    finalConfiguration.Configuration,
                    commonSnapshot,
                    integrationSnapshot
                );
            }
            catch
            {
                this.state = RegistrationState.Failed;
                throw;
            }
        }
    }

    AllureRuntimeRegistrationSnapshot<
        TConfiguration,
        TEndpointRegistrationContext,
        TEndpointHook,
        TRuntime
    > CaptureCommonSnapshot() => new(
        ContextFactory: this.currentContextFactory,
        LifecycleApiFactory: this.currentLifecycleApiFactory,
        ModelApiFactory: this.currentModelApiFactory,
        UseRuleBasedSerializer: this.useRuleBasedSerializer,
        SerializerFactory: this.currentSerializerFactory,
        DestinationFactory: this.currentDestinationFactory,
        RuleBasedSerializerRegistrations: [.. this.currentRuleBasedSerializerRegistrations],
        EndpointRegistration: this.currentEndpointRegistration
    );

    void OpenRegistration()
    {
        if (this.state is not RegistrationState.Created)
        {
            throw new InvalidOperationException(
                "The registration session has already been used."
            );
        }

        this.state = RegistrationState.Opened;
    }

    void CloseRegistration()
    {
        if (this.state is not RegistrationState.Opened)
        {
            throw new InvalidOperationException(
                "The registration context is not active."
            );
        }

        this.state = RegistrationState.Closed;
    }

    void EnsureCanModify()
    {
        if (this.state != RegistrationState.Opened)
        {
            throw new InvalidOperationException(
                "The registration context is not active."
            );
        }
    }

    TrackedConfiguration<TConfiguration> ResolveConfiguration() =>
        this.ApplyConfigurationTransformations(
            this.LoadConfiguration()
        );

    TrackedConfiguration<TConfiguration> LoadConfiguration()
    {
        foreach (var source in this.currentConfigurationSourcesFactory())
        {
            if (source.CanLoad)
            {
                return source.LoadConfiguration();
            }
        }

        return new TrackedConfiguration<TConfiguration>(
            "default",
            new()
        );
    }

    TrackedConfiguration<TConfiguration> RunHooks(
        TrackedConfiguration<TConfiguration> initialConfiguration
    )
    {
        var configurationSourcesFactoryBefore = this.currentConfigurationSourcesFactory;
        var transformationCount = configurationTransformations.Count;

        foreach (var provider in this.currentHooksFactory(initialConfiguration.Configuration))
        {
            provider?.SetUp(this.RegistrationContext);
        }

        return ReferenceEquals(configurationSourcesFactoryBefore, this.currentConfigurationSourcesFactory)
            ? this.ApplyRemainingTransformations(initialConfiguration, transformationCount)
            : this.ResolveConfiguration();
    }

    TrackedConfiguration<TConfiguration> ApplyRemainingTransformations(
        TrackedConfiguration<TConfiguration> loadedConfiguration,
        int skip
    ) =>
        ApplyConfigurationTransformations(
            loadedConfiguration,
            this.configurationTransformations.Skip(skip)
        );

    TrackedConfiguration<TConfiguration> ApplyConfigurationTransformations(
        TrackedConfiguration<TConfiguration> loadedConfiguration
    ) =>
        ApplyConfigurationTransformations(
            loadedConfiguration,
            this.configurationTransformations
        );

    static TrackedConfiguration<TConfiguration> ApplyConfigurationTransformations(
        TrackedConfiguration<TConfiguration> loadedConfiguration,
        IEnumerable<Func<TrackedConfiguration<TConfiguration>, TrackedConfiguration<TConfiguration>>> transformations
    )
    {
        TrackedConfiguration<TConfiguration> transformedConfiguration = loadedConfiguration;
        foreach (var transformation in transformations)
        {
            transformedConfiguration = transformation(transformedConfiguration);
        }
        return transformedConfiguration;
    }

    enum RegistrationState
    {
        Created,
        Opened,
        Closed,
        Failed,
    }
}

/// <summary>
/// Provides a single-use registration session for a standard Allure runtime with
/// custom configuration, registration, and endpoint types.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntimeRegistrationContext">The runtime registration context type.</typeparam>
/// <typeparam name="TRuntimeHook">The runtime registration hook type.</typeparam>
/// <typeparam name="TEndpointRegistrationContext">The endpoint registration context type.</typeparam>
/// <typeparam name="TEndpointHook">The endpoint registration hook type.</typeparam>
/// <typeparam name="TRuntimeIntegrationContext">The integration context type.</typeparam>
/// <typeparam name="TIntegrationSnapshot">The integration snapshot type.</typeparam>
public abstract class AllureRuntimeRegistrationSession<
    TConfiguration,
    TRuntimeRegistrationContext,
    TRuntimeHook,
    TEndpointRegistrationContext,
    TEndpointHook,
    TRuntimeIntegrationContext,
    TIntegrationSnapshot
> :
    AllureRuntimeRegistrationSession<
        TConfiguration,
        TRuntimeRegistrationContext,
        TRuntimeHook,
        TEndpointRegistrationContext,
        TEndpointHook,
        TRuntimeIntegrationContext,
        TIntegrationSnapshot,
        IAllureRuntime<TConfiguration>
    >,
    IAllureRuntimeIntegrationContext<
        TConfiguration,
        TRuntimeRegistrationContext,
        TRuntimeHook,
        TEndpointRegistrationContext,
        TEndpointHook
    >

    where TConfiguration : AllureConfiguration, new()
    where TRuntimeRegistrationContext : IAllureRuntimeRegistrationContext<TConfiguration>
    where TRuntimeHook : IAllureRuntimeRegistrationHook<
        TConfiguration,
        TRuntimeRegistrationContext
    >
    where TEndpointRegistrationContext : IAllureInProcessEndpointRegistrationContext<
        TConfiguration
    >
    where TEndpointHook : IAllureInProcessEndpointRegistrationHook<
        TConfiguration,
        TEndpointRegistrationContext
    >
    where TRuntimeIntegrationContext : IAllureRuntimeIntegrationContext<
        TConfiguration,
        TRuntimeRegistrationContext,
        TRuntimeHook,
        TEndpointRegistrationContext,
        TEndpointHook
    >
    where TIntegrationSnapshot : IAllureRuntimeIntegrationSnapshot<
        TConfiguration,
        TEndpointRegistrationContext,
        TEndpointHook
    >;

/// <summary>
/// Provides a single-use registration session for a standard Allure runtime and its
/// optional in-process endpoint.
/// </summary>
public class AllureRuntimeRegistrationSession :
    AllureRuntimeRegistrationSession<
        AllureConfiguration,
        IAllureRuntimeRegistrationContext,
        IAllureRuntimeRegistrationHook,
        IAllureInProcessEndpointRegistrationContext,
        IAllureInProcessEndpointRegistrationHook,
        IAllureRuntimeIntegrationContext,
        IAllureRuntimeIntegrationSnapshot
    >,
    IAllureRuntimeIntegrationContext
{
    /// <inheritdoc/>
    protected override IAllureRuntimeIntegrationContext IntegrationContext => this;

    /// <inheritdoc/>
    protected override IAllureRuntimeRegistrationContext RegistrationContext => this;

    /// <inheritdoc/>
    protected override IAllureRuntimeIntegrationSnapshot CaptureIntegrationSnapshot() =>
        new AllureRuntimeIntegrationSnapshot();
}
