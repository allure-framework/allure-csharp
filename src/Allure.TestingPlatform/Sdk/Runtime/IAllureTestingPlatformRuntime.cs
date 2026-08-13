using Allure.Sdk.Runtime;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Microsoft.Testing.Platform.Logging;
using Microsoft.Testing.Platform.Messages;

namespace Allure.TestingPlatform.Sdk.Runtime;

public interface IAllureTestingPlatformRuntimeBase :
    IAllureRuntimeBase
{
    ILogger Logger { get; }

    IMessageBus MessageBus { get; }

    ICorrelationStrategy CorrelationStrategy { get; }

    ICorrelationContext CorrelationContext { get; }

    ExecutionStateContext ExecutionStateContext { get; }
}

public interface IAllureTestingPlatformRuntime<out TConfiguration> :
    IAllureTestingPlatformRuntimeBase,
    IAllureRuntime<TConfiguration>

    where TConfiguration : AllureTestingPlatformConfiguration;

public interface IAllureTestingPlatformRuntime :
    IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>;
