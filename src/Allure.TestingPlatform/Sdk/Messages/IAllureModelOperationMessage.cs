using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Represents a message that operates on an Allure lifecycle context.
/// </summary>
public interface IAllureModelOperationMessage
{
    /// <summary>
    /// Gets the affected context identifier.
    /// </summary>
    IAllureExecutionStateUid ContextUid { get; }

    /// <summary>
    /// Applies the message to the live Allure runtime.
    /// </summary>
    void ApplyTo(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> allureRuntime);
}
