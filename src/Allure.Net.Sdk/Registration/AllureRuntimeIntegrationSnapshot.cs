using Allure.Sdk.Configuration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Provides the standard runtime factory and an integration-specific in-process route
/// builder factory.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
public abstract class AllureRuntimeIntegrationSnapshot<TConfiguration>() :
    IAllureRuntimeIntegrationSnapshot<TConfiguration>

    where TConfiguration : AllureConfiguration
{
    /// <inheritdoc/>
    public abstract IPreparedInProcessRouteBuilder CreateRouteBuilder(
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
    AllureRuntimeIntegrationSnapshot<AllureConfiguration>,
    IAllureRuntimeIntegrationSnapshot
{
    /// <inheritdoc/>
    public override IPreparedInProcessRouteBuilder CreateRouteBuilder(
        AllureRouteBuilderArgs<AllureConfiguration, IAllureRuntime<AllureConfiguration>> args
    ) =>
        new AllureInProcessRouteBuilder(args);
}
