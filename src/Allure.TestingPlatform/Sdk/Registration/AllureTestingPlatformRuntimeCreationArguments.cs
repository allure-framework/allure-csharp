using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Microsoft.Testing.Platform.Logging;
using Microsoft.Testing.Platform.Messages;

namespace Allure.TestingPlatform.Sdk.Registration;

public record class AllureTestingPlatformRuntimeCreationArguments(
    ILogger Logger,
    IMessageBus MessageBus,
    ICorrelationStrategy CorrelationStrategy,
    ICorrelationContext CorrelationContext,
    ExecutionStateContext ExecutionStateContext
);
