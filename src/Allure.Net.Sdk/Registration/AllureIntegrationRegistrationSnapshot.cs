using Allure.Sdk.Configuration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

public abstract class AllureRuntimeIntegrationSnapshot<
    TConfiguration,
    TEndpointRegistrationContext,
    TEndpointHook
>() :
    IAllureRuntimeIntegrationSnapshot<
        TConfiguration,
        TEndpointRegistrationContext,
        TEndpointHook
    >

    where TConfiguration : AllureConfiguration
    where TEndpointRegistrationContext : IAllureInProcessEndpointRegistrationContext<TConfiguration>
    where TEndpointHook : IAllureInProcessEndpointRegistrationHook<
        TConfiguration,
        TEndpointRegistrationContext
    >
{
    public abstract AllureInProcessRouteBuilder<
        TConfiguration,
        TEndpointRegistrationContext,
        TEndpointHook,
        IAllureRuntime<TConfiguration>
    > CreateRouteBuilder(
        AllureRouteBuilderArgs<TConfiguration, IAllureRuntime<TConfiguration>> args
    );

    public IAllureRuntime<TConfiguration> CreateRuntime(
        RuntimeCreationArguments<TConfiguration> args
    ) =>
        new AllureRuntime<TConfiguration>(
            args.Configuration,
            args.ParameterSerializer,
            args.Destination,
            args.Context,
            args.LifecycleApi,
            args.ModelApi
        );
}

public class AllureRuntimeIntegrationSnapshot() :
    AllureRuntimeIntegrationSnapshot<
        AllureConfiguration,
        IAllureInProcessEndpointRegistrationContext,
        IAllureInProcessEndpointRegistrationHook
    >,
    IAllureRuntimeIntegrationSnapshot
{
    public override AllureInProcessRouteBuilder<
        AllureConfiguration,
        IAllureInProcessEndpointRegistrationContext,
        IAllureInProcessEndpointRegistrationHook,
        IAllureRuntime<AllureConfiguration>
    > CreateRouteBuilder(
        AllureRouteBuilderArgs<AllureConfiguration, IAllureRuntime<AllureConfiguration>> args
    ) =>
        new AllureInProcessRouteBuilder(args);
}
