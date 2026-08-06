using Allure.Sdk.Configuration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Provides the standard runtime factory and an integration-specific in-process route
/// builder factory.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TEndpointRegistrationContext">The endpoint registration context type.</typeparam>
/// <typeparam name="TEndpointHook">The endpoint registration hook type.</typeparam>
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
    /// <inheritdoc/>
    public abstract AllureInProcessRouteBuilder<
        TConfiguration,
        TEndpointRegistrationContext,
        TEndpointHook,
        IAllureRuntime<TConfiguration>
    > CreateRouteBuilder(
        AllureRouteBuilderArgs<TConfiguration, IAllureRuntime<TConfiguration>> args
    );

    /// <inheritdoc/>
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

/// <summary>
/// Provides the factories used to construct a standard Allure runtime and its
/// in-process route builder.
/// </summary>
public class AllureRuntimeIntegrationSnapshot() :
    AllureRuntimeIntegrationSnapshot<
        AllureConfiguration,
        IAllureInProcessEndpointRegistrationContext,
        IAllureInProcessEndpointRegistrationHook
    >,
    IAllureRuntimeIntegrationSnapshot
{
    /// <inheritdoc/>
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
