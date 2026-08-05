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
    TIntegrationSnapshot integrationSnapshot
) :
    IPreparedRuntimeRegistration<TConfiguration, TRuntime>

    where TConfiguration : AllureConfiguration
    where TEndpointRegistrationContext : IAllureInProcessEndpointRegistrationContext<TConfiguration, TRuntime>
    where TEndpointHook : IAllureInProcessEndpointRegistrationHook<TConfiguration, TEndpointRegistrationContext, TRuntime>
    where TIntegrationSnapshot : IAllureRuntimeIntegrationSnapshot<
        TConfiguration,
        TEndpointRegistrationContext,
        TEndpointHook,
        TRuntime
    >
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

        var runtime = integrationSnapshot.CreateRuntime(runtimeCreationArguments);

        try
        {
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
                var endpointRouteBuilder = integrationSnapshot.CreateRouteBuilder(endpointBuildArgs);

                routeRegistration(runtime, endpointRouteBuilder);

                var route = endpointRouteBuilder.Build();
                endpointRegistration = AllureRuntimeRouter.Install(route);
            }

            return new AllureRuntimeRegistration<TRuntime>(runtime, endpointRegistration);
        }
        catch
        {
            (runtime as IDisposable)?.Dispose();
            throw;
        }
    }
}
