using Allure.Sdk.Configuration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

public interface IAllureRuntimeIntegrationSnapshot<
    TConfiguration,
    TEndpointRegistrationContext,
    TEndpointHook,
    TRuntime
>
    where TConfiguration : AllureConfiguration
    where TEndpointRegistrationContext : IAllureInProcessEndpointRegistrationContext<
        TConfiguration,
        TRuntime
    >
    where TEndpointHook : IAllureInProcessEndpointRegistrationHook<
        TConfiguration,
        TEndpointRegistrationContext,
        TRuntime
    >
    where TRuntime : IAllureRuntime<TConfiguration>
{
    TRuntime CreateRuntime(RuntimeCreationArguments<TConfiguration> args);

    AllureInProcessRouteBuilder<
        TConfiguration,
        TEndpointRegistrationContext,
        TEndpointHook,
        TRuntime
    > CreateRouteBuilder(AllureRouteBuilderArgs<TConfiguration, TRuntime> args);
}

public interface IAllureRuntimeIntegrationSnapshot<
    TConfiguration,
    TEndpointRegistrationContext,
    TEndpointHook
> :
    IAllureRuntimeIntegrationSnapshot<
        TConfiguration,
        TEndpointRegistrationContext,
        TEndpointHook,
        IAllureRuntime<TConfiguration>
    >

    where TConfiguration : AllureConfiguration
    where TEndpointRegistrationContext : IAllureInProcessEndpointRegistrationContext<TConfiguration>
    where TEndpointHook : IAllureInProcessEndpointRegistrationHook<
        TConfiguration,
        TEndpointRegistrationContext
    >;

public interface IAllureRuntimeIntegrationSnapshot :
    IAllureRuntimeIntegrationSnapshot<
        AllureConfiguration,
        IAllureInProcessEndpointRegistrationContext,
        IAllureInProcessEndpointRegistrationHook
    >;
