using Allure.TestingPlatform.Sdk.ContextIdentifiers;
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
    IAllureContextUid ContextUid { get; }

    /// <summary>
    /// Applies the message to the live Allure runtime.
    /// </summary>
    void ApplyTo(LiveAllureTestingPlatformRuntime allureRuntime);
}
