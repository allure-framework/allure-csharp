using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Results;
using Allure.Sdk.TestPlan;

namespace Allure.Sdk.Runtime;

/// <summary>
/// Provides the standard implementation of an Allure runtime.
/// </summary>
/// <remarks>
/// Integration authors may derive from this class to expose additional
/// integration-specific services.
/// </remarks>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <param name="args">The runtime creation arguments.</param>
public class AllureRuntime<TConfiguration>(
    RuntimeCreationArguments<TConfiguration> args
) :
    IAllureRuntime<TConfiguration>

    where TConfiguration : AllureConfiguration
{
    AllureConfiguration IAllureRuntimeBase.Configuration => this.Configuration;

    /// <inheritdoc/>
    public TConfiguration Configuration { get; } = args.Configuration;

    /// <inheritdoc/>
    public IAllureParameterSerializer ParameterSerializer { get; } = args.ParameterSerializer;

    /// <inheritdoc/>
    public IAllureResultsDestination ResultsDestination { get; } = args.Destination;

    /// <inheritdoc/>
    public IAllureExecutionContext ContextApi { get; } = args.Context;

    /// <inheritdoc/>
    public IAllureLifecycleApi LifecycleApi { get; } = args.LifecycleApi;

    /// <inheritdoc/>
    public IAllureModelApi ModelApi { get; } = args.ModelApi;

    /// <inheritdoc/>
    public AllureTestPlan TestPlan { get; } = args.TestPlan;
}

/// <summary>
/// Provides the standard implementation of an Allure runtime using
/// <see cref="AllureConfiguration"/>.
/// </summary>
/// <param name="args">The runtime creation arguments.</param>
public class AllureRuntime(
    RuntimeCreationArguments<AllureConfiguration> args
) :
    AllureRuntime<AllureConfiguration>(args),
    IAllureRuntime;
