using Allure.Sdk.Configuration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Captures the integration-specific factories used to construct a custom runtime and
/// its in-process route builder.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TEndpointRegistrationContext">The endpoint registration context type.</typeparam>
/// <typeparam name="TEndpointHook">The endpoint registration hook type.</typeparam>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
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
    /// <summary>
    /// Creates the runtime from its resolved components.
    /// </summary>
    /// <param name="args">The resolved runtime components.</param>
    /// <returns>The constructed runtime.</returns>
    TRuntime CreateRuntime(RuntimeCreationArguments<TConfiguration> args);

    /// <summary>
    /// Creates the builder used to configure the runtime's in-process route.
    /// </summary>
    /// <param name="args">The resolved route builder arguments.</param>
    /// <returns>The in-process route builder.</returns>
    AllureInProcessRouteBuilder<
        TConfiguration,
        TEndpointRegistrationContext,
        TEndpointHook,
        TRuntime
    > CreateRouteBuilder(AllureRouteBuilderArgs<TConfiguration, TRuntime> args);
}

/// <summary>
/// Captures the integration-specific factories used to construct a standard runtime
/// with custom configuration and endpoint types.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TEndpointRegistrationContext">The endpoint registration context type.</typeparam>
/// <typeparam name="TEndpointHook">The endpoint registration hook type.</typeparam>
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

/// <summary>
/// Captures the factories used to construct a standard Allure runtime and its
/// in-process route builder.
/// </summary>
public interface IAllureRuntimeIntegrationSnapshot :
    IAllureRuntimeIntegrationSnapshot<
        AllureConfiguration,
        IAllureInProcessEndpointRegistrationContext,
        IAllureInProcessEndpointRegistrationHook
    >;
