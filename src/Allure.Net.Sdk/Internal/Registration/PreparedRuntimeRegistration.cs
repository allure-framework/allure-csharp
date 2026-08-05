using System;
using Allure.Runtime;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Registration;

internal class PreparedRuntimeRegistration<TConfiguration, TEndpointRegistrationContext, TEndpointHook, TRuntime, TIntegrationSnapshot>(
    string runtimeName,
    TConfiguration configuration,
    AllureRuntimeRegistrationSnapshot<TConfiguration, TEndpointRegistrationContext, TEndpointHook, TRuntime> commonSnapshot,
    TIntegrationSnapshot integrationSnapshot,
    Func<RuntimeCreationArguments<TConfiguration>, TIntegrationSnapshot, TRuntime> runtimeFactory,
    Func<AllureRouteBuilderArgs<TConfiguration, TRuntime>, TIntegrationSnapshot, AllureInProcessRouteBuilder<
        TConfiguration,
        TEndpointRegistrationContext,
        TEndpointHook,
        TRuntime
    >> routeBuilderFactory
) :
    IPreparedRuntimeRegistration<TConfiguration, TRuntime>

    where TConfiguration : AllureConfiguration
    where TEndpointRegistrationContext : IAllureInProcessEndpointRegistrationContext<TConfiguration, TRuntime>
    where TEndpointHook : IAllureInProcessEndpointRegistrationHook<TConfiguration, TEndpointRegistrationContext, TRuntime>
    where TRuntime : IAllureRuntime<TConfiguration>
{
    public TConfiguration Configuration => configuration;

    public IAllureRuntimeRegistration<TRuntime> Build()
    {
        var parameterSerializer = commonSnapshot.SerializerFactory(configuration);
        var destination = commonSnapshot.DestinationFactory(configuration);

        var dependencies = new AllureRegistrationDependencies<TConfiguration>(
            configuration,
            parameterSerializer,
            new LateBoundReference<IAllureRuntime<TConfiguration>>()
        );

        var context = commonSnapshot.ContextFactory(dependencies);
        var lifecycleApi = commonSnapshot.LifecycleApiFactory(dependencies);
        var modelApi = commonSnapshot.ModelApiFactory(dependencies);

        RuntimeCreationArguments<TConfiguration> runtimeCreationArguments = new(
            Configuration: configuration,
            ParameterSerializer: parameterSerializer,
            Destination: destination,
            Context: context,
            LifecycleApi: lifecycleApi,
            ModelApi: modelApi
        );

        var runtime = runtimeFactory(runtimeCreationArguments, integrationSnapshot);

        dependencies.BindRuntime(runtime);

        IDisposable? endpointRegistration = null;

        if (commonSnapshot.EndpointRegistration is var (routeId, routeRegistration))
        {
            AllureRouteBuilderArgs<TConfiguration, TRuntime> endpointBuildArgs = new(
                runtimeName,
                routeId,
                runtime,
                commonSnapshot.UseRuleBasedSerializer,
                commonSnapshot.RuleBasedSerializerRegistrations
            );
            var endpointRouteBuilder = routeBuilderFactory(endpointBuildArgs, integrationSnapshot);

            routeRegistration(runtime, endpointRouteBuilder);

            var route = endpointRouteBuilder.Build();
            endpointRegistration = AllureRuntimeRouter.Install(route);
        }

        return new AllureRuntimeRegistration<TRuntime>(runtime, endpointRegistration);
    }
}
