using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Results;
using Allure.Sdk.TestPlan;

namespace Allure.Sdk.Runtime;

/// <summary>
/// Exposes the configuration and services that make up an Allure runtime.
/// </summary>
public interface IAllureRuntimeBase
{
    /// <summary>
    /// Gets the runtime configuration.
    /// </summary>
    AllureConfiguration Configuration { get; }

    /// <summary>
    /// Gets the execution-context service.
    /// </summary>
    IAllureExecutionContext ContextApi { get; }

    /// <summary>
    /// Gets the lifecycle API.
    /// </summary>
    IAllureLifecycleApi LifecycleApi { get; }

    /// <summary>
    /// Gets the model API.
    /// </summary>
    IAllureModelApi ModelApi { get; }

    /// <summary>
    /// Gets the destination for generated results.
    /// </summary>
    IAllureResultsDestination ResultsDestination { get; }

    /// <summary>
    /// Gets the parameter serializer.
    /// </summary>
    IAllureParameterSerializer ParameterSerializer { get; }

    /// <summary>
    /// Gets the test plan associated with this runtime.
    /// </summary>
    /// <remarks>
    /// A <see langword="null"/> value means that no test plan was provided.
    /// Integrations must treat this as no test-plan filtering and allow all tests
    /// to run.
    /// </remarks>
    AllureTestPlan? TestPlan { get; }
}
