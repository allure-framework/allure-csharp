using Allure.Abstractions;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk.Runtime;

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
    public ILogger Logger => logger;

    public ICorrelationStrategy CorrelationStrategy => correlationStrategy;

    public ICorrelationContext CorrelationContext => correlationContext;

    public ExecutionStateContext ExecutionStateContext => executionStateContext;
}

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
