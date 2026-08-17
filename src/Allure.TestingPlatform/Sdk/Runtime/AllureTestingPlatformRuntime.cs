using Allure.Abstractions;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk.Runtime;

/// <summary>
/// Provides an Allure Microsoft Testing Platform runtime with a specific configuration type.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <param name="configuration">The resolved runtime configuration.</param>
/// <param name="parameterSerializer">The serializer used for Allure parameter values.</param>
/// <param name="resultsDestination">The destination to which Allure result files are written.</param>
/// <param name="context">The Allure execution context.</param>
/// <param name="lifecycleApi">The Allure lifecycle API.</param>
/// <param name="modelApi">The Allure object model API.</param>
/// <param name="logger">The runtime logger.</param>
/// <param name="correlationStrategy">The strategy used to correlate incoming messages.</param>
/// <param name="correlationContext">The context that provides the current correlation identifier.</param>
/// <param name="executionStateContext">The context that tracks the current execution state.</param>
public class AllureTestingPlatformRuntime<TConfiguration>(
    TConfiguration configuration,
    IAllureParameterSerializer parameterSerializer,
    IAllureResultsDestination resultsDestination,
    IAllureExecutionContext context,
    IAllureLifecycleApi lifecycleApi,
    IAllureModelApi modelApi,
    ILogger logger,
    ICorrelationStrategy correlationStrategy,
    ICorrelationContext correlationContext,
    ExecutionStateContext executionStateContext
) :
    AllureRuntime<TConfiguration>(
        configuration,
        parameterSerializer,
        resultsDestination,
        context,
        lifecycleApi,
        modelApi
    ),
    IAllureTestingPlatformRuntime<TConfiguration>

    where TConfiguration : AllureTestingPlatformConfiguration
{
    /// <inheritdoc />
    public ILogger Logger => logger;

    /// <inheritdoc />
    public ICorrelationStrategy CorrelationStrategy => correlationStrategy;

    /// <inheritdoc />
    public ICorrelationContext CorrelationContext => correlationContext;

    /// <inheritdoc />
    public ExecutionStateContext ExecutionStateContext => executionStateContext;
}

/// <summary>
/// Provides the default Allure Microsoft Testing Platform runtime.
/// </summary>
/// <param name="configuration">The resolved runtime configuration.</param>
/// <param name="parameterSerializer">The serializer used for Allure parameter values.</param>
/// <param name="resultsDestination">The destination to which Allure result files are written.</param>
/// <param name="context">The Allure execution context.</param>
/// <param name="lifecycleApi">The Allure lifecycle API.</param>
/// <param name="modelApi">The Allure object model API.</param>
/// <param name="logger">The runtime logger.</param>
/// <param name="correlationStrategy">The strategy used to correlate incoming messages.</param>
/// <param name="correlationContext">The context that provides the current correlation identifier.</param>
/// <param name="executionStateContext">The context that tracks the current execution state.</param>
public class AllureTestingPlatformRuntime(
    AllureTestingPlatformConfiguration configuration,
    IAllureParameterSerializer parameterSerializer,
    IAllureResultsDestination resultsDestination,
    IAllureExecutionContext context,
    IAllureLifecycleApi lifecycleApi,
    IAllureModelApi modelApi,
    ILogger logger,
    ICorrelationStrategy correlationStrategy,
    ICorrelationContext correlationContext,
    ExecutionStateContext executionStateContext
) :
    AllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>(
        configuration,
        parameterSerializer,
        resultsDestination,
        context,
        lifecycleApi,
        modelApi,
        logger,
        correlationStrategy,
        correlationContext,
        executionStateContext
    ),
    IAllureTestingPlatformRuntime;
