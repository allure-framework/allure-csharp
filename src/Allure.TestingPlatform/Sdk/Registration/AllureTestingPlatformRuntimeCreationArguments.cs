using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk.Registration;

public sealed record class AllureTestingPlatformRuntimeCreationArguments(
    ILogger Logger,
    ICorrelationStrategy CorrelationStrategy,
    ICorrelationContext CorrelationContext,
    ExecutionStateContext ExecutionStateContext
);
