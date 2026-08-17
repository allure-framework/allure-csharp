using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk.Registration;

/// <summary>
/// Contains Microsoft Testing Platform-specific services used to create an Allure runtime.
/// </summary>
/// <param name="Logger">The runtime logger.</param>
/// <param name="CorrelationStrategy">The strategy used to correlate incoming messages.</param>
/// <param name="CorrelationContext">The context that provides the current correlation identifier.</param>
/// <param name="ExecutionStateContext">The context that tracks the current execution state.</param>
public sealed record class AllureTestingPlatformRuntimeArguments(
    ILogger Logger,
    ICorrelationStrategy CorrelationStrategy,
    ICorrelationContext CorrelationContext,
    ExecutionStateContext ExecutionStateContext
);
