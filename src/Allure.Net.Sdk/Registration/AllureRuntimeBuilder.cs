using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Runtime;
using Allure.Sdk.Configuration;
using Allure.Sdk.Internal.Registration;
using Allure.Sdk.Internal.Runtime;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

public class AllureRuntimeBuilder<TConfiguration, TRuntimeHook, TEndpointHook>(string runtimeName) :
    IAllureIntegrationRegistrationContext<TConfiguration, TRuntimeHook, TEndpointHook>

    where TConfiguration : AllureConfiguration, new()
    where TRuntimeHook : IAllureRuntimeRegistrationHook<TConfiguration>
    where TEndpointHook : IAllureInProcessEndpointRegistrationHook<TConfiguration>
{
    Func<IEnumerable<IAllureConfigurationSource<TConfiguration>>> currentConfigurationSourcesFactory =
        AllureRegistrationDefaults.ConfigurationSources<TConfiguration>();

    Func<TConfiguration, IEnumerable<TRuntimeHook?>> currentHooksFactory =
        AllureRegistrationDefaults.RuntimeHookProviders<TConfiguration, TRuntimeHook>();

    Func<IAllureRegistrationDependencies<TConfiguration>, IAllureExecutionContext> currentContextFactory =
        AllureRegistrationDefaults.Context<TConfiguration>();

    Func<IAllureRegistrationDependencies<TConfiguration>, IAllureLifecycleApi> currentLifecycleApiFactory =
        AllureRegistrationDefaults.LifecycleApi<TConfiguration>();

    Func<IAllureRegistrationDependencies<TConfiguration>, IAllureModelApi> currentModelApiFactory =
        AllureRegistrationDefaults.ModelApi<TConfiguration>();

    Func<TConfiguration, IAllureParameterSerializer> currentSerializerFactory =
        AllureRegistrationDefaults.ParameterSerializer<TConfiguration>();

    Func<TConfiguration, IAllureResultsDestination> currentDestinationFactory =
        AllureRegistrationDefaults.Destination<TConfiguration>();

    (
        string id,
        Action<IAllureRuntime<TConfiguration>, IAllureInProcessEndpointRegistrationContext<TConfiguration, TEndpointHook>>
    )? currentEndpointRegistration = null;

    public void UseConfigurationSources(Func<IEnumerable<IAllureConfigurationSource<TConfiguration>>> sourcesFactory)
    {
        this.currentConfigurationSourcesFactory = sourcesFactory;
    }

    public void UseRegistrationHooks(Func<TConfiguration, IEnumerable<TRuntimeHook?>> hookFactory)
    {
        this.currentHooksFactory = hookFactory;
    }

    public void UseContext(Func<IAllureRegistrationDependencies<TConfiguration>, IAllureExecutionContext> contextFactory)
    {
        this.currentContextFactory = contextFactory;
    }

    public void UseDestination(Func<TConfiguration, IAllureResultsDestination> destinationFactory)
    {
        this.currentDestinationFactory = destinationFactory;
    }

    public void UseLifecycleApi(Func<IAllureRegistrationDependencies<TConfiguration>, IAllureLifecycleApi> lifecycleApiFactory)
    {
        this.currentLifecycleApiFactory = lifecycleApiFactory;
    }

    public void UseModelApi(Func<IAllureRegistrationDependencies<TConfiguration>, IAllureModelApi> modelApiFactory)
    {
        this.currentModelApiFactory = modelApiFactory;
    }

    public void UseParameterSerializer(Func<TConfiguration, IAllureParameterSerializer> serializerFactory)
    {
        this.currentSerializerFactory = serializerFactory;
    }

    public void UseParameterSerializer(Func<IAllureParameterSerializer> serializerFactory)
    {
        this.currentSerializerFactory = (_) => serializerFactory();
    }

    public void RegisterInProcessEndpoint(
        string endpointId,
        Action<IAllureRuntime<TConfiguration>, IAllureInProcessEndpointRegistrationContext<TConfiguration, TEndpointHook>> endpointRegistration
    )
    {
        this.currentEndpointRegistration = (endpointId, endpointRegistration);
    }

    public IAllureRuntime<TConfiguration> Build()
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
            var endpointRouteBuilder =
                new AllureInProcessRouteBuilder<TConfiguration, TEndpointHook>(runtimeName, routeId, runtime);
            routeRegistration(runtime, endpointRouteBuilder);
            var route = endpointRouteBuilder.Build();
            AllureRuntimeRouter.Install(route);
        }

        return runtime;
    }

    protected virtual IAllureRuntime<TConfiguration> CreateRuntimeInstance(
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
            provider?.SetUp(this);
        }

        return ReferenceEquals(configurationSourcesFactoryBefore, this.currentConfigurationSourcesFactory)
            ? preHookConfiguration
            : this.ResolveConfiguration();
    }
}
