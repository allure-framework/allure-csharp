using Allure.Sdk.Configuration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Captures the integration-specific factories used to construct a custom runtime and
/// its in-process route builder.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
public interface IAllureRuntimeIntegrationSnapshot<TConfiguration, TRuntime>
    where TConfiguration : AllureConfiguration
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
    IPreparedInProcessRouteBuilder CreateRouteBuilder(
        AllureRouteBuilderArgs<TConfiguration, TRuntime> args
    );
}

/// <summary>
/// Captures the integration-specific factories used to construct a standard runtime
/// with custom configuration.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
public interface IAllureRuntimeIntegrationSnapshot<TConfiguration> :
    IAllureRuntimeIntegrationSnapshot<
        TConfiguration,
        IAllureRuntime<TConfiguration>
    >

    where TConfiguration : AllureConfiguration;

/// <summary>
/// Captures the factories used to construct a standard Allure runtime and its
/// in-process route builder.
/// </summary>
public interface IAllureRuntimeIntegrationSnapshot :
    IAllureRuntimeIntegrationSnapshot<AllureConfiguration>;
