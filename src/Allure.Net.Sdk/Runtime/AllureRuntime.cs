using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Results;

namespace Allure.Sdk.Runtime;

/// <summary>
/// Provides the standard implementation of an Allure runtime.
/// </summary>
/// <remarks>
/// Integration authors may derive from this class to expose additional
/// integration-specific services.
/// </remarks>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <param name="configuration">The runtime configuration.</param>
/// <param name="parameterSerializer">The parameter serializer.</param>
/// <param name="resultsDestination">The destination for generated results.</param>
/// <param name="context">The execution-context service.</param>
/// <param name="lifecycleApi">The lifecycle API.</param>
/// <param name="modelApi">The model API.</param>
public class AllureRuntime<TConfiguration>(
    TConfiguration configuration,
    IAllureParameterSerializer parameterSerializer,
    IAllureResultsDestination resultsDestination,
    IAllureExecutionContext context,
    IAllureLifecycleApi lifecycleApi,
    IAllureModelApi modelApi
) :
    IAllureRuntime<TConfiguration>

    where TConfiguration : AllureConfiguration
{
    AllureConfiguration IAllureRuntimeBase.Configuration => this.Configuration;

    /// <inheritdoc/>
    public TConfiguration Configuration { get; } = configuration;

    /// <inheritdoc/>
    public IAllureParameterSerializer ParameterSerializer { get; } = parameterSerializer;

    /// <inheritdoc/>
    public IAllureResultsDestination ResultsDestination { get; } = resultsDestination;

    /// <inheritdoc/>
    public IAllureExecutionContext ContextApi { get; } = context;

    /// <inheritdoc/>
    public IAllureLifecycleApi LifecycleApi { get; } = lifecycleApi;

    /// <inheritdoc/>
    public IAllureModelApi ModelApi { get; } = modelApi;
}
