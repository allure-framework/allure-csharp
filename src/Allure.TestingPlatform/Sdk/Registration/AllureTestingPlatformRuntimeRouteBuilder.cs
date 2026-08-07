using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Registration;

public abstract class AllureTestingPlatformRuntimeRouteBuilder<
    TConfiguration,
    TContext,
    THook,
    TRuntime
>(
    AllureRouteBuilderArgs<TConfiguration, TRuntime> args
) :
    AllureInProcessRouteBuilder<TConfiguration, TContext, THook, TRuntime>(args),
    IAllureTestingPlatformEndpointRegistrationContext<TConfiguration, TRuntime>

    where TConfiguration : AllureTestingPlatformConfiguration
    where TContext : IAllureTestingPlatformEndpointRegistrationContext<
        TConfiguration,
        TRuntime
    >
    where THook : IAllureTestingPlatformEndpointRegistrationHook<
        TConfiguration,
        TContext,
        TRuntime
    >
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>;

public abstract class AllureTestingPlatformRuntimeRouteBuilder<
    TConfiguration,
    TContext,
    THook
>(
    AllureRouteBuilderArgs<
        TConfiguration,
        IAllureTestingPlatformRuntime<TConfiguration>
    > args
) :
    AllureTestingPlatformRuntimeRouteBuilder<
        TConfiguration,
        TContext,
        THook,
        IAllureTestingPlatformRuntime<TConfiguration>
    >(args),
    IAllureTestingPlatformEndpointRegistrationContext<TConfiguration>

    where TConfiguration : AllureTestingPlatformConfiguration
    where TContext : IAllureTestingPlatformEndpointRegistrationContext<
        TConfiguration
    >
    where THook : IAllureTestingPlatformEndpointRegistrationHook<
        TConfiguration,
        TContext
    >;

public class AllureTestingPlatformRuntimeRouteBuilder(
    AllureRouteBuilderArgs<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
    > args
) :
    AllureTestingPlatformRuntimeRouteBuilder<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformEndpointRegistrationContext,
        IAllureTestingPlatformEndpointRegistrationHook
    >(args),
    IAllureTestingPlatformEndpointRegistrationContext
{
    protected override IAllureTestingPlatformEndpointRegistrationContext RegistrationContext =>
        this;
}
