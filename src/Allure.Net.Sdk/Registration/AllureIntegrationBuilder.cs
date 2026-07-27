using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Runtime;
using Allure.Sdk.Configuration;
using Allure.Sdk.Extensions;
using Allure.Sdk.Internal.Registration;
using Allure.Sdk.Internal.Runtime;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

public class AllureIntegrationBuilder<TConfiguration, THook>(string runtimeName) :
    IAllureIntegrationRegistrationContext<TConfiguration, THook>,
    IAllureRuntimeRegistrationContext<TConfiguration>

    where TConfiguration : AllureConfiguration, new()
    where THook : IAllureRuntimeRegistrationHook<TConfiguration>
{
    Func<IEnumerable<IAllureConfigurationSource<TConfiguration>>> currentConfigurationSourcesFactory =
        AllureRegistrationDefaults<TConfiguration>.ConfigurationSources;

    Func<TConfiguration, IEnumerable<IAllureRuntimeRegistrationHookProvider<TConfiguration, THook>>> currentHooksProviderFactory =
        AllureRegistrationDefaults<TConfiguration, THook>.HookProviders;

    Func<IAllureRegistrationDependencies<TConfiguration>, IAllureRuntimeContext> currentContextFactory =
        AllureRegistrationDefaults<TConfiguration>.Context;

    Func<IAllureRegistrationDependencies<TConfiguration>, IAllureLifecycleApi> currentLifecycleApiFactory =
        AllureRegistrationDefaults<TConfiguration>.LifecycleApi;

    Func<IAllureRegistrationDependencies<TConfiguration>, IAllureModelApi> currentModelApiFactory =
        AllureRegistrationDefaults<TConfiguration>.ModelApi;

    Func<TConfiguration, IAllureParameterSerializer> currentSerializerFactory =
        (_) => AllureRegistrationDefaults.ParameterSerializer();

    Func<TConfiguration, IAllureResultsDestination> currentDestinationFactory =
        AllureRegistrationDefaults<TConfiguration>.Destination;

    (
        string id,
        Action<IAllureRuntime<TConfiguration>, IAllureInProcessEndpointRegistrationContext<TConfiguration>>
    )? currentEndpointRegistration = null;

    public void UseConfigurationSources(Func<IEnumerable<IAllureConfigurationSource<TConfiguration>>> sourcesFactory)
    {
        this.currentConfigurationSourcesFactory = sourcesFactory;
    }

    public void UseRegistrationHooks(Func<TConfiguration, IEnumerable<IAllureRuntimeRegistrationHookProvider<TConfiguration, THook>>> hookProvidersFactory)
    {
        this.currentHooksProviderFactory = hookProvidersFactory;
    }

    public void UseContext(Func<IAllureRegistrationDependencies<TConfiguration>, IAllureRuntimeContext> contextFactory)
    {
        this.currentContextFactory = contextFactory;
    }

    public void UseDestination(Func<TConfiguration, IAllureResultsDestination> destinationFactory)
    {
        this.currentDestinationFactory = destinationFactory;
    }

    public void UseDestination(Func<IAllureResultsDestination> destinationFactory)
    {
        this.currentDestinationFactory = (_) => destinationFactory();
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
        Action<IAllureRuntime<TConfiguration>, IAllureInProcessEndpointRegistrationContext<TConfiguration>> endpointRegistration
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
                new AllureInProcessEndpointBuilder<TConfiguration>(runtimeName, routeId, runtime);
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
        IAllureRuntimeContext context,
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

        foreach (var provider in this.currentHooksProviderFactory(preHookConfiguration))
        {
            if (provider.HasHook)
            {
                provider.GetHook().SetUp(this);
            }
        }

        return ReferenceEquals(configurationSourcesFactoryBefore, this.currentConfigurationSourcesFactory)
            ? preHookConfiguration
            : this.ResolveConfiguration();
    }
}
