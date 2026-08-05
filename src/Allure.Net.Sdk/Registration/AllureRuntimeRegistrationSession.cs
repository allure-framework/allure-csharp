using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Internal.Registration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

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
    where TRuntime : IAllureRuntime<TConfiguration>
{
    readonly object gate = new();

    RegistrationState state = RegistrationState.Created;

    Func<IEnumerable<IAllureConfigurationSource<TConfiguration>>> currentConfigurationSourcesFactory =
        AllureRegistrationDefaults.ConfigurationSources<TConfiguration>();

    readonly List<Func<TConfiguration, TConfiguration>> configurationTransformations = [];

    Func<TConfiguration, IEnumerable<TRuntimeHook?>> currentHooksFactory =
        AllureRegistrationDefaults.RuntimeHookProviders<
            TConfiguration,
            TRuntimeRegistrationContext,
            TRuntimeHook
        >();

    Func<
        IAllureRegistrationDependencies<TConfiguration>,
        IAllureExecutionContext
    > currentContextFactory =
        AllureRegistrationDefaults.Context<TConfiguration>();

    Func<
        IAllureRegistrationDependencies<TConfiguration>,
        IAllureLifecycleApi
    > currentLifecycleApiFactory =
        AllureRegistrationDefaults.LifecycleApi<TConfiguration>();

    Func<
        IAllureRegistrationDependencies<TConfiguration>,
        IAllureModelApi
    > currentModelApiFactory =
        AllureRegistrationDefaults.ModelApi<TConfiguration>();

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

    public AllureRuntimeRegistrationSession() : base()
    {
        this.currentSerializerFactory = AllureRegistrationDefaults.ParameterSerializer(
            this.currentRuleBasedSerializerRegistrations
        );
    }

    public void UseRegistrationHooks(Func<TConfiguration, IEnumerable<TRuntimeHook?>> hooksFactory) =>
        this.Modify(() => this.currentHooksFactory = hooksFactory);

    public void UseContext(Func<IAllureRegistrationDependencies<TConfiguration>, IAllureExecutionContext> contextFactory) =>
        this.Modify(() => this.currentContextFactory = contextFactory);

    public void UseLifecycleApi(Func<IAllureRegistrationDependencies<TConfiguration>, IAllureLifecycleApi> lifecycleApiFactory) =>
        this.Modify(() => this.currentLifecycleApiFactory = lifecycleApiFactory);

    public void UseModelApi(Func<IAllureRegistrationDependencies<TConfiguration>, IAllureModelApi> modelApiFactory) =>
        this.Modify(() => this.currentModelApiFactory = modelApiFactory);

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

    public void UseConfigurationSources(Func<IEnumerable<IAllureConfigurationSource<TConfiguration>>> sourcesFactory) =>
        this.Modify(() => this.currentConfigurationSourcesFactory = sourcesFactory);

    public void TransformConfiguration(Func<TConfiguration, TConfiguration> transformation) =>
        this.Modify(() => this.configurationTransformations.Add(transformation));

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

    public void UseParameterSerializer(Func<TConfiguration, IAllureParameterSerializer> serializerFactory) =>
        this.Modify(() =>
        {
            this.useRuleBasedSerializer = false;
            this.currentRuleBasedSerializerRegistrations.Clear();
            this.currentSerializerFactory = serializerFactory;
        });

    public void UseDestination(Func<TConfiguration, IAllureResultsDestination> destinationFactory) =>
        this.Modify(() => this.currentDestinationFactory = destinationFactory);

    public void UseParameterSerializer(Func<IAllureParameterSerializer> serializerFactory) =>
        this.UseParameterSerializer((_) => serializerFactory());

    public void ConfigureSerialization(Action<IParameterSerializationRulesContext> registration) =>
        this.ConfigureSerialization((_, context) => registration(context));

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

    protected abstract TRuntimeIntegrationContext IntegrationContext { get; }

    /// <summary>
    /// Gets the integration-specific context passed to runtime registration
    /// hooks.
    /// </summary>
    protected abstract TRuntimeRegistrationContext RegistrationContext { get; }

    protected abstract TIntegrationSnapshot CaptureIntegrationSnapshot();

    protected abstract TRuntime CreateRuntime(
        RuntimeCreationArguments<TConfiguration> args,
        TIntegrationSnapshot integrationSnapshot
    );

    protected abstract AllureInProcessRouteBuilder<
        TConfiguration,
        TEndpointRegistrationContext,
        TEndpointHook,
        TRuntime
    > CreateRouteBuilder(
        AllureRouteBuilderArgs<TConfiguration, TRuntime> args,
        TIntegrationSnapshot integrationSnapshot
    );

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

                var commonSnapshot = this.CaptureCommonSnapshot();
                var integrationSnapshot = this.CaptureIntegrationSnapshot();

                this.CloseRegistration();

                return new PreparedRuntimeRegistration<
                    TConfiguration,
                    TEndpointRegistrationContext,
                    TEndpointHook,
                    TRuntime,
                    TIntegrationSnapshot
                >(
                    runtimeName,
                    finalConfiguration,
                    commonSnapshot,
                    integrationSnapshot,
                    runtimeFactory: this.CreateRuntime,
                    routeBuilderFactory: this.CreateRouteBuilder
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

    TConfiguration ResolveConfiguration() =>
        this.ApplyConfigurationTransformations(
            this.LoadConfiguration()
        );

    TConfiguration LoadConfiguration()
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

    TConfiguration RunHooks(TConfiguration initialConfiguration)
    {
        var configurationSourcesFactoryBefore = this.currentConfigurationSourcesFactory;

        foreach (var provider in this.currentHooksFactory(initialConfiguration))
        {
            provider?.SetUp(this.RegistrationContext);
        }

        return ReferenceEquals(configurationSourcesFactoryBefore, this.currentConfigurationSourcesFactory)
            ? initialConfiguration
            : this.ResolveConfiguration();
    }

    TConfiguration ApplyConfigurationTransformations(TConfiguration loadedConfiguration)
    {
        TConfiguration transformedConfiguration = loadedConfiguration;
        foreach (var transformation in this.configurationTransformations)
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

public abstract class AllureRuntimeRegistrationSession<
    TConfiguration,
    TRuntimeRegistrationContext,
    TRuntimeHook,
    TEndpointRegistrationContext,
    TEndpointHook,
    TRuntimeIntegrationContext
> :
    AllureRuntimeRegistrationSession<
        TConfiguration,
        TRuntimeRegistrationContext,
        TRuntimeHook,
        TEndpointRegistrationContext,
        TEndpointHook,
        TRuntimeIntegrationContext,
        object,
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
{
    static readonly object snapshot = new();

    protected override object CaptureIntegrationSnapshot() => snapshot;

    protected override IAllureRuntime<TConfiguration> CreateRuntime(
        RuntimeCreationArguments<TConfiguration> args,
        object _
    ) => new AllureRuntime<TConfiguration>(
        args.Configuration,
        args.ParameterSerializer,
        args.Destination,
        args.Context,
        args.LifecycleApi,
        args.ModelApi
    );
}

public class AllureRuntimeRegistrationSession :
    AllureRuntimeRegistrationSession<
        AllureConfiguration,
        IAllureRuntimeRegistrationContext,
        IAllureRuntimeRegistrationHook,
        IAllureInProcessEndpointRegistrationContext,
        IAllureInProcessEndpointRegistrationHook,
        IAllureRuntimeIntegrationContext
    >,
    IAllureRuntimeIntegrationContext
{
    protected override IAllureRuntimeIntegrationContext IntegrationContext => this;

    protected override IAllureRuntimeRegistrationContext RegistrationContext => this;

    protected override AllureInProcessRouteBuilder<
        AllureConfiguration,
        IAllureInProcessEndpointRegistrationContext,
        IAllureInProcessEndpointRegistrationHook,
        IAllureRuntime<AllureConfiguration>
    > CreateRouteBuilder(
        AllureRouteBuilderArgs<AllureConfiguration, IAllureRuntime<AllureConfiguration>> args,
        object _
    ) =>
        new AllureInProcessRouteBuilder(args);
}
